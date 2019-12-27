using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Newtonsoft.Json;
using System.IO;
using Microsoft.CodeAnalysis.Scripting;

namespace ZerochSharp.Models
{
    public class Plugins
    {
        public static Plugins SharedPlugins { get; set; }
        private const string PLUGIN_SETTING_PATH = "plugins/plugins.json";
        private const string PLUGIN_BOARD_SETTINGS_FOLDER_PATH = "plugins/boardsettings";

        private Plugins()
        { }
        public static async Task<Plugins> Initialize()
        {
            var jsonText = await File.ReadAllTextAsync(PLUGIN_SETTING_PATH);

            var plugins = JsonConvert.DeserializeObject<Plugin[]>(jsonText);

            return new Plugins() { LoadedPlugins = plugins, Count = plugins.Length };
        }
        public async void RunPlugins(PluginTypes types, Board board, Thread thread, Response response)
        {
            var targetPlugin = LoadedPlugins.Where(x => (x.PluginType & types) == types && x.IsEnabled && x.ActivatedBoards.Contains(board.BoardKey))
                .OrderBy(x => x.Priority);
            foreach (var item in targetPlugin)
            {
                try
                {
                    await item.Script.RunAsync(new ZerochSharpPlugin(board, thread, response, types));
                }
                catch (CompilationErrorException ex)
                {
                    Console.WriteLine("Error in plugin running.");
                    Console.WriteLine(ex.Message);
                    LoadedPlugins.FirstOrDefault(x => x.PluginPath == item.PluginPath).IsEnabled = false;
                    await SavePluginInfo();
                    Console.WriteLine($"Plugin {item.PluginName} is disabled.");
                }
                catch
                {
                    continue;
                }
            }
        }
        public void PreCompilePlugins()
        {
            Parallel.ForEach(LoadedPlugins, item =>
            {
                var scriptText = "";

                foreach (var path in item.ScriptPaths ?? new[] { "main.cs" })
                {
                    scriptText += File.ReadAllText($"plugins/{item.PluginPath}/{path}");
                }
                var script = CSharpScript.Create(scriptText, globalsType: typeof(ZerochSharpPlugin));
                script.Compile();
                item.Script = script;
            });
        }
        // Board settings will allow only key-value pair and value is not nested item or array.
        // ex.
        // Settings: {
        //     "Name" : "text", // allow
        //     "Users" : [ "Kain", "KOJIMA" ] // allow
        //     "UserDetail" : [ { "name" : "kain", auth: false } ] // disallow
        // }
        public async Task GetBoardPluginSetting(string boardKey)
        {
            var file = await File.ReadAllTextAsync($"{PLUGIN_BOARD_SETTINGS_FOLDER_PATH}/{boardKey}.json");
            
        }

        public IEnumerable<Plugin> LoadedPlugins { get; private set; }

        public int Count { get; private set; }
        public async Task PatchPluginEnable(string pluginPath, bool enable)
        {
            var plugin = LoadedPlugins.FirstOrDefault(x => x.PluginPath == pluginPath);
            plugin.IsEnabled = enable;
            await SavePluginInfo();
        }

        public async Task PatchPluginPriority(string pluginPath, int priority)
        {
            var plugin = LoadedPlugins.FirstOrDefault(x => x.PluginPath == pluginPath);
            plugin.Priority = priority;
            await SavePluginInfo();
        }

        private async Task SavePluginInfo() =>
            await File.WriteAllTextAsync(PLUGIN_SETTING_PATH, JsonConvert.SerializeObject(LoadedPlugins));
    }

}
