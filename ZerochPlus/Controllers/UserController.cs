using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZerochPlus.Models;

namespace ZerochPlus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MainContext _context;

        public UsersController(MainContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public IActionResult GetAllUser()
        {
            return BadRequest();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!(await IsAdminAsync()))
            {
                return Unauthorized();
            }
            var password = user.Password;
            _context.Users.Add(user);

            await _context.SaveChangesAsync();
            user.PasswordHash = Common.HashPasswordGenerator.GeneratePasswordHash(password, user.Id);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        private async Task<bool> IsAdminAsync()
        {
            if (HttpContext.Request.Cookies.ContainsKey("sessiontoken"))
            {
                var session = await UserSession.CheckSession(_context, HttpContext.Request.Cookies["sessiontoken"]);
                if (session != null &&
                    ((await _context.Users.FindAsync(session.UserId)).Authority & UserAuthority.Admin) == UserAuthority.Admin)
                {
                    return true;
                }
            }
            return false;
        }
    }
}