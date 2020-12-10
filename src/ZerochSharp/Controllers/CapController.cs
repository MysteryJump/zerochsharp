using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZerochSharp.Models;
using ZerochSharp.Models.Caps;
using ZerochSharp.Services;

namespace ZerochSharp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CapController : ApiControllerBase
    {
        public CapController(MainContext context, PluginDependency dependency) : base(context, dependency)
        {

        }

        // POST: api/cap/
        [HttpPost]
        public async Task<IActionResult> CreateCap([FromBody] CapRequestQuery query)
        {
            var cap = new Cap
            {
                CapId = query.CapId,
                CapName = query.CapName,
                Description = query.Description,
                PasswordHash = Common.HashPasswordGenerator.GeneratePasswordHash(query.Password, query.CapName.GetHashCode()).hashed
            };
            await Context.Caps.AddAsync(cap);
            await Context.SaveChangesAsync();
            return CreatedAtAction("GetCaps", cap);
        }
        // PUT: api/cap
        [HttpPut]
        public async Task<IActionResult> PutAssociatedCapGroup()
        {
            throw new NotImplementedException();
        }


        // GET: api/cap
        [HttpGet]
        public async Task<IActionResult> GetCaps()
        {
            var caps = await Context.Caps.ToListAsync();
            return Ok(caps);
        }

        // GET: api/cap/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCapById([FromRoute] int id)
        {
            var associatedCapGroup = await Context.CapGroupCaps.Where(x => x.Id == id).ToListAsync();
            var cap = await Context.Caps.FirstOrDefaultAsync(x => x.Id == id);
            return Ok(
            new
            {
                cap.Id,
                cap.CapId,
                cap.Description,
                cap.CapName,
                AssociatedCapGroup = associatedCapGroup
            });
        }

        public class CapRequestQuery
        {
            public string CapName { get; set; }
            public string CapId { get; set; }
            public string Password { get; set; }
            public string Description { get; set; }
        }
    }

}
