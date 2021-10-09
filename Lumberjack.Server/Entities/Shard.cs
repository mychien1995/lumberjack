using System;
using System.Collections.Generic;

#nullable disable

namespace Lumberjack.Server.Entities
{
    public partial class Shard
    {
        public Guid ShardId { get; set; }
        public int? Number { get; set; }
        public string TableName { get; set; }
        public long? CreatedDate { get; set; }
        public long? MaximumSize { get; set; }
        public bool IsCurrent { get; set; }
    }
}
