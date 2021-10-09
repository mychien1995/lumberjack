using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using Lumberjack.Server.Entities;
using Lumberjack.Server.Extensions;
using Lumberjack.Server.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TanvirArjel.EFCore.GenericRepository;

namespace Lumberjack.Server.Services
{
    public interface IShardManager
    {
        List<ShardModel> GetShards();

        ShardModel CurrentShard();

        void Initialize();
    }

    public class ShardManager : IShardManager
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private List<ShardModel> _shards = new();
        private ShardModel _currentShard;

        private readonly IRepository _repository;
        private bool _initialized;
        private readonly ShardOption _configuration;
        private readonly ILogger<ShardManager> _logger;
        private static readonly object Lock = new();

        public ShardManager(IRepository repository, IOptionsMonitor<ShardOption> shardOptions,
            IDbConnectionFactory dbConnectionFactory, ILogger<ShardManager> logger)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _logger = logger;
            _repository = repository;
            _configuration = shardOptions.CurrentValue;
            var autosScaleAgent = new Thread(() => AutoScale(true));
            autosScaleAgent.Start();
        }

        public List<ShardModel> GetShards() => _shards;

        public ShardModel CurrentShard() => _currentShard;

        public void Initialize()
        {
            if (_initialized) return;
            lock (Lock)
            {
                if (_initialized) return;
                _logger.LogInformation("Shard Manager started"); 
                AutoScale();
                _initialized = true;
            }
        }

        private void AutoScale(bool isJob = false)
        {
            if (!isJob)
            {
                DoScale();
                return;
            }
            while (true)
            {
                if (!_initialized) continue;
                DoScale(); 
                Thread.Sleep(_configuration.AutoScaleInterval);
            }

            void DoScale()
            {
                _logger.LogInformation("Auto scale in progress");
                InitShards();
                var shards = _repository.GetListAsync<Shard>().Result.Select(s => s.MapTo<ShardModel>()).OrderByDescending(s => s.CreatedDate).ToList();
                var currentShard = shards.First(s => s.IsCurrent);
                Interlocked.Exchange(ref _currentShard, currentShard);
                Interlocked.Exchange(ref _shards, shards);
                _logger.LogInformation($"Auto scale in done. {shards} shards found. Current shard is {_currentShard.TableName}");
            }
        }

        private void InitShards()
        {
            var connection = _dbConnectionFactory.GetConnection();
            try
            {
                var command = new SqlCommand("dbo.AutoScale", connection);
                command.Parameters.Add(new SqlParameter("@currentTime", SqlDbType.BigInt)
                { Value = DateTimeOffset.UtcNow.ToUnixTimeSeconds() });
                command.Parameters.Add(new SqlParameter("@maximumSize", SqlDbType.BigInt)
                { Value = _configuration.MaximumSize });
                command.Parameters.Add(new SqlParameter("@buffer", SqlDbType.BigInt)
                { Value = _configuration.Buffer });
                command.CommandType = CommandType.StoredProcedure;
                command.ExecuteNonQuery();
            }
            finally
            {
                connection.Dispose();
            }
        }
    }
}
