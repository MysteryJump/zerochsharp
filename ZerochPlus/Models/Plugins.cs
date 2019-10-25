using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Text.Json;
using System.IO;
using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis.Scripting;

namespace ZerochPlus.Models
{
    public class Plugins
    {
        public static Plugins SharedPlugins { get; set; }
        private IEnumerable<Plugin> plugins;
        private Plugins()
        {

        }
        public static async Task<Plugins> Initialize()
        {
            var jsonText = await File.ReadAllTextAsync("plugins/plugins.json");
            var plugins = JsonSerializer.Deserialize<Plugin[]>(jsonText);
            
            return new Plugins() { plugins = plugins, Count = plugins.Length };
        }
        public async void RunPlugins(PluginTypes types, Board board, Thread thread)
        {
            var targetPlugin = plugins.Where(x => (x.PluginType & types) == types && x.IsEnable).OrderBy(x => x.Priority);
            foreach (var item in targetPlugin)
            {
                
                try
                {
                    await item.Script.RunAsync(new ZerochPlusPlugin(board, thread, types));
                }
                catch (CompilationErrorException)
                {
                    plugins.FirstOrDefault(x => x.PluginPath == item.PluginPath).IsEnable = false;
                    await File.WriteAllTextAsync("plugins/plugins.json", JsonSerializer.Serialize(this));
                }
                catch
                {
                    continue;
                }
            }
        }
        public void PreCompilePlugins()
        {
            Parallel.ForEach(plugins, item =>
            {
                var scriptText = File.ReadAllText("plugins/" + item.PluginPath);
                var script = CSharpScript.Create(scriptText, globalsType: typeof(ZerochPlusPlugin));
                script.Compile();
                item.Script = script;
            });
        }

        public int Count { get; private set; }
    }

    public class Plugin
    {
        public string PluginName { get; set; }
        public PluginTypes PluginType { get; set; }
        [JsonIgnore]
        public string PluginBody { get; set; }
        public string PluginPath { get; set; }
        public int Priority { get; set; }
        public bool IsEnable { get; set; }
        [JsonIgnore]
        public Script<object> Script { get; set; }
        public string PluginDescription { get; set; }
    }
    [Flags]
    public enum PluginTypes
    {
        Response = 1,
        Thread = 1 << 1
    }

    public class ZerochPlusPlugin
    {
        public Board Board { get; private set; }
        public Thread Thread { get; private set; }
        public PluginTypes PluginTypes { get; private set; }
        public ZerochPlusPlugin(Board board, Thread thread, PluginTypes types)
        {
            Board = board;
            Thread = thread;
            PluginTypes = types;
        }
    }
}
