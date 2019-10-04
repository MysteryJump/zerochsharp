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
    public class LoginController : ControllerBase
    {
        private readonly MainContext _context;

        public LoginController(MainContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetSession([FromQuery] string userName, [FromQuery]string password)
        {

            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userName);
            if (user == null)
            {
                return BadRequest();
            }
            else if (user.PasswordHash != Common.HashPasswordGenerator.GeneratePasswordHash(password, user.Id))
            {
                return Unauthorized();
            }
            password = null;

            var session = await _context.UserSessions.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (session == null)
            {
                session = new UserSession()
                {
                    CreatedAt = DateTime.Now,
                    UserId = user.Id,
                };
                session.Expired = session.CreatedAt + new TimeSpan(12, 0, 0);
                session.SessionToken = HashGenerator.GenerateSHA512(userName + ":" + new Random().Next(0, 10101019).ToString() + ":" + DateTime.Now.ToString());
                _context.UserSessions.Add(session);
                await _context.SaveChangesAsync();
                return Ok(session);
            }
            else
            {
                return Ok(session);
            }
        }
    }
}