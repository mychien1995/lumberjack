using System;
using Serilog.Events;

namespace Lumberjack.Serilog.Sink
{
    public class LumberjackConfiguration
    {
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public string Instance { get; set; }
        public long SubmitIntervalMs { get; set; }
        public int BatchSize { get; set; }
        public int RequestTimeoutMs { get; set; }
        public bool Enabled { get; set; }
        public LogEventLevel MinimumLevel { get; set; }

        public LumberjackNamespaceConfiguration[] Namespaces { get; set; } =
            Array.Empty<LumberjackNamespaceConfiguration>();
    }

    public class LumberjackNamespaceConfiguration
    {
        public string Namespace { get; set; }
        public LogEventLevel MinimumLevel { get; set; }
        public bool Disabled { get; set; }
    }
}
