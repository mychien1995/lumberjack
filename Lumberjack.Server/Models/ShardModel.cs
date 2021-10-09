using System;

namespace Lumberjack.Server.Models
{
    public class ShardModel
    {
        public Guid ShardId { get; set; }
        public int? Number { get; set; }
        public string TableName { get; set; }
        public long CreatedDate { get; set; }
        public long MaximumSize { get; set; }
        public bool IsCurrent { get; set; }
    }
}
