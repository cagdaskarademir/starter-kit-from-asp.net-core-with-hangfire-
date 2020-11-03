using System;
using System.Threading;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StarterKit.Hangfire.Web.Jobs;
using RecurringJob = Hangfire.RecurringJob;

namespace StarterKit.Hangfire.Web
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire(configuration => configuration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseMemoryStorage());

            services.AddTransient<IRecurringJob, CustomRecurringJob>();

            services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IBackgroundJobClient backgroundJobs)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello Hangfire! - Goto https://localhost:5001/hangfire");
                });
            });

            while (true)
            {
                try
                {
                    using var service = app.ApplicationServices.GetService<IRecurringJob>();
                    // Thread.Sleep(3000);
                    RecurringJob.AddOrUpdate("Recurring Random",() => service.GenerateGuid(),Cron.Minutely);

                    return;
                }
                catch (Exception e)
                {
                   Thread.Sleep(3000);
                }
            }
           

        }
    }
}
