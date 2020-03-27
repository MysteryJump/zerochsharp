using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ZerochSharp.Models;
using ZerochSharp.Services;

namespace ZerochSharp.Controllers
{
    [Route("/api/global/")]
    public class SiteSettingController : ApiControllerBase
    {
        public SiteSettingController(MainContext context, PluginDependency dependency) : base(context, dependency)
        {
        }
        [HttpGet]
        public async Task<IActionResult> GetSiteSettings()
        {
            var settings = await _context.Setting.FirstOrDefaultAsync();
            return Ok(settings);
        }
        [HttpPatch]
        public async Task<IActionResult> PatchSiteSetting([FromBody] JObject datas)
        {
            if (!await HasSystemAuthority(SystemAuthority.Admin))
            {
                return Unauthorized();
            }
            var setting = await _context.Setting.FirstOrDefaultAsync();
            var settingType = setting.GetType();
            var properties = settingType.GetProperties();
            foreach (var prop in properties)
            {
                if (prop.GetCustomAttributes(typeof(KeyAttribute),false).Length > 0)
                {
                    continue;
                }
                var name = (char)(prop.Name[0] + 32) + prop.Name.Remove(0,1);
                if (datas.ContainsKey(name))
                {
                    prop.SetValue(setting, datas.Value<string>(name));
                }
            }
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
