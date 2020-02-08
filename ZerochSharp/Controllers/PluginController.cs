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
    public class PluginController : ControllerBase
    {
        private readonly MainContext _context;
        private readonly PluginDependency pluginDependency;
        public PluginController(MainContext context, PluginDependency plugin)
        {
            pluginDependency = plugin;
            _context = context;
        }

        // GET: api/plugin
        [HttpGet]
        public async Task<IActionResult> GetAllPlugins()
        {
            if (await IsAdminAsync())
            {
                return Ok(pluginDependency.LoadedPlugins);
            }
            return Unauthorized();
        }

        // GET: api/plugin/{plugin}/{boardKey}
        [HttpGet("{plugin}/{boardKey}")]
        public async Task<IActionResult> GetBoardPluginSettings([FromRoute] string plugin, [FromRoute] string boardKey)
        {
            if (await IsAdminAsync())
            {
                return Ok(await pluginDependency.GetBoardPluginSetting(boardKey, plugin));
                //return Ok(await pluginDependency.GetBoardPluginSetting(boardKey, plugin));
            }
            return Unauthorized();
        }

        [HttpPost("{plugin}/{boardKey}")]
        public async Task<IActionResult> PostBoardPluginSettings([FromRoute] string plugin, [FromRoute] string boardKey, [FromBody] JObject settings)
        {
            if (await IsAdminAsync())
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
            if (!await IsAdminAsync())
            {
                return Unauthorized();
            }
            if (conf.Priority != null)
            {
                //await Plugins.SharedPlugins.PatchPluginPriority(plugin, (int)conf.Priority);
                await pluginDependency.PatchPluginSetting(plugin, conf.Priority, PluginDependency.PluginSettingType.Priority);
            }
            if (conf.IsEnable != null)
            {
                //await Plugins.SharedPlugins.PatchPluginEnable(plugin, (bool)conf.IsEnable);
                await pluginDependency.PatchPluginSetting(plugin, conf.IsEnable, PluginDependency.PluginSettingType.IsEnable);
            }
            if (conf.ActivatedBoards != null)
            {
                //await Plugins.SharedPlugins.PatchPluginActivatedBoards(plugin, conf.ActivatedBoards ?? new string[0]);
                await pluginDependency.PatchPluginSetting(plugin, conf.ActivatedBoards ?? new string[0], PluginDependency.PluginSettingType.ActivatedBoards);
            }

            return Ok();
        }

        private async Task<bool> IsAdminAsync()
        {
            if (HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                var session = new UserSession
                {
                    SessionToken = HttpContext.Request.Headers["Authorization"]
                };
                return (((await session.GetSessionUserAsync(_context))?.Authority ?? UserAuthority.Normal) & UserAuthority.Admin)
                    == UserAuthority.Admin;
            }
            return false;
        }
    }
    public class PluginConfig
    {
        public bool? IsEnable { get; set; }
        public int? Priority { get; set; }
        public string[] ActivatedBoards { get; set; }
    }
}
