namespace Lumberjack.Server.Models
{
    public class RawLogInput
    {
        public long Timestamp { get; set; }
        public int LogLevel { get; set; }
        public string Namespace { get; set; }
        public string Message { get; set; }
        public string Request { get; set; }
        public string RequestContext { get; set; }
    }
}
