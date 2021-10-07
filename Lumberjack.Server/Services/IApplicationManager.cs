using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Lumberjack.Server.Entities;
using Lumberjack.Server.Extensions;
using Lumberjack.Server.Models;
using Lumberjack.Server.Models.Common;
using Microsoft.EntityFrameworkCore;
using TanvirArjel.EFCore.GenericRepository;

namespace Lumberjack.Server.Services
{
    public interface IApplicationManager
    {
        Task<ApplicationModel> Persist(ApplicationModel application);
        Task<SearchResult<ApplicationModel>> GetAll();
        Task Delete(Guid applicationId);
        Task<ApplicationModel> GetById(Guid applicationId);
    }
    public class ApplicationManager : IApplicationManager
    {
        private readonly IRepository _repository;
        private readonly IEventRegistry _eventRegistry;

        public ApplicationManager(IRepository repository, IEventRegistry eventRegistry)
        {
            _repository = repository;
            _eventRegistry = eventRegistry;
        }

        public async Task<SearchResult<ApplicationModel>> GetAll()
        {
            var data = await _repository.GetQueryable<Application>().OrderBy(a => a.SortOrder)
                .ThenBy(a => a.ApplicationName).ToListAsync();
            return new SearchResult<ApplicationModel>(data.Count,
                data.Select(a => a.MapTo<ApplicationModel>()).ToArray());
        }

        public async Task<ApplicationModel> Persist(ApplicationModel application)
        {
            var transaction = await _repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                var existingApplication = await _repository.GetByIdAsync<Application>(application.Id);
                if (existingApplication == null)
                {
                    application.Id = Guid.NewGuid();
                    await _repository.InsertAsync(application.MapTo<Application>());
                }
                else await _repository.UpdateAsync(application.MapTo<Application>());
                var existingInstances =
                   await _repository.GetListAsync<ApplicationInstance>(c => c.ApplicationId == application.Id);
                var existingApiKeys = await _repository.GetListAsync<ApiKey>(c => c.ApplicationId == application.Id);
                await MergeList(application.Instances, existingInstances, (e, m) => e.InstanceName == m.InstanceName, e => e.ApplicationId = application.Id, (e1, e2) => e1.Id = e2.Id);
                await MergeList(application.ApiKeys, existingApiKeys, (e, m) => e.KeyValue == m.KeyValue, e => e.ApplicationId = application.Id, (e1, e2) => e1.Id = e2.Id);
                await transaction.CommitAsync();
                _eventRegistry.Emit(EventChannels.ApplicationChanged, application);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            return application;
        }

        public async Task Delete(Guid applicationId)
        {
            await _repository.DeleteAsync((await _repository.GetListAsync<ApplicationInstance>(a => a.ApplicationId == applicationId)).AsEnumerable());
            await _repository.DeleteAsync(
                (await _repository.GetListAsync<ApiKey>(a => a.ApplicationId == applicationId)).AsEnumerable());
            var application = await _repository.GetByIdAsync<Application>(applicationId);
            if (application != null) await _repository.DeleteAsync(application);
        }

        public async Task<ApplicationModel> GetById(Guid applicationId)
        {
            var application = await _repository.GetByIdAsync<Application>(applicationId);
            if (application == null) throw new ArgumentException("Application Not Found");
            var apiKeys = await _repository.GetListAsync<ApiKey>(a => a.ApplicationId == applicationId);
            var instances = await _repository.GetListAsync<ApplicationInstance>(a => a.ApplicationId == applicationId);
            var applicationModel = application.MapTo<ApplicationModel>();
            applicationModel.ApiKeys = apiKeys.Select(a => a.MapTo<ApiKeyModel>()).ToList();
            applicationModel.Instances = instances.Select(a => a.MapTo<ApplicationInstanceModel>()).ToList();
            return applicationModel;
        }

        private async Task MergeList<TEntity, TModel>(List<TModel> models, List<TEntity> entities,
            Func<TEntity, TModel, bool> comparer, Action<TEntity> entityModifier, Action<TEntity, TEntity> assignId) where TEntity : class
        {
            foreach (var model in models.Where(i => entities.Any(e => comparer(e, i))))
            {
                var entity = model.MapTo<TEntity>();
                var matchingEntity = entities.First(e => comparer(e, model));
                assignId(entity, matchingEntity);
                entityModifier(entity);
                await _repository.UpdateAsync(entity);
            }
            foreach (var entity in entities.Where(i => models.All(e => !comparer(i, e))))
                await _repository.DeleteAsync(entity);
            foreach (var model in models.Where(i =>
                entities.All(e => !comparer(e, i))).ToArray())
            {
                var entity = model.MapTo<TEntity>();
                entityModifier(entity);
                await _repository.InsertAsync(entity);
            }
        }
    }
}
