using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web;
using log4net.spi;
using Newtonsoft.Json;
using Sitecore.Diagnostics;

namespace Lumberjack.Log4Net.Appender
{
    public interface ILumberjackLogCollector
    {
        void Collect(LoggingEvent logEvent);
    }

    public class LumberjackLogCollector : ILumberjackLogCollector
    {
        private readonly LumberjackConfiguration _configuration;
        private readonly ConcurrentQueue<ConvertedLogEvent> _queue = new ConcurrentQueue<ConvertedLogEvent>();
        private readonly HttpClient _singleClient;
        private int _failedAttempt = 0;

        public LumberjackLogCollector()
        {
            _configuration = LumberjackConfiguration.Current;
            if (!_configuration.Enabled)
            {
                Log.Info("Lumberjack is disabled", this);
                return;
            }

            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback += (a, b, c, d) => true;
            _singleClient = new HttpClient(handler);
            //A bit unsafe since no cancellation
            var dedicatedThread = new Thread(Submit);
            dedicatedThread.Start();
        }

        private void Submit()
        {
            Log.Info("Lumberjack log collector started", this);
            while (true)
            {
                var items = new List<ConvertedLogEvent>(_queue.Count);
                if (!_queue.IsEmpty)
                    while (_queue.TryDequeue(out var log) && items.Count < _configuration.BatchSize)
                        items.Add(log);
                if (items.Count == 0) continue;
                try
                {
                    var message = new HttpRequestMessage(HttpMethod.Post, _configuration.Endpoint);
                    message.Headers.Add("x-api-key", _configuration.ApiKey);
                    message.Headers.Add("x-instance", _configuration.Instance);
                    message.Content = new StringContent(JsonConvert.SerializeObject(new { Data = items }), Encoding.UTF8,
                        "application/json");
                    _singleClient.SendAsync(message).Wait(_configuration.RequestTimeoutMs);
                }
                catch (Exception ex)
                {
                    Log.Error("Error on submit logs", ex, this);
                    _failedAttempt++;
                }

                if (_failedAttempt > 10)
                {
                    Log.Error("Temporary disable log colletor for 5 minutes", this);
                    Thread.Sleep(300000);
                }
                Thread.Sleep((int)_configuration.SubmitIntervalMs);
            }
        }

        public void Collect(LoggingEvent logEvent)
        {
            if (!ShouldProcess(logEvent, out var domainEvent)) return;
            if (domainEvent == null) return;
            _queue.Enqueue(domainEvent);
        }

        private bool ShouldProcess(LoggingEvent logEvent, out ConvertedLogEvent domainEvent)
        {
            domainEvent = null;
            try
            {
                if (!_configuration.Enabled) return false;
                if (_queue.Count >= _configuration.MaximumQueueSize) return false;
                var level = logEvent.Level.Value;
                var message = GetMesssage(logEvent);
                if (string.IsNullOrWhiteSpace(message)) return false;
                var ns = GetNamespace(logEvent);
                if (string.IsNullOrEmpty(ns)) return true;
                var matchingNamespace = false;
                foreach (var nsConfiguration in _configuration.Namespaces)
                {
                    if (!ns.StartsWith(nsConfiguration.Namespace, StringComparison.OrdinalIgnoreCase)) continue;
                    var shouldExclude = level < _configuration.MinimumLevel || nsConfiguration.MinimumLevel > level ||
                                        (nsConfiguration.ExcludedText != null &&
                                         nsConfiguration.ExcludedText.Any(e => e.StartsWith(message)));
                    var alwaysInclude = nsConfiguration.AlwaysIncludedText != null &&
                                        nsConfiguration.AlwaysIncludedText.Any(t =>
                                            message.IndexOf(t, StringComparison.OrdinalIgnoreCase) > -1);
                    if (nsConfiguration.Disabled || (!alwaysInclude && shouldExclude)) return false;
                    matchingNamespace = true;
                }
                if (!matchingNamespace && level < _configuration.MinimumLevel) return false;
                var requestContext = GetRequestContext();
                domainEvent = new ConvertedLogEvent(((DateTimeOffset)logEvent.TimeStamp).ToUnixTimeSeconds(), (int)GetLogLevel(logEvent.Level),
                    ns, message,
                    requestContext.Item1, requestContext.Item2);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Error on convert domain event", ex, this);
                return false;
            }
        }

        private string GetMesssage(LoggingEvent logEvent)
        {
            var msg = new StringBuilder(logEvent.RenderedMessage);
            if (!string.IsNullOrEmpty(logEvent.GetExceptionStrRep())) msg.Append(". Exception:" + logEvent.GetExceptionStrRep());
            return msg.ToString();
        }

        private (string, string) GetRequestContext()
        {
            var context = HttpContext.Current;
            if (context == null) return (string.Empty, string.Empty);
            try
            {
                var path = context.Request.Url.AbsoluteUri;
                var body = string.Empty;
                if (context.Request.ContentLength > 0)
                {
                    var stream = new StreamReader(context.Request.InputStream);
                    body = stream.ReadToEndAsync().Result;
                    stream.Dispose();
                }

                var headers = new Dictionary<string, object>();
                foreach (var key in context.Request.Headers.AllKeys)
                    headers[key] = context.Request.Headers[key];
                var requestContext = JsonConvert.SerializeObject(new { headers, body });

                return (path, requestContext);
            }
            catch (HttpException ex)
            {
                if (ex.Message.Contains("Request is not available in this context"))
                    return (string.Empty, string.Empty);
                throw;
            }

        }

        private SystemLogLevel GetLogLevel(Level level)
        {
            var errors = new[] { Level.ALERT, Level.CRITICAL, Level.EMERGENCY, Level.ERROR, Level.FATAL, Level.SEVERE };
            var warn = new[] { Level.WARN };
            var debug = new[] { Level.ALL, Level.TRACE, Level.VERBOSE, Level.OFF };
            if (errors.Contains(level)) return SystemLogLevel.ERROR;
            if (warn.Contains(level)) return SystemLogLevel.WARNING;
            if (debug.Contains(level)) return SystemLogLevel.DEBUG;
            return SystemLogLevel.INFO;
        }

        private static string GetNamespace(LoggingEvent logEvent) => logEvent.LoggerName;
    }
}
