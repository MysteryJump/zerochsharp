using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ZerochSharp.Models;

namespace ZerochSharp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (!Directory.Exists("plugins"))
            {
                Directory.CreateDirectory("plugins");
            }
            if (!File.Exists("plugins/plugins.json"))
            {
                File.AppendAllText("plugins/plugins.json", "[]");
            }

            Console.WriteLine("Loading Plugins...");
            var pluginsInitTask = Plugins.Initialize();
            pluginsInitTask.Wait();
            Plugins.SharedPlugins = pluginsInitTask.Result;
            Console.WriteLine($"{Plugins.SharedPlugins.Count} Plugins Loaded!");
            Console.WriteLine("Precomiling Plugins...");
            Plugins.SharedPlugins.PreCompilePlugins();
            Console.WriteLine("Done!");
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://*:5000/")
                .UseStartup<Startup>();
    }
}
