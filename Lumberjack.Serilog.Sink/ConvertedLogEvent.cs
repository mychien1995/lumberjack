namespace Lumberjack.Serilog.Sink
{
    public class ConvertedLogEvent
    {
        public ConvertedLogEvent(long timestamp, int logLevel, string ns, string message, string request, string requestContext)
        {
            Timestamp = timestamp;
            LogLevel = logLevel;
            Namespace = ns;
            Message = message;
            Request = request;
            RequestContext = requestContext;
        }

        public long Timestamp { get; }
        public int LogLevel { get; }
        public string Namespace { get; }
        public string Message { get; }
        public string Request { get; }
        public string RequestContext { get; }
    }
}
