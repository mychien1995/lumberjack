using System;
using log4net.Appender;
using log4net.spi;

namespace Lumberjack.Log4Net.Appender
{
    public class LumberjackAppender : AppenderSkeleton
    {
        private static readonly LumberjackLogCollector Collector = new LumberjackLogCollector();
        protected override void Append(LoggingEvent loggingEvent)
        {
            if (loggingEvent.LoggerName.IndexOf("Lumberjack", StringComparison.OrdinalIgnoreCase) > -1 || loggingEvent.Level == Level.OFF) return;
            var configuration = LumberjackConfiguration.Current;
            if (!configuration.Enabled) return;
            Collector.Collect(loggingEvent);
        }
    }
}
