using System;
using System.Collections.Generic;
using System.Linq;
using Lumberjack.Server.Extensions;
using Lumberjack.Server.Models;

namespace Lumberjack.Server.Services
{
    public interface ILogReceiver
    {
        InputValidationResult ValidateAndDispatch(string apiKey, string instance, RawLogInput[] logs);
    }

    public class LogReceiver : ILogReceiver
    {
        private readonly IApplicationCache _applicationCache;
        private readonly ILogWorkerPool _logWorkerPool;

        public LogReceiver(IApplicationCache applicationCache, ILogWorkerPool logWorkerPool)
        {
            _applicationCache = applicationCache;
            _logWorkerPool = logWorkerPool;
        }

        public InputValidationResult ValidateAndDispatch(string apiKey, string instance, RawLogInput[] logs)
        {
            if (logs == null || logs.All(l => string.IsNullOrEmpty(l.Message))) return InputValidationResult.LOG_EMPTY;
            var application = _applicationCache.GetApplication(apiKey);
            if (string.IsNullOrEmpty(apiKey) || application == null) return InputValidationResult.INVALID_API_KEY;
            var data = new List<LogEntry>();
            foreach (var log in logs)
            {
                var createdDate = log.Timestamp.UnixTimeStampToDateTime();
                var logEntry = new LogEntry(Guid.NewGuid(), createdDate, log.Timestamp, (SystemLogLevel)log.LogLevel,
                    log.Namespace,
                    log.Message, log.Request, log.RequestContext, application.Id, instance);
                data.Add(logEntry);
            }
            _logWorkerPool.Dispatch(data);
            return InputValidationResult.OK;
        }
    }
}
