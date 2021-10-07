namespace Lumberjack.Server.Models
{
    public class WorkerOption
    {
        public int NumberOfWorkers { get; set; }
        public int ProcessingInterval { get; set; }
        public int MaximumBatchSize { get; set; }
    }
}
