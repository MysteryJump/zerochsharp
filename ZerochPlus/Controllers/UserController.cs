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
    public class UsersController : ControllerBase // もう一つBaseを
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
        public IActionResult GetUser([FromRoute] string id)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //var user = await _context.Users.FindAsync(id);

            //if (user == null)
            //{
            //    return NotFound();
            //}

            //return Ok(user);
            return BadRequest();
        }

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            if ((await _context.Users.FirstOrDefaultAsync(x => x.UserId == user.UserId)) != null)
            {
                return Conflict();
            }
            
            var password = user.Password;
            user.Password = null;
            _context.Users.Add(user);

            await _context.SaveChangesAsync();
            var (hash, salt) = Common.HashPasswordGenerator.GeneratePasswordHash(password, user.Id);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
            await _context.SaveChangesAsync();
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

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var sessionUser = await GetSessionUserAsync();
            
            if ((sessionUser.Authority & UserAuthority.Admin) == UserAuthority.Admin || user.Id == sessionUser.Id)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(user);
            }
            return BadRequest();

        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        private async Task<bool> IsAdminAsync()
        {

            var session = await GetSessionUserAsync();
            if (session != null &&
                ((session.Authority & UserAuthority.Admin) == UserAuthority.Admin))
            {
                return true;
            }

            return false;
        }
        private async Task<User> GetSessionUserAsync()
        {
            if (HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                var session = new UserSession();
                session.SessionToken = HttpContext.Request.Headers["Authorization"];
                return await session.GetSessionUserAsync(_context);
            }
            return null;
        }
    }
}