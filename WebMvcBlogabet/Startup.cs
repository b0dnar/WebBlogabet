﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebMvcBlogabet.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging;
using WebMvcBlogabet.Logging;

namespace WebMvcBlogabet
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddTransient<BackgroundService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, BackgroundService backgroundService, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            var date = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time")).ToString("yyyy-MM-dd_HH-mm");
            loggerFactory.AddFile($"Logs/myapp-{date}.txt");
            Log.LoggerFactory = loggerFactory;

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "api", template: "api/{controller=Forecast}");
                routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
            });

            Task.Run(async() => { var cts = new CancellationTokenSource();  await backgroundService.StartAsync(cts.Token); }); 
        }
    }
}
