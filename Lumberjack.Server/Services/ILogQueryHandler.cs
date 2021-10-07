using System;
using System.Linq;
using System.Threading.Tasks;
using Lumberjack.Server.Entities;
using Lumberjack.Server.Models;
using Lumberjack.Server.Models.Common;
using Microsoft.EntityFrameworkCore;
using TanvirArjel.EFCore.GenericRepository;

namespace Lumberjack.Server.Services
{
    public interface ILogQueryHandler
    {
        Task<SearchResult<LogEntry>> Query(LogsQuery query);
    }

    public class LogQueryHandler : ILogQueryHandler
    {
        private readonly IRepository _repository;

        public LogQueryHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<SearchResult<LogEntry>> Query(LogsQuery query)
        {
            var baseQuery = _repository.GetQueryable<LogData>().AsNoTracking();
            if (!string.IsNullOrEmpty(query.Text))
            {
                var text = query.Text.Trim();
                baseQuery = baseQuery.Where(l => l.Message.Contains(text));
            }
            if (query.ApplicationId.HasValue)
                baseQuery = baseQuery.Where(l => l.ApplicationId == query.ApplicationId);
            if (query.LogLevel.HasValue)
                baseQuery = baseQuery.Where(l => l.LogLevel == query.LogLevel);
            if (!string.IsNullOrEmpty(query.Instance))
                baseQuery = baseQuery.Where(l => l.Instance == query.Instance);
            if (query.StartTime.HasValue)
                baseQuery = baseQuery.Where(l => l.Timestamp >= query.StartTime);
            if (query.EndTime.HasValue)
                baseQuery = baseQuery.Where(l => l.Timestamp <= query.EndTime);
            var total = await baseQuery.CountAsync();
            baseQuery = baseQuery.OrderByDescending(q => q.Timestamp).Skip((query.PageIndex - 1) * query.PageSize).Take(query.PageSize);
            var data = (await baseQuery.ToListAsync()).Select(l => new LogEntry(l.Id, l.CreateDate, l.Timestamp!.Value,
                (SystemLogLevel)l.LogLevel, l.Namespace,
                l.Message, l.Request, l.RequestContext, l.ApplicationId ?? Guid.Empty, l.Instance)).ToArray();
            return new SearchResult<LogEntry>(total, data);
        }
    }
}
