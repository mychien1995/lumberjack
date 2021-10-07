using System;

namespace Lumberjack.Server.Models
{
    public class LogEntry
    {
        public Guid Id { get;  }
        public DateTime CreateDate { get;  }
        public long Timestamp { get;  }
        public SystemLogLevel LogLevel { get;  }
        public string Namespace { get;  }
        public string Message { get;  }
        public string Request { get;  }
        public string RequestContext { get;  }
        public Guid? ApplicationId { get;  }
        public string Instance { get;  }

        public LogEntry(Guid id, DateTime createDate, long timestamp, SystemLogLevel logLevel, string ns, string message,
            string request, string requestContext, Guid? applicationId, string instance)
        {
            Id = id;
            CreateDate = createDate;
            Timestamp = timestamp;
            LogLevel = logLevel;
            Namespace = ns;
            Message = message;
            Request = request;
            RequestContext = requestContext;
            ApplicationId = applicationId;
            Instance = instance;
        }
    }
}
