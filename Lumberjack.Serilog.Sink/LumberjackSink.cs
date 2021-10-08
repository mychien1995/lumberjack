using System.Collections.Generic;
using System.Threading;
using Serilog.Core;
using Serilog.Events;

namespace Lumberjack.Serilog.Sink
{
    public class LumberjackSink : ILogEventSink
    {
        private readonly List<LogEvent> _deferredLogs = new List<LogEvent>();
        private ILumberjackLogCollector _collector;

        public LumberjackSink()
        {
            _collector = new DummyLogCollector(_deferredLogs);
        }

        public void Emit(LogEvent logEvent)
        {
            _collector.Collect(logEvent);
        }

        public void Initialize(ILumberjackLogCollector collector)
        {
            Interlocked.Exchange(ref _collector, collector);
            foreach (var entry in _deferredLogs)
                collector.Collect(entry);
        }
    }
}
