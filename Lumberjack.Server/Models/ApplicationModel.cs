using System;
using System.Collections.Generic;

namespace Lumberjack.Server.Models
{
    public class ApplicationModel
    {
        public Guid Id { get; set; }
        public string ApplicationName { get; set; } = string.Empty;
        public string ApplicationCode { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public List<ApplicationInstanceModel> Instances { get; set; } = new();
        public List<ApiKeyModel> ApiKeys { get; set; } = new();
    }
}
