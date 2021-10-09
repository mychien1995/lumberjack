using System.Collections.Generic;
using System.Linq;
using System.Xml;
using log4net.spi;
using Sitecore.Configuration;

namespace Lumberjack.Log4Net.Appender
{
    public class LumberjackConfiguration
    {
        public LumberjackConfiguration()
        {
        }
        public string Endpoint { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string Instance { get; set; } = string.Empty;
        public long SubmitIntervalMs { get; set; } = 3000;
        public int BatchSize { get; set; } = 200;
        public int RequestTimeoutMs { get; set; } = 10000;
        public int MaximumQueueSize { get; set; } = 1000000;
        public bool Enabled { get; set; } = false;
        public int MinimumLevel { get; set; } = Level.INFO.Value;

        public List<LumberjackNamespaceConfiguration> Namespaces { get; set; } =
            new List<LumberjackNamespaceConfiguration>();

        private static LumberjackConfiguration _current;

        public void AddNamespace(XmlNode config)
        {
            var namespaceConfiguration = new LumberjackNamespaceConfiguration();
            if (config.Attributes != null)
            {
                namespaceConfiguration.Namespace = config.Attributes["value"].Value;
                if (config.Attributes["disabled"]?.Value != null && config.Attributes["disabled"].Value == "true")
                    namespaceConfiguration.Disabled = true;
                if (config.Attributes["level"]?.Value != null &&
                    int.TryParse(config.Attributes["level"]?.Value, out var level))
                    namespaceConfiguration.MinimumLevel = level;
                else namespaceConfiguration.MinimumLevel = Level.WARN.Value;
            }
            foreach (XmlNode node in config.ChildNodes)
            {
                if (node.Name == "exclude" && node.HasChildNodes)
                {
                    foreach (XmlNode excludeTextNode in node.ChildNodes)
                    {
                        namespaceConfiguration.ExcludedText.Add(excludeTextNode.InnerText);
                    }
                }
                else if (node.Name == "include" && node.HasChildNodes)
                {
                    foreach (XmlNode includeTextNode in node.ChildNodes)
                    {
                        namespaceConfiguration.AlwaysIncludedText.Add(includeTextNode.InnerText);
                    }
                }
            }
            if(!namespaceConfiguration.Disabled && !string.IsNullOrEmpty(namespaceConfiguration.Namespace))
                Namespaces.Add(namespaceConfiguration);
        }

        public static LumberjackConfiguration From(XmlNode node)
        {
            var instance = new LumberjackConfiguration();
            var childs = new List<XmlNode>();
            foreach (XmlNode child in node.ChildNodes)
                childs.Add(child);
            instance.Endpoint = childs.First(n => n.Name == nameof(Endpoint)).InnerText;
            instance.ApiKey = childs.First(n => n.Name == nameof(ApiKey)).InnerText;
            instance.Enabled = childs.FirstOrDefault(n => n.Name == nameof(Enabled))?.InnerText == "true";
            instance.Instance = childs.First(n => n.Name == nameof(Instance)).InnerText;
            instance.BatchSize = childs.FirstOrDefault(n => n.Name == nameof(BatchSize))?.InnerText != null
                                 && int.TryParse(childs.FirstOrDefault(n => n.Name == nameof(BatchSize))?.InnerText,
                                     out var batchSize)
                ? batchSize
                : 500;
            instance.MaximumQueueSize = childs.FirstOrDefault(n => n.Name == nameof(MaximumQueueSize))?.InnerText != null
                                        && int.TryParse(childs.FirstOrDefault(n => n.Name == nameof(MaximumQueueSize))?.InnerText,
                                            out var maximumQueueSize)
                ? maximumQueueSize
                : 1000000;
            instance.RequestTimeoutMs = childs.FirstOrDefault(n => n.Name == nameof(RequestTimeoutMs))?.InnerText != null
                                        && int.TryParse(childs.FirstOrDefault(n => n.Name == nameof(RequestTimeoutMs))?.InnerText,
                                            out var requestTimeoutMs)
                ? requestTimeoutMs
                : 10000;
            instance.SubmitIntervalMs = childs.FirstOrDefault(n => n.Name == nameof(SubmitIntervalMs))?.InnerText != null
                                        && int.TryParse(childs.FirstOrDefault(n => n.Name == nameof(SubmitIntervalMs))?.InnerText,
                                            out var submitIntervalMs)
                ? submitIntervalMs
                : 3000;
            var nsNodes = childs.FirstOrDefault(n => n.Name == "Namespaces");
            if (nsNodes == null) return instance;
            foreach (XmlNode nsNode in nsNodes)
            {
                instance.AddNamespace(nsNode);
            }
            return instance;
        }

        public static LumberjackConfiguration Current
        {
            get
            {
                if (_current == null)
                {
                    var node = Factory.GetConfigNode("lumberjack/setting");
                    if (node == null) return new LumberjackConfiguration();
                    _current = From(node);
                }
                return _current;
            }
        }
    }

    public class LumberjackNamespaceConfiguration
    {
        public string Namespace { get; set; }
        public int MinimumLevel { get; set; }
        public bool Disabled { get; set; }
        public List<string> ExcludedText { get; set; } = new List<string>();
        public List<string> AlwaysIncludedText { get; set; } = new List<string>();
    }
}
