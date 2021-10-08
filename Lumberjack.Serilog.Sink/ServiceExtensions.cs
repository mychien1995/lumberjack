using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
            var config = app.ApplicationServices.GetService<IOptionsMonitor<LumberjackConfiguration>>();
            if (config == null || !config.CurrentValue.Enabled) return;
            var service = app.ApplicationServices.GetService<ILumberjackLogCollector>();
            sink.Initialize(service);
        }

        public static void UseBuffering(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                try
                {
                    if (context.Request.ContentLength != null && context.Request.ContentLength > 0)
                    {
                        context.Request.EnableBuffering();
                        if (context.Request.Body.CanSeek)
                        {
                            context.Request.Body.Seek(0, SeekOrigin.Begin);
                            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                            {
                                var body = await reader.ReadToEndAsync();
                                context.Items["lb_rq_bd"] = body;
                            }

                            context.Request.Body.Position = 0;
                        }
                    }
                }
                catch (Exception _)
                {
                    // ignored
                }
                finally
                {
                    await next();
                }
            });
        }
    }
}
