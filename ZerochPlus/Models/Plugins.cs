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
        private IEnumerable<Plugin> plugins;
        private Plugins()
        {

        }
        public static async Task<Plugins> Initialize()
        {
            var jsonText = await File.ReadAllTextAsync("plugins/plugins.json");
            var plugins = JsonSerializer.Deserialize<Plugin[]>(jsonText);
            return new Plugins() { plugins = plugins };
        }
        public async void RunPlugins(PluginTypes types)
        {
            var targetPlugin = plugins.Where(x => (x.PluginType & types) == types && x.Priority > 0).OrderBy(x => x.Priority);
            foreach (var item in targetPlugin)
            {
                var scriptText = await File.ReadAllTextAsync("plugins/" + item.PluginPath);
                var script = CSharpScript.Create(scriptText);
                try
                {
                    await script.RunAsync();
                }
                catch (CompilationErrorException)
                {
                    plugins.FirstOrDefault(x => x.PluginPath == item.PluginPath).Priority = -1;
                    await File.WriteAllTextAsync("plugins/plugins.json", JsonSerializer.Serialize(this));
                }
                catch
                {
                    continue;
                }
            }
        }

    }

    public class Plugin
    {
        public string PluginName { get; set; }
        public PluginTypes PluginType { get; set; }
        [JsonIgnore]
        public string PluginBody { get; set; }
        public string PluginPath { get; set; }
        public int Priority { get; set; }
    }
    [Flags]
    public enum PluginTypes
    {
        Response = 1,
        Thread = 1 << 1
    }
}
