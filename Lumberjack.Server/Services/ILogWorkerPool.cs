using System;
using System.Collections.Generic;
using System.Threading;
using Lumberjack.Server.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lumberjack.Server.Services
{
    public interface ILogWorkerPool
    {
        void Dispatch(List<LogEntry> logs);

        void Start();
    }

    public class LogWorkerPool : ILogWorkerPool
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly int _numberOfWorkers;
        private static readonly Random Random = new();
        private readonly Dictionary<int, ILogWorker> _workers = new();
        private readonly ILogger<LogWorkerPool> _logger;
        private readonly Dictionary<int, CancellationTokenSource> _cancellationTokenSources = new();

        public LogWorkerPool(IOptionsMonitor<WorkerOption> workerOption, IServiceProvider serviceProvider, ILogger<LogWorkerPool> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _numberOfWorkers = workerOption.CurrentValue.NumberOfWorkers;
        }

        public void Dispatch(List<LogEntry> logs)
        {
            var selectedWorkerId = Random.Next(1,_numberOfWorkers);
            _workers[selectedWorkerId].Process(logs);
        }

        public void Start()
        {
            _logger.LogInformation("LogWorkerPool Start");
            for (var i = 1; i <= _numberOfWorkers; i++)
            {
                var worker = ActivatorUtilities.CreateInstance<LogWorker>(_serviceProvider, i);
                var cancellationTokenSource = new CancellationTokenSource();
                _workers[i] = worker;
                _cancellationTokenSources[i] = cancellationTokenSource;
                worker.Start(cancellationTokenSource.Token);
            }
        }
    }
}
