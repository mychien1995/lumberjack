using System;
using System.Collections.Generic;

#nullable disable

namespace Lumberjack.Server.Entities
{
    public partial class ApiKey
    {
        public Guid Id { get; set; }
        public string KeyValue { get; set; }
        public Guid? ApplicationId { get; set; }
        public bool? IsActive { get; set; }

        public virtual Application Application { get; set; }
    }
}
