using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZerochSharp.Models;

namespace ZerochSharp.Controllers.Common
{
    public class SessionManager
    {
        private readonly HttpContext _context;
        private readonly MainContext _dbContext;
        public bool Valid { get; private set; }
        private readonly CookieOptions _defaultOptions;
        public Session Session { get; }

        public SessionManager(HttpContext context, MainContext dbContext)
        {
            var sessionToken = context.Request.Cookies.FirstOrDefault(x => x.Key == "sess").Value;
            if (sessionToken != null)
            {
                var task = dbContext.Sessions.FirstOrDefaultAsync(x => x.SessionToken == sessionToken);
                task.Wait();
                Valid = task.Result != null;
                if (Valid)
                {
                    Session = task.Result;
                }
            }
            else
            {
                Valid = false;
            }
            _context = context;
            _dbContext = dbContext;
            _defaultOptions = new CookieOptions() { Expires = DateTime.Now + TimeSpan.FromDays(366), HttpOnly = true };
        }
        public async Task UpdateSession()
        {
            if (Valid)
            {
                Session.Expired = DateTime.Now + TimeSpan.FromDays(366);
                await _dbContext.SaveChangesAsync();
                _context.Response.Cookies.Append("sess", Session.SessionToken, _defaultOptions);
            }
            else
            {
                var sess = new Session(DateTime.Now, "ddede");
                sess.Created = DateTime.Now;
                sess.Expired = DateTime.Now + TimeSpan.FromDays(366);
                await _dbContext.Sessions.AddAsync(sess);
                await _dbContext.SaveChangesAsync();
                _context.Response.Cookies.Append("sess", sess.SessionToken, _defaultOptions);
            }
        }
    }
}
