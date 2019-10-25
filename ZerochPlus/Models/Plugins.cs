using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Newtonsoft.Json;
using System.IO;
using Microsoft.CodeAnalysis.Scripting;

namespace ZerochPlus.Models
{
    public class Plugins
    {
        public static Plugins SharedPlugins { get; set; }

        private Plugins()
        {

        }
        public static async Task<Plugins> Initialize()
        {
            var jsonText = await File.ReadAllTextAsync("plugins/plugins.json");
            
            var plugins = JsonConvert.DeserializeObject<Plugin[]>(jsonText);
            
            return new Plugins() { LoadedPlugins = plugins, Count = plugins.Length };
        }
        public async void RunPlugins(PluginTypes types, Board board, Thread thread, Response response)
        {
            var targetPlugin = LoadedPlugins.Where(x => (x.PluginType & types) == types && x.IsEnable).OrderBy(x => x.Priority);
            foreach (var item in targetPlugin)
            {
                
                try
                {
                    await item.Script.RunAsync(new ZerochPlusPlugin(board, thread, response, types));
                }
                catch (CompilationErrorException)
                {
                    LoadedPlugins.FirstOrDefault(x => x.PluginPath == item.PluginPath).IsEnable = false;
                    await SavePluginInfo();
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
                var script = CSharpScript.Create(scriptText, globalsType: typeof(ZerochPlusPlugin));
                script.Compile();
                item.Script = script;
            });
        }

        public IEnumerable<Plugin> LoadedPlugins { get; private set; }

        public int Count { get; private set; }
        public async Task PatchPluginEnable(string pluginPath, bool enable)
        {
            var plugin = LoadedPlugins.FirstOrDefault(x => x.PluginPath == pluginPath);
            plugin.IsEnable = enable;
            await SavePluginInfo();
        }

        public async Task PatchPluginPriority(string pluginPath, int priority)
        {
            var plugin = LoadedPlugins.FirstOrDefault(x => x.PluginPath == pluginPath);
            plugin.Priority = priority;
            await SavePluginInfo();
        }

        private async Task SavePluginInfo() => 
            await File.WriteAllTextAsync("plugins/plugins.json", JsonConvert.SerializeObject(this.LoadedPlugins));
    }

}
