using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZerochSharp.Models;

namespace ZerochSharp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly MainContext _context;

        public LoginController(MainContext context)
        {
            _context = context;
        }

        // POST: api/login
        [HttpPost]
        public async Task<IActionResult> GetSession([FromBody] User users)
        {

            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == users.UserId);
            if (user == null)
            {
                return NotFound();
            }
            else if (!Common.HashPasswordGenerator.VerifyPassword(users.Password, user.Id, user.PasswordHash, user.PasswordSalt))
            {
                return Unauthorized();
            }
            users.Password = null;

            var session = await _context.UserSessions.FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (session == null || session.Expired < DateTime.Now)
            {
                session = new UserSession()
                {
                    CreatedAt = DateTime.Now,
                    UserId = user.Id,
                };
                var storedUser = (await _context.Users.FirstOrDefaultAsync(x => x.Id == session.UserId));
                session.UserName = storedUser.UserId;
                session.Authority = storedUser.Authority;
                session.Expired = session.CreatedAt + new TimeSpan(365,0, 0, 0);
                session.SessionToken = HashGenerator.GenerateSHA512(user.UserId + ":" + new Random().Next(0, 10101019).ToString() + ":" + DateTime.Now.ToString());
                _context.UserSessions.Add(session);
                await _context.SaveChangesAsync();
                return Ok(session);
            }
            else
            {
                session.UserName = user.UserId; 
                return Ok(session);
            }
        }
        // GET: api/login?session=sessionToken
        [HttpGet]
        public async Task<IActionResult> CheckSession([FromQuery]string session)
        {
            var sess = await _context.UserSessions.FirstOrDefaultAsync(x => x.SessionToken == session);
            if (sess != null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == sess.UserId);
                var reuser = new
                {
                    UserName = user.UserId,
                    user.Authority
                };
                return Ok(reuser);
            }
            else
            {
                return NotFound();
            }
        }
    }
}