using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZerochSharp.Models.Boards;

namespace ZerochSharp.Models
{
    public class Plugin
    {
        public string PluginName { get; set; }
        public PluginTypes PluginType { get; set; }
        [JsonIgnore]
        public string PluginBody { get; set; }
        public string PluginPath { get; set; }
        public int Priority { get; set; }
        public bool IsEnabled { get; set; }
        [JsonIgnore]
        public Script<object> Script { get; set; }
        public string PluginDescription { get; set; }
        public string ScriptPath { get; set; }
        public string Author { get; set; }
        public string OfficialSite { get; set; }
        public string[] ActivatedBoards { get; set; }
        public bool Valid { get; set; }
        public object DefaultBoardSetting { get; set; }
        public bool HasBoardSetting { get; set; }
        public string Version { get; set; }
        [JsonIgnore]
        public Dictionary<string, object> BoardSetting { get; set; }
    }
    [Flags]
    public enum PluginTypes
    {
        Response = 1,
        Thread = 1 << 1
    }

    public class ZerochSharpPlugin
    {
        public Board Board { get; private set; }
        public Thread Thread { get; private set; }
        public Response Response { get; private set; }
        public PluginTypes PluginTypes { get; private set; }
        public dynamic PluginSetting { get; set; }
        public  dynamic SessionData { get; set; }
        public ZerochSharpPlugin(
            Board board, Thread thread, Response response, PluginTypes types, object settings = null,
            object sessionData = null)
        {
            Board = board;
            Thread = thread;
            PluginTypes = types;
            Response = response;
            PluginSetting = settings;
            SessionData = sessionData;
        }
    }
}
