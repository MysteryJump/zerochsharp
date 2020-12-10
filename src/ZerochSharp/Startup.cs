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
using ZerochSharp.Services;
using Microsoft.AspNetCore.Diagnostics;
using System.Net.Http;

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
        public static string SiteName { get; set; }
        public static string BBSBaseUrl { get; private set; }
        public static bool HasElasticsearchService { get; private set; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var pluginInitTask = System.Threading.Tasks.Task.Run(PluginDependency.Initialize);
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
            var connectionStr = Configuration.GetConnectionString("MainContext");
            var sqlVer = Configuration.GetConnectionString("ServerVersion");
            var rdbmsType = Configuration.GetConnectionString("ServerType");
            services.AddDbContextPool<MainContext>(
                options =>
                {
                    MainContext.InitializeDbBuilder(options, connectionStr, sqlVer, rdbmsType);
                });
            services.AddSingleton(typeof(PluginDependency));
            services.AddDistributedMemoryCache();
            var builder = new DbContextOptionsBuilder();
            MainContext.InitializeDbBuilder(builder, connectionStr, sqlVer, rdbmsType);
            var dbContext = new MainContext(builder.Options);
            dbContext.Database.Migrate();
            SiteName = dbContext.Setting.First().SiteName;
            HasElasticsearchService = IsAliveElasticsearchService();
            var globalSetting = dbContext.GlobalSettings.FirstOrDefault();
            if (!(globalSetting?.IsInitialized ?? false))
            {
                if (globalSetting == null)
                {
                    globalSetting = new GlobalSetting() { Id = 1, IsInitialized = true, IsInitializedElasticsearchService = false };
                    var userName = Environment.GetEnvironmentVariable("OWNER_USERNAME");
                    var password = Environment.GetEnvironmentVariable("OWNER_PASSWORD");                    
                    Environment.SetEnvironmentVariable("OWNER_PASSWORD", "");

                    if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
                    {
                        var user = new User
                        {
                            UserId = userName
                        };
                        // HACK: It is possible that User table already contains Id:1 .
                        var (hash, salt) = Controllers.Common.HashPasswordGenerator.GeneratePasswordHash(password, 1);
                        user.PasswordHash = hash;
                        user.PasswordSalt = salt;
                        user.SystemAuthority = SystemAuthority.Owner;
                        password = null;
                        dbContext.Users.Add(user);
                    }
                    dbContext.SaveChanges();
                }
            }
            Console.WriteLine(HasElasticsearchService ? "Elasticsearch Service: Running" : "Elasticsearch Service: Not Running");
            pluginInitTask.Wait();
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
            BBSError.InitializeBBSErrors().Wait();

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

        private bool IsAliveElasticsearchService(string path = "localhost")
        {
            var httpClient = new HttpClient();
            try
            {
                var data = httpClient.GetAsync($"http://{path}:9200/");
                data.Wait();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
