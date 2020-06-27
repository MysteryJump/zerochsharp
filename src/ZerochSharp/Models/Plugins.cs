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
using System.Text;
using System.IO.Compression;
using ZerochSharp.Models.Boards;

namespace ZerochSharp.Models
{
    public class Plugins
    {
        private const string PluginSettingPath = "plugins/plugins.json";

        private Plugins()
        { }
        public static async Task<Plugins> Initialize()
        {
            var jsonText = await File.ReadAllTextAsync(PluginSettingPath);

            var plugins = JsonConvert.DeserializeObject<Plugin[]>(jsonText);

            return new Plugins() { LoadedPlugins = plugins, Count = plugins.Length };
        }
        public async void RunPlugins(PluginTypes types, Board board, Thread thread, Response response, Session session)
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
                    await item.Script.RunAsync(new ZerochSharpPlugin(board, thread, response, types,
                        item.BoardSetting?[board.BoardKey] as dynamic, session));
                }
                catch (CompilationErrorException ex)
                {
                    Console.WriteLine("Error in plugin running.");
                    Console.WriteLine(ex.Message);
                    var plugin = LoadedPlugins.FirstOrDefault(x => x.PluginPath == item.PluginPath);
                    if (plugin == null)
                    {
                        throw new InvalidOperationException();
                    }

                    plugin.Valid = false;
                    await SavePluginInfo();
                    Console.WriteLine($"Plugin {item.PluginName} is disabled.");
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException("error in running plugin", e);
                }
            }
        }
        public void PreCompilePlugins()
        {
            Parallel.ForEach(LoadedPlugins, (item) =>
            {
                PreCompilePlugin(item);
            });
        }
        private void PreCompilePlugin(Plugin plugin)
        {
            var scriptText = AnalysisCsxUsingsAndReferences(LoadCsxLines($"plugins/{plugin.PluginPath}", plugin.ScriptPath));
            var options = ScriptOptions.Default;
            var script = CSharpScript.Create(scriptText, globalsType: typeof(ZerochSharpPlugin), options: options);
            script.Compile();
            plugin.Script = script;
        }
        public async Task AddPlugin(string settingFile, List<ZipArchiveEntry> files)
        {
            var plugin = JsonConvert.DeserializeObject<Plugin>(settingFile);
            plugin.Valid = true;
            if (LoadedPlugins.Any())
            {
                plugin.Priority = LoadedPlugins.Max(x => x.Priority) + 1;
            }
            else
            {
                plugin.Priority = 1;
            }
            var loadeds = new List<Plugin>(LoadedPlugins);
            Directory.CreateDirectory($"plugins/{plugin.PluginPath}");
            plugin.IsEnabled = false;
            foreach (var file in files)
            {
                file.ExtractToFile(Path.Combine($"plugins/{plugin.PluginPath}", file.FullName), true);
            }
            PreCompilePlugin(plugin);
            loadeds.Add(plugin);
            LoadedPlugins = loadeds;
            await File.WriteAllTextAsync(PluginSettingPath, JsonConvert.SerializeObject(LoadedPlugins));
        }
        private List<string> LoadCsxLines(string rootPath, string mainName)
        {
            var lines = File.ReadAllLines(rootPath + "/" + mainName);
            var sourceLines = new List<string>();
            foreach (var line in lines)
            {
                if (line.StartsWith("#load"))
                {
                    var newFile = line.Replace("#load", "").Trim().Replace("\"", "");
                    if (newFile == mainName)
                    {
                        throw new InvalidOperationException("this plugin has reccursion reference");
                    }
                    sourceLines.AddRange(LoadCsxLines(rootPath, newFile));
                }
                else
                {
                    sourceLines.Add(line);
                }
            }
            return sourceLines;
        }
        private string AnalysisCsxUsingsAndReferences(List<string> lines)
        {
            var usings = new List<string>();
            var rs = new List<string>();
            var withoutUsings = lines.Aggregate(new List<string>(), (current, next) =>
            {
                if (next.StartsWith("using"))
                {
                    usings.Add(next);
                }
                else if (next.StartsWith("#r"))
                {
                    rs.Add(next);
                }
                else
                {
                    current.Add(next);
                }
                return current;
            });
            var sb = new StringBuilder();

            foreach (var line in rs.Select(x => x.Trim()).Distinct())
            {
                sb.AppendLine(line);
            }
            foreach (var line in usings.Select(x => x.Trim()).Distinct())
            {
                sb.AppendLine(line);
            }
            foreach (var line in withoutUsings)
            {
                sb.AppendLine(line);
            }
            return sb.ToString();

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
            await File.WriteAllTextAsync(PluginSettingPath, JsonConvert.SerializeObject(LoadedPlugins));

        private string GetPluginBoardSettingPath(string boardKey, string pluginPath)
        {
            return $"boards/{boardKey}/plugins_conf/{pluginPath}.json";
        }
    }

}
