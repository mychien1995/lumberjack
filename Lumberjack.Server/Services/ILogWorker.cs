using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using Lumberjack.Server.Entities;
using Lumberjack.Server.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lumberjack.Server.Services
{
    public interface ILogWorker
    {
        int WorkerId { get; }

        void Process(List<LogEntry> logs);

        void Start(CancellationToken token);
    }

    public class LogWorker : ILogWorker
    {
        private readonly Thread _dedicatedThread;
        private readonly ILogger<LogWorker> _logger;
        private readonly WorkerOption _workerOption;
        private readonly ConcurrentQueue<LogEntry> _queue = new();
        private readonly string _connectionString;
        public LogWorker(ILogger<LogWorker> logger, IOptionsMonitor<WorkerOption> workerOptions, IConfiguration configuration, int workerId)
        {
            WorkerId = workerId;
            _logger = logger;
            _connectionString = configuration.GetConnectionString("Default");
            _dedicatedThread = new Thread(SaveData)
            {
                Name = $"log-worker-{workerId}"
            };
            _workerOption = workerOptions.CurrentValue;
        }

        public int WorkerId { get; }

        public void Process(List<LogEntry> logs)
        {
            foreach (var log in logs)
                _queue.Enqueue(log);
        }

        public void Start(CancellationToken token)
        {
            _dedicatedThread.Start();
            _logger.LogInformation($"Worker {WorkerId} started");
        }

        private void SaveData()
        {
            while (true)
            {
                if (!_queue.IsEmpty)
                {
                    var batch = new List<LogEntry>();
                    while (batch.Count < _workerOption.MaximumBatchSize && !_queue.IsEmpty)
                    {
                        if (_queue.TryDequeue(out var entry))
                            batch.Add(entry);
                    }

                    if (batch.Any())
                    {
                        try
                        {
                            using var connection = new SqlConnection(_connectionString);
                            var commandText = "INSERT INTO  dbo.LogDatas SELECT * FROM @logs";
                            var command = new SqlCommand(commandText, connection);
                            var param = command.Parameters.AddWithValue("@logs", ToDataTable(batch));
                            param.TypeName = "dbo.LogDatasType";
                            if (connection.State != ConnectionState.Open)
                                connection.Open();
                            command.ExecuteNonQuery();
                            connection.Dispose();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Worker {WorkerId} - error on executing insert");
                        }
                    }
                }
                Thread.Sleep(_workerOption.ProcessingInterval);
            }
        }

        private static DataTable ToDataTable(List<LogEntry> logs)
        {
            var table = new DataTable();
            table.Columns.Add(nameof(LogData.Id), typeof(Guid));
            table.Columns.Add(nameof(LogData.CreateDate), typeof(DateTime));
            table.Columns.Add(nameof(LogData.Timestamp), typeof(long));
            table.Columns.Add(nameof(LogData.LogLevel), typeof(int));
            table.Columns.Add(nameof(LogData.Namespace), typeof(string));
            table.Columns.Add(nameof(LogData.Message), typeof(string));
            table.Columns.Add(nameof(LogData.Request), typeof(string));
            table.Columns.Add(nameof(LogData.RequestContext), typeof(string));
            table.Columns.Add(nameof(LogData.ApplicationId), typeof(Guid));
            table.Columns.Add(nameof(LogData.Instance), typeof(string));
            foreach (var log in logs)
            {
                table.Rows.Add(log.Id, log.CreateDate, log.Timestamp, (int)log.LogLevel, log.Namespace, log.Message,
                    log.Request, log.RequestContext, log.ApplicationId, log.Instance);
            }

            return table;
        }
    }
}
