using System;
using System.Collections.Generic;

#nullable disable

namespace Lumberjack.Server.Entities
{
    public partial class ApplicationInstance
    {
        public Guid Id { get; set; }
        public string InstanceName { get; set; }
        public Guid? ApplicationId { get; set; }

        public virtual Application Application { get; set; }
    }
}
