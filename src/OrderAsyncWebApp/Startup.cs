using Help.Jobs;
using Help.Jobs.Util;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderAsyncWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            #region MediatR
            services.AddMediatR(typeof(Startup));
            #endregion
            services.AddSingleton<TestJob>();
            services.AddLogging()
                        .AddSingleton<IJobFactory, SingletonJobFactory>()
                        .AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton(new JobSchedule(typeof(TestJob), (SimpleScheduleBuilder x) => x.WithIntervalInSeconds(1).RepeatForever(), null, true));
            //services.AddHostedService<QuartzHostedService>();

        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            //DbHelper.CreateConnection(Configuration["ConnectionStrings:DefaultConnection"], Configuration["ConnectionStrings:DevConnection"]);
        }
    }
}
