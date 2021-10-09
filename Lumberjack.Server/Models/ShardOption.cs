namespace Lumberjack.Server.Models
{
    public class ShardOption
    {
        public long MaximumSize { get; set; }
        public int Buffer { get; set; }
        public int AutoScaleInterval { get; set; }
    }
}
