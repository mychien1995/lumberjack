using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lumberjack.Serilog.Sink
{
    public static class ServiceExtensions
    {
        public static void AddLumberjack(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddSingleton<ILumberjackLogCollector, LumberjackLogCollector>();
            serviceCollection.Configure<LumberjackConfiguration>(o => configuration.GetSection("Lumberjack").Bind(o));
        }

        public static void UseLumberjack(this IApplicationBuilder app, LumberjackSink sink)
        {
            var service = app.ApplicationServices.GetService<ILumberjackLogCollector>();
            sink.Initialize(service);
        }
    }
}
