using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ZerochSharp.Models;
using Newtonsoft.Json.Linq;
using ZerochSharp.Services;

namespace ZerochSharp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PluginController : ApiControllerBase
    {
        public PluginController(MainContext context, PluginDependency plugin) : base(context, plugin)
        { }

        // GET: api/plugin
        [HttpGet]
        public async Task<IActionResult> GetAllPlugins()
        {
            if (await HasSystemAuthority(SystemAuthority.Admin))
            {
                return Ok(pluginDependency.LoadedPlugins);
            }
            return Unauthorized();
        }

        // GET: api/plugin/{plugin}/{boardKey}
        [HttpGet("{plugin}/{boardKey}")]
        public async Task<IActionResult> GetBoardPluginSettings([FromRoute] string plugin, [FromRoute] string boardKey)
        {
            if (await HasSystemAuthority(SystemAuthority.BoardSetting, boardKey))
            {
                return Ok(await pluginDependency.GetBoardPluginSetting(boardKey, plugin));
            }
            return Unauthorized();
        }

        [HttpPost("{plugin}/{boardKey}")]
        public async Task<IActionResult> PostBoardPluginSettings([FromRoute] string plugin, [FromRoute] string boardKey, [FromBody] JObject settings)
        {
            if (await HasSystemAuthority(SystemAuthority.BoardSetting, boardKey))
            {
                await pluginDependency.SaveBoardPluginSetting(boardKey, plugin, settings);
                return Ok();
            }
            return Unauthorized();
        }

        // PATCH: api/plugin/{plugin}
        [HttpPatch("{plugin}")]
        public async Task<IActionResult> PatchPluginConfig([FromRoute] string plugin, [FromBody] PluginConfig conf)
        {
            if (!await HasSystemAuthority(SystemAuthority.Admin))
            {
                return Unauthorized();
            }
            if (conf.Priority != null)
            {
                await pluginDependency.PatchPluginSetting(plugin, conf.Priority, PluginDependency.PluginSettingType.Priority);
            }
            if (conf.IsEnable != null)
            {
                await pluginDependency.PatchPluginSetting(plugin, conf.IsEnable, PluginDependency.PluginSettingType.IsEnable);
            }
            if (conf.ActivatedBoards != null)
            {
                await pluginDependency.PatchPluginSetting(plugin, conf.ActivatedBoards ?? new string[0],
                                                          PluginDependency.PluginSettingType.ActivatedBoards);
            }
            return Ok();
        }
    }
    public class PluginConfig
    {
        public bool? IsEnable { get; set; }
        public int? Priority { get; set; }
        public string[] ActivatedBoards { get; set; }
    }
}
