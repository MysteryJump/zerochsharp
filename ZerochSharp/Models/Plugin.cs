using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public string[] ScriptPaths { get; set; }
        public string Author { get; set; }
        public string OfficialSite { get; set; }
        public string[] ActivatedBoards { get; set; }
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
        public ZerochSharpPlugin(Board board, Thread thread, Response response, PluginTypes types)
        {
            Board = board;
            Thread = thread;
            PluginTypes = types;
            Response = response;
        }
    }
}
