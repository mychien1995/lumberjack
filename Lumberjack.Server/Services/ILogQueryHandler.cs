using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Lumberjack.Server.Models;
using Lumberjack.Server.Models.Common;
using Microsoft.Data.SqlClient;

namespace Lumberjack.Server.Services
{
    public interface ILogQueryHandler
    {
        Task<SearchResult<LogEntry>> Query(LogsQuery query);
    }

    public class LogQueryHandler : ILogQueryHandler
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly IShardManager _shardManager;

        public LogQueryHandler(IDbConnectionFactory dbConnectionFactory, IShardManager shardManager)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _shardManager = shardManager;
        }

        public async Task<SearchResult<LogEntry>> Query(LogsQuery query)
        {
            var tableName = !string.IsNullOrEmpty(query.TableName)
                ? query.TableName
                : _shardManager.CurrentShard().TableName;
            var connection = _dbConnectionFactory.GetConnection();
            var command = new SqlCommand("dbo.QueryLogs", connection);
            command.Parameters.Add(new SqlParameter("@TableName", SqlDbType.VarChar, 200) { Value = tableName });
            command.Parameters.Add(new SqlParameter("@Text", SqlDbType.NVarChar, -1) { Value = (object)query.Text ?? DBNull.Value});
            command.Parameters.Add(new SqlParameter("@ApplicationId", SqlDbType.UniqueIdentifier) { Value = (object)query.ApplicationId ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@LogLevel", SqlDbType.Int) { Value = (object)query.LogLevel ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@StartTime", SqlDbType.BigInt) { Value = (object)query.StartTime ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@EndTime", SqlDbType.BigInt) { Value = (object)query.EndTime ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@Skip", SqlDbType.Int) { Value = (object)((query.PageIndex - 1) * query.PageSize) ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@Take", SqlDbType.Int) { Value = (object)query.PageSize ?? DBNull.Value });
            command.CommandType = CommandType.StoredProcedure;
            var reader = await command.ExecuteReaderAsync();
            long total = 0;
            var data = new List<LogEntry>();
            while (reader.Read())
                total = reader.GetInt32(0);
            reader.NextResult();
            while (reader.Read())
            {
                var logEntry = new LogEntry(reader.GetGuid(0), reader.GetDateTime(1), reader.GetInt64(2),
                    (SystemLogLevel)reader.GetInt32(3)
                    , reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7),
                    reader.GetGuid(8),
                    reader.GetString(9));
                data.Add(logEntry);
            }
            return new SearchResult<LogEntry>(total, data.ToArray());
        }
    }
}
