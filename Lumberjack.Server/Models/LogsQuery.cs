using System;

namespace Lumberjack.Server.Models
{
    public class LogsQuery
    {
        public string Text { get; set; }
        public bool ExcludeText { get; set; }
        public Guid? ApplicationId { get; set; }
        public string Instance { get; set; }
        public int? LogLevel { get; set; }
        public long? StartTime { get; set; }
        public long? EndTime { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 100;

    }
}
