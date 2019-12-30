using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ZerochSharp.Models;

namespace ZerochSharp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PluginController : ControllerBase
    {
        private readonly MainContext _context;
        public PluginController(MainContext context)
        {
            _context = context;
        }

        // GET: api/plugin
        [HttpGet]
        public async Task<IActionResult> GetAllPlugins()
        {
            if (await IsAdminAsync())
            {
                return Ok(Plugins.SharedPlugins.LoadedPlugins);
            }
            return Unauthorized();
        }

        // GET: api/plugin/{plugin}/{boardKey}
        public async Task<IActionResult> GetBoardPluginSettings()
        {
            if (await IsAdminAsync())
            {
                return BadRequest();
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
                await Plugins.SharedPlugins.PatchPluginPriority(plugin, (int)conf.Priority);
            }
            if (conf.IsEnable != null)
            {
                await Plugins.SharedPlugins.PatchPluginEnable(plugin, (bool)conf.IsEnable);
            }
            if (conf.ActivatedBoards != null)
            {
                await Plugins.SharedPlugins.PatchPluginActivatedBoards(plugin, conf.ActivatedBoards ?? new string[0]);
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
