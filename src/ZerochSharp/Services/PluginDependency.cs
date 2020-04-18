using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using ZerochSharp.Models;

namespace ZerochSharp.Services
{
    public class PluginDependency
    {
        private static Plugins sharedPlugins;
        private static bool isInitialized = false;
        private ILogger<PluginDependency> _logger;
        private static bool isFirstRunning = true;
        public PluginDependency(ILogger<PluginDependency> logger)
        {
            _logger = logger;
        }
        public static async Task Initialize()
        {
            if (!Directory.Exists("plugins"))
            {
                Directory.CreateDirectory("plugins");
            }
            if (!File.Exists("plugins/plugins.json"))
            {
                File.AppendAllText("plugins/plugins.json", "[]");
            }
            if (isInitialized)
            {
                throw new InvalidOperationException("cannot reinitialize plugins");
            }

            Console.WriteLine("Plugin: Loading Plugins...");
            sharedPlugins = await Plugins.Initialize();

            Console.WriteLine($"Plugin: {sharedPlugins.Count} Plugins Loaded!");
            Console.WriteLine("Plugin: Precomiling Plugins...");
            sharedPlugins.PreCompilePlugins();
            Console.WriteLine("Plugin: Done!");
            isInitialized = true;
        }

        public async Task AddPlugin(List<ZipArchiveEntry> list)
        {
            var pluginPackageFileArchive = list.FirstOrDefault(x => x.Name == "plugin.json");
            if (pluginPackageFileArchive == null)
            {
                throw new InvalidOperationException("plugin packaged zip needs plugin.json at the top level.");
            }
            var pluginPackageFile = await new StreamReader(pluginPackageFileArchive.Open()).ReadToEndAsync();
            await sharedPlugins.AddPlugin(pluginPackageFile, list.ToList());
        }

        public async Task RunPlugin(PluginTypes pluginTypes, Response response, Thread thread, Board board, MainContext context)
        {
            if (isFirstRunning)
            {
                await sharedPlugins.LoadBoardPluginSettings((await context.Boards.ToListAsync()).Select(x => x.BoardKey));
                isFirstRunning = false;
            }
            sharedPlugins.RunPlugins(pluginTypes, board, thread, response);
        }

        public IEnumerable<Plugin> LoadedPlugins => sharedPlugins.LoadedPlugins;

        public async Task PatchPluginSetting(string pluginPath, object value, PluginSettingType type)
        {
            if (type == PluginSettingType.ActivatedBoards && value is string[] boards)
            {
                await sharedPlugins.PatchPluginActivatedBoards(pluginPath, boards);
            }
            else if (type == PluginSettingType.IsEnable && value is bool isEnable)
            {
                await sharedPlugins.PatchPluginEnable(pluginPath, isEnable);
            }
            else if (type == PluginSettingType.Priority && value is int prior)
            {
                await sharedPlugins.PatchPluginPriority(pluginPath, prior);
            }
        }

        public async Task SaveBoardPluginSetting(string boardKey, string pluginPath, JObject settings)
        {
            await sharedPlugins.SaveBoardPluginSetting(boardKey, pluginPath, settings);
        }
        public async Task<string> GetBoardPluginSetting(string boardKey, string pluginPath) => 
            await sharedPlugins.GetBoardPluginSetting(boardKey, pluginPath);
        public enum PluginSettingType
        {
            Priority,
            IsEnable,
            ActivatedBoards
        }
    }
}
