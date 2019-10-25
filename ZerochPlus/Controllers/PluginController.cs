using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZerochPlus.Models;

namespace ZerochPlus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PluginController : ControllerBase
    {
        private MainContext _context;
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

        [HttpPatch("{plugin}")]
        public async Task<IActionResult> PatchPluginConfig([FromRoute] string plugin, [FromBody] PluginConfig conf)
        {
            if (!await IsAdminAsync())
            {
                return Unauthorized();
            }
            if (conf.Priority != null)
            {
                await Plugins.SharedPlugins.PatchPluginPriority(plugin,(int)conf.Priority);
            }
            if (conf.IsEnable != null)
            {
                await Plugins.SharedPlugins.PatchPluginEnable(plugin, (bool)conf.IsEnable);
            }
            return Ok();
        }

        private async Task<bool> IsAdminAsync()
        {
            if (HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                var session = new UserSession();
                session.SessionToken = HttpContext.Request.Headers["Authorization"];
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
    }
}
