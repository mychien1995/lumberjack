using Lumberjack.Server.Entities;
using Lumberjack.Server.Models;
using Lumberjack.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nelibur.ObjectMapper;
using TanvirArjel.EFCore.GenericRepository;

namespace Lumberjack.Server.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddTinyMapper(this IServiceCollection collection)
        {
            TinyMapper.Bind<ApplicationModel, Application>(o =>
            {
                o.Ignore(a => a.ApiKeys);
                o.Ignore(a => a.Instances);
            });
            TinyMapper.Bind<Application, ApplicationModel>(o =>
            {
                o.Ignore(a => a.ApiKeys);
                o.Ignore(a => a.ApplicationInstances);
            });
            TinyMapper.Bind<ApiKeyModel, ApiKey>();
            TinyMapper.Bind<ApiKey, ApiKeyModel>();
            TinyMapper.Bind<ApplicationInstanceModel, ApplicationInstance>();
            TinyMapper.Bind<ApplicationInstance, ApplicationInstanceModel>();
            TinyMapper.Bind<ShardModel, Shard>();
            TinyMapper.Bind<Shard, ShardModel>();
            return collection;
        }

        public static IServiceCollection AddDbContext(this IServiceCollection collection, IConfiguration configuration)
        {
            collection.AddEntityFrameworkSqlServer()
                .AddDbContext<CoreDbContext>(o =>
                {
                    o.UseSqlServer(configuration.GetConnectionString("Default"))
                        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                }, ServiceLifetime.Transient, ServiceLifetime.Singleton);
            return collection;
        }

        public static IServiceCollection AddRepository(this IServiceCollection collection)
        {
            collection.AddGenericRepository<CoreDbContext>(ServiceLifetime.Transient);
            return collection;
        }

        public static IServiceCollection AddServices(this IServiceCollection collection)
        {
            collection.AddTransient<IApplicationManager, ApplicationManager>();
            collection.AddSingleton<IEventRegistry, EventRegistry>();
            collection.AddSingleton<IApplicationCache, ApplicationCache>();
            collection.AddSingleton<ILogReceiver, LogReceiver>();
            collection.AddSingleton<ILogWorkerPool, LogWorkerPool>();
            collection.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            collection.AddTransient<ILogQueryHandler, LogQueryHandler>();
            collection.AddSingleton<IShardManager, ShardManager>();
            return collection;
        }

        public static void Initialize(this IApplicationBuilder app)
        {
            var applicationCache = app.ApplicationServices.GetService<IApplicationCache>();
            applicationCache!.Initialize();
            var shardManager = app.ApplicationServices.GetService<IShardManager>();
            shardManager!.Initialize();
            var workerPool = app.ApplicationServices.GetService<ILogWorkerPool>();
            workerPool!.Start();
        }
    }
}
