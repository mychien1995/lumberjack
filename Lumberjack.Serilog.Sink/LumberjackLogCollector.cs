using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog.Events;

namespace Lumberjack.Serilog.Sink
{
    public interface ILumberjackLogCollector
    {
        void Collect(LogEvent logEvent);
    }

    public class LumberjackLogCollector : ILumberjackLogCollector
    {
        private readonly LumberjackConfiguration _configuration;
        private readonly ConcurrentQueue<ConvertedLogEvent> _queue = new ConcurrentQueue<ConvertedLogEvent>();
        private readonly HttpClient _singleClient = new HttpClient();
        private readonly ILogger<LumberjackLogCollector> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private int _failedAttempt = 0;

        public LumberjackLogCollector(IOptionsMonitor<LumberjackConfiguration> configuration, ILogger<LumberjackLogCollector> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration.CurrentValue;
            if (!_configuration.Enabled)
            {
                logger.LogInformation("Lumberjack is disabled");
                return;
            }
            //A bit unsafe since no cancellation
            var dedicatedThread = new Thread(Submit);
            dedicatedThread.Start();
        }

        private void Submit()
        {
            _logger.LogInformation("Lumberjack log collector started");
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
                    message.Content = new StringContent(JsonSerializer.Serialize(new { Data = items }), Encoding.UTF8,
                        "application/json");
                    _singleClient.SendAsync(message).Wait(_configuration.RequestTimeoutMs);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error on submit logs");
                    _failedAttempt++;
                    if (_failedAttempt >= 10) break;
                }
                Thread.Sleep((int)_configuration.SubmitIntervalMs);
            }
        }

        public void Collect(LogEvent logEvent)
        {
            if (!ShouldProcess(logEvent)) return;
            var ev = ToDomainEvent(logEvent);
            if (ev == null) return;
            _queue.Enqueue(ev);
        }

        private bool ShouldProcess(LogEvent logEvent)
        {
            if (!_configuration.Enabled) return false;
            var level = logEvent.Level;
            if (level < _configuration.MinimumLevel) return false;
            var ns = GetNamespace(logEvent);
            if (string.IsNullOrEmpty(ns)) return true;
            foreach (var nsConfiguration in _configuration.Namespaces)
            {
                if (ns.StartsWith(nsConfiguration.Namespace, StringComparison.OrdinalIgnoreCase))
                    return !nsConfiguration.Disabled && nsConfiguration.MinimumLevel <= level;
            }
            return true;
        }

        private ConvertedLogEvent ToDomainEvent(LogEvent logEvent)
        {
            try
            {
                var ns = GetNamespace(logEvent);
                var stringBuilder = new StringBuilder();
                using var stringWriter = new StringWriter(stringBuilder);
                logEvent.RenderMessage(stringWriter);
                var requestContext = GetRequestContext();
                return new ConvertedLogEvent(logEvent.Timestamp.ToUnixTimeSeconds(), (int)GetLogLevel(logEvent.Level),
                    ns,
                    stringBuilder.ToString(),
                    requestContext.Item1, requestContext.Item2);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on convert domain event");
                return null;
            }
        }

        private (string, string) GetRequestContext()
        {
            var context = _httpContextAccessor;
            var currentContext = context?.HttpContext;
            var request = currentContext != null && currentContext.Request.Path.HasValue
                ? $"{currentContext.Request.Path.ToUriComponent()}{currentContext.Request.QueryString.ToUriComponent()}" : string.Empty;
            var requestContext = string.Empty;
            if (currentContext == null) return (request, requestContext);
            //using var stream = new StreamReader(currentContext.Request.Body);
            //var body = stream.ReadToEnd();
            var body = currentContext.GetRawBodyString(Encoding.UTF8);
            requestContext = JsonSerializer.Serialize(new { headers = currentContext.Request.Headers.ToDictionary(k => k.Key, v => v.Value.ToArray()), body });

            return (request, requestContext);

        }

        private SystemLogLevel GetLogLevel(LogEventLevel level)
        {
            switch (level)
            {
                case LogEventLevel.Debug:
                case LogEventLevel.Verbose:
                    return SystemLogLevel.DEBUG;
                case LogEventLevel.Error:
                case LogEventLevel.Fatal:
                    return SystemLogLevel.ERROR;
                case LogEventLevel.Warning:
                    return SystemLogLevel.WARNING;
                case LogEventLevel.Information:
                    return SystemLogLevel.INFO;
                default:
                    return SystemLogLevel.OTHER;
            }
        }

        private static string GetNamespace(LogEvent logEvent)
            => logEvent.Properties != null && logEvent.Properties.ContainsKey("SourceContext")
                ? logEvent.Properties["SourceContext"].ToString().Trim('"')
                : "";
    }

    public class DummyLogCollector : ILumberjackLogCollector
    {
        private readonly List<LogEvent> _deferredLogs;

        public DummyLogCollector(List<LogEvent> deferredLogs)
        {
            _deferredLogs = deferredLogs;
        }

        public void Collect(LogEvent logEvent)
        {
            _deferredLogs.Add(logEvent);
        }
    }
}
