namespace Lumberjack.Server.Models.Common
{
    public class SearchResult<T>
    {
        public SearchResult(long total, T[] data)
        {
            Total = total;
            Data = data;
        }

        public long Total { get; }
        public T[] Data { get; }
    }
}
