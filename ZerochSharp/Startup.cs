using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;
using System.Linq;
using ZerochSharp.Controllers;
using ZerochSharp.Models;
using System.Timers;
using System.Collections.Generic;

namespace ZerochSharp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public static bool IsUsingLegacyMode { get; internal set; }
        public static bool IsUsingCloudflare { get; internal set; }
        public static bool IsDevelopment { get; private set; }

        public static string BBSBaseUrl { get; private set; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(options =>
            {
                options.OutputFormatters.Add(new ShiftJISTextOutputFormatter());
                options.OutputFormatters.Add(new HtmlOutputFormatter());
            }).AddNewtonsoftJson();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ReactClient/build";
            });
            
            services.AddDbContextPool<MainContext>(
                options =>
                {
                    MainContext.InitializeDbBuilder(options, Configuration.GetConnectionString("MainContext"), Configuration.GetConnectionString("ServerVersion"), Configuration.GetConnectionString("ServerType"));
                });
            services.AddDistributedMemoryCache();
            var builder = new DbContextOptionsBuilder();
            MainContext.InitializeDbBuilder(builder, Configuration.GetConnectionString("MainContext"), Configuration.GetConnectionString("ServerVersion"), Configuration.GetConnectionString("ServerType"));
            var dbContext = new MainContext(builder.Options);
            dbContext.Database.Migrate();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            IsDevelopment = env.IsDevelopment();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            IsUsingLegacyMode = Configuration.GetValue<bool>("UseLegacymode");
            BBSBaseUrl = Configuration.GetValue<string>("BBSBaseUrl");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            if (Configuration.GetValue<bool>("RestrictHTTPConnection"))
            {
                app.UseHttpsRedirection();
            }
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ReactClient";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
