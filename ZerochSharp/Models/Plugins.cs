using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Newtonsoft.Json;
using System.IO;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace ZerochSharp.Models
{
    public class Plugins
    {
        public static Plugins SharedPlugins { get; set; }
        public static bool Runed { get; private set; } = false;
        private const string PLUGIN_SETTING_PATH = "plugins/plugins.json";

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
            var targetPlugin = LoadedPlugins.Where(x => (x.PluginType & types) == types
                                                        && x.IsEnabled
                                                        && x.Valid
                                                        && x.ActivatedBoards.Contains(board.BoardKey))
                .OrderBy(x => x.Priority);
            foreach (var item in targetPlugin)
            {
                
                try
                {
                    
                    await item.Script.RunAsync(new ZerochSharpPlugin(board, thread, response, types, item.BoardSetting?[board.BoardKey] as dynamic));
                }
                catch (CompilationErrorException ex)
                {
                    Console.WriteLine("Error in plugin running.");
                    Console.WriteLine(ex.Message);
                    LoadedPlugins.FirstOrDefault(x => x.PluginPath == item.PluginPath).Valid = false;
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
                var options = ScriptOptions.Default;

                var script = CSharpScript.Create(scriptText, globalsType: typeof(ZerochSharpPlugin), options: options);
                script.Compile();
                item.Script = script;
            });
        }

        public async Task LoadBoardPluginSettings(IEnumerable<string> boardKeys)
        {
            
            foreach (var item in LoadedPlugins)
            {
                if (item.HasBoardSetting)
                {
                    item.BoardSetting = new Dictionary<string, object>();
                    foreach (var board in boardKeys)
                    {
                        var path = GetPluginBoardSettingPath(board, item.PluginPath);

                        if (File.Exists(path))
                        {
                            var file = await File.ReadAllTextAsync(path);
                            var deserialzed = JsonConvert.DeserializeObject(file);
                            item.BoardSetting[board] = (deserialzed as JObject)?.ToObject<IDictionary<string, object>>();
                        }
                        else
                        {
                            item.BoardSetting[board] = (item.DefaultBoardSetting as JObject)?.ToObject<IDictionary<string, object>>();
                        }
                    }
                }
            }
            Runed = true;
        }
        // Board settings will allow only key-value pair and value is not nested item or array.
        // ex.
        // Settings: {
        //     "Name" : "text", // allow
        //     "Users" : [ "Kain", "KOJIMA" ] // allow
        //     "UserDetail" : [ { "name" : "kain", auth: false } ] // disallow
        // }
        public async Task<string> GetBoardPluginSetting(string boardKey, string pluginPath)
        {
            var path = GetPluginBoardSettingPath(boardKey, pluginPath);
            if (!File.Exists(path))
            {
                var plugin = LoadedPlugins.FirstOrDefault(x => x.PluginPath == pluginPath);
                if (plugin == null)
                {
                    throw new InvalidOperationException("Not found plugin");
                }
                else
                {
                    if (plugin.HasBoardSetting)
                    {
                        return JsonConvert.SerializeObject(plugin.DefaultBoardSetting);
                    }
                    else
                    {
                        throw new InvalidOperationException("This plugin cannot has settings.");
                    }
                }
            }
            return await File.ReadAllTextAsync(path);
        }

        public async Task SaveBoardPluginSetting(string boardKey, string pluginPath, JObject settings)
        {
            var plugin = LoadedPlugins.FirstOrDefault(x => x.PluginPath == pluginPath);
            if (plugin == null)
            {
                throw new InvalidOperationException("Not found plugin");
            }
            else if (!plugin.HasBoardSetting)
            {
                throw new InvalidOperationException("This plugin cannot has settings.");
            }
            var path = GetPluginBoardSettingPath(boardKey, pluginPath);
            var saveTask = File.WriteAllTextAsync(path, settings.ToString());
            plugin.BoardSetting[boardKey] = settings.ToObject<object>();
            await saveTask;
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
        public async Task PatchPluginActivatedBoards(string pluginPath, string[] activatedBoards)
        {
            var plugin = LoadedPlugins.FirstOrDefault(x => x.PluginPath == pluginPath);
            plugin.ActivatedBoards = activatedBoards;
            await SavePluginInfo();
        }

        private async Task SavePluginInfo() =>
            await File.WriteAllTextAsync(PLUGIN_SETTING_PATH, JsonConvert.SerializeObject(LoadedPlugins));

        private string GetPluginBoardSettingPath(string boardKey, string pluginPath)
        {
            return $"boards/{boardKey}/plugins_conf/{pluginPath}.json";
        }
    }

}
