using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Compression;
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
                return Ok(PluginDependency.LoadedPlugins);
            }
            return Unauthorized();
        }

        // POST: api/plugin
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddPlugin([FromForm] PluginFormData body)
        {
            if (!await HasSystemAuthority(SystemAuthority.Admin | SystemAuthority.Owner))
            {
                return Unauthorized();
            }
            var stream = body.File.OpenReadStream();
            using var archive = new ZipArchive(stream);
            var entries = archive.Entries;

            await PluginDependency.AddPlugin(entries.ToList());

            return Ok();
        }

        // GET: api/plugin/{plugin}/{boardKey}
        [HttpGet("{plugin}/{boardKey}")]
        public async Task<IActionResult> GetBoardPluginSettings([FromRoute] string plugin, [FromRoute] string boardKey)
        {
            if (await HasSystemAuthority(SystemAuthority.BoardSetting, boardKey))
            {
                return Ok(await PluginDependency.GetBoardPluginSetting(boardKey, plugin));
            }
            return Unauthorized();
        }

        [HttpPost("{plugin}/{boardKey}")]
        public async Task<IActionResult> PostBoardPluginSettings([FromRoute] string plugin, [FromRoute] string boardKey, [FromBody] JObject settings)
        {
            if (await HasSystemAuthority(SystemAuthority.BoardSetting, boardKey))
            {
                await PluginDependency.SaveBoardPluginSetting(boardKey, plugin, settings);
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
                await PluginDependency.PatchPluginSetting(plugin, conf.Priority, PluginDependency.PluginSettingType.Priority);
            }
            if (conf.IsEnable != null)
            {
                await PluginDependency.PatchPluginSetting(plugin, conf.IsEnable, PluginDependency.PluginSettingType.IsEnable);
            }
            if (conf.ActivatedBoards != null)
            {
                await PluginDependency.PatchPluginSetting(plugin, conf.ActivatedBoards ?? new string[0],
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
