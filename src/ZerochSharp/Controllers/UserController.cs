using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZerochSharp.Models;
using ZerochSharp.Services;

namespace ZerochSharp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ApiControllerBase // もう一つBaseを
    {
        public UsersController(MainContext context, PluginDependency dependency) : base(context, dependency)
        {
        }

        // GET: api/Users?isAdmin=true|false
        [HttpGet]
        public async Task<IActionResult> GetAllUser([FromQuery] string isAdmin)
        {
            if (isAdmin == "true")
            {
                if (await HasSystemAuthority(SystemAuthority.Admin))
                {
                    var users = await Context.Users.Where(x => x.SystemAuthority != 0).ToListAsync();
                    return Ok(users);
                }
                else
                {
                    return Unauthorized();
                }

            }
            else
            {
                return BadRequest();
            }

        }

        // GET: api/Users/kain
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] string id)
        {
            var user = await Context.Users.FirstOrDefaultAsync(x => x.UserId == id);
            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return NotFound();
            }
        }
        // GET: api/Users/search?q=ka
        [HttpGet("search")]
        public async Task<IActionResult> SearchUser([FromQuery] string q)
        {
            var users = await Context.Users.Where(x => x.UserId.Contains(q)).ToListAsync();
            return Ok(users);
        }

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if ((await Context.Users.FirstOrDefaultAsync(x => x.UserId == user.UserId)) != null)
            {
                return Conflict();
            }
            if (!user.IsValidUserName())
            {
                return BadRequest("username is not acceptable");
            }
            var password = user.Password;
            user.Password = null;
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            {
                return BadRequest("password needs minimum 8 chatacter");
            }
            Context.Users.Add(user);

            await Context.SaveChangesAsync();
            var (hash, salt) = Common.HashPasswordGenerator.GeneratePasswordHash(password, user.Id);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
            await Context.SaveChangesAsync();
            password = "";
            return Ok(new { user = user.UserId });
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await Context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            await GetSessionUserAsync();

            if (user.Id == CurrentUser.Id || await HasSystemAuthority(SystemAuthority.Admin))
            {
                Context.Users.Remove(user);
                await Context.SaveChangesAsync();

                return Ok(user);
            }
            return BadRequest();

        }
    }
}