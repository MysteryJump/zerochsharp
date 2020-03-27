using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZerochSharp.Models;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ZerochSharp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MainContext _context;
        public AuthController(MainContext context)
        {
            _context = context;
        }
        // GET: api/auth
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // var sessionName = HttpContext.Session.GetString("user");
            var sessionAuth = HttpContext.Request.Cookies.FirstOrDefault(x => x.Key == "user_sess");
            if (sessionAuth.Value == "none" || sessionAuth.Value == null)
            {
                return NoContent();
            }
            else
            {
                var session = await _context.UserSessions.Where(x => x.SessionToken == sessionAuth.Value).FirstOrDefaultAsync();
                if (session == null)
                {
                    return Unauthorized();
                }
                var user = await _context.Users.FindAsync(session.UserId);
                return Ok(new
                {
                    user.UserId,
                    SetAuthorization = session.SessionToken,
                    user.SystemAuthority,
                    user.ControllableBoards
                });
            }
        }

        // POST: api/auth
        [HttpPost]
        public async Task<IActionResult> PostLogin([FromBody] User cUser)
        {
            if (string.IsNullOrEmpty(cUser.UserId) || string.IsNullOrWhiteSpace(cUser.Password))
            {
                return BadRequest();
            }
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == cUser.UserId);
            if (user == null)
            {
                return NotFound();
            }
            if (!Common.HashPasswordGenerator.VerifyPassword(cUser.Password, user.Id, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest();
            }
            cUser.Password = null;
            var session = new UserSession()
            {
                CreatedAt = DateTime.Now,
                Expired = DateTime.Now + TimeSpan.FromDays(365),
                UserId = user.Id,
                UserName = user.UserId,
                SessionToken = HashGenerator.GenerateSHA512(user.UserId + ":" + new Random().Next(0, 10101019).ToString() + ":" + DateTime.Now.ToString())
            };
            _context.UserSessions.Add(session);
            await _context.SaveChangesAsync();

            // HttpContext.Session.SetString("user", session.SessionToken);
            HttpContext.Response.Cookies.Append("user_sess", session.SessionToken, 
                new CookieOptions() 
                {
                    Expires = DateTimeOffset.Now + TimeSpan.FromDays(366),
                    HttpOnly = true
                });
            return Ok(new { SetAuthorization = session.SessionToken, user.UserId, user.SystemAuthority, user.ControllableBoards });
        }

        // GET: api/auth/logout
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("user_sess");
            return Ok();
        }

    }
}
