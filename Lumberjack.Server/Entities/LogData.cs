using System;
using System.Collections.Generic;

#nullable disable

namespace Lumberjack.Server.Entities
{
    public partial class LogData
    {
        public Guid Id { get; set; }
        public DateTime CreateDate { get; set; }
        public long? Timestamp { get; set; }
        public int LogLevel { get; set; }
        public string Namespace { get; set; }
        public string Message { get; set; }
        public string Request { get; set; }
        public string RequestContext { get; set; }
        public Guid? ApplicationId { get; set; }
        public string Instance { get; set; }
    }
}
