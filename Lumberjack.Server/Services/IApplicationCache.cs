using System.Collections.Concurrent;
using System.Linq;
using Lumberjack.Server.Entities;
using Lumberjack.Server.Extensions;
using Lumberjack.Server.Models;
using Microsoft.Extensions.Logging;
using TanvirArjel.EFCore.GenericRepository;

namespace Lumberjack.Server.Services
{
    public interface IApplicationCache
    {
        ApplicationModel GetApplication(string apiKey);

        void Initialize();
    }

    public class ApplicationCache : IApplicationCache
    {
        private readonly IRepository _repository;
        public readonly ConcurrentDictionary<string, ApplicationModel> ApiKeyToApplicationMap =
            new();
        public ConcurrentDictionary<string, ApplicationModel> Snapshot =
            new();

        private static readonly object Lock = new ();
        private readonly ILogger<ApplicationCache> _logger;


        private bool _isReloading;

        public ApplicationCache(IRepository repository, IEventRegistry eventRegistry, ILogger<ApplicationCache> logger)
        {
            _repository = repository;
            _logger = logger;
            eventRegistry.On(EventChannels.ApplicationChanged, _ => Reload());
        }


        public ApplicationModel GetApplication(string apiKey)
        {
            if (_isReloading) return Snapshot.ContainsKey(apiKey) ? Snapshot[apiKey] : null;
            return ApiKeyToApplicationMap.ContainsKey(apiKey) ? ApiKeyToApplicationMap[apiKey] : null;
        }

        public void Initialize()
        {
            _logger.LogInformation("ApplicationCache Initializing");
            Reload();
            _logger.LogInformation("ApplicationCache Initialized");
        }

        private void Reload()
        {
            lock (Lock)
            {
                _logger.LogInformation("ApplicationCache Reloading");
                _isReloading = true;
                var applications = _repository.GetListAsync<Application>().Result;
                var apiKeys = _repository.GetListAsync<ApiKey>().Result;
                var instances = _repository.GetListAsync<ApplicationInstance>().Result;
                ApiKeyToApplicationMap.Clear();
                foreach (var key in apiKeys)
                {
                    var application = applications.FirstOrDefault(a => a.Id == key.ApplicationId);
                    if(application == null) return;
                    var applicationInstances = instances.Where(i => i.ApplicationId == application.Id).Select(i => i.MapTo<ApplicationInstanceModel>()).ToList();
                    var model = application.MapTo<ApplicationModel>();
                    model.Instances = applicationInstances;
                    ApiKeyToApplicationMap[key.KeyValue] = model;
                }
                Snapshot.Clear();
                Snapshot = new ConcurrentDictionary<string, ApplicationModel>(ApiKeyToApplicationMap.ToDictionary(k => k.Key, v => v.Value));
                _isReloading = false;
                _logger.LogInformation("ApplicationCache Reloaded");
            }
        }
    }
}
