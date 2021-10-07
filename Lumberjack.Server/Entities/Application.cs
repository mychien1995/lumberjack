using System;
using System.Collections.Generic;

#nullable disable

namespace Lumberjack.Server.Entities
{
    public partial class Application
    {
        public Application()
        {
            ApiKeys = new HashSet<ApiKey>();
            ApplicationInstances = new HashSet<ApplicationInstance>();
        }

        public Guid Id { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationCode { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<ApiKey> ApiKeys { get; set; }
        public virtual ICollection<ApplicationInstance> ApplicationInstances { get; set; }
    }
}
