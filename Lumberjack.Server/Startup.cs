using System;
using System.IO;
using Lumberjack.Server.Extensions;
using Lumberjack.Server.Framework.Filters;
using Lumberjack.Server.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace Lumberjack.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(Path.Join(Environment.WebRootPath, $"logs\\logs_{DateTime.Now:ddMMyyyy}.txt"))
                .CreateLogger();
            services.AddControllers(o => o.Filters.Add<LogExceptionFilter>());
            services.AddTinyMapper();
            services.AddDbContext(Configuration).AddRepository().AddServices();
            services.AddScoped<LogExceptionFilter>();
            services.AddCors(o => o.AddPolicy("AllowAll", b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyMethod().AllowAnyOrigin()));
            services.AddMvcCore().AddNewtonsoftJson(o => o.SerializerSettings.ContractResolver =
                new DefaultContractResolver
                {
                    NamingStrategy = null
                }); 
            services.Configure<WorkerOption>(Configuration.GetSection("WorkerOption"));
            services.Configure<ShardOption>(Configuration.GetSection("ShardOption"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowAll");
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Initialize();
        }
    }
}
