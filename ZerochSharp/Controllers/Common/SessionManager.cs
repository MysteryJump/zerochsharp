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
        private readonly string sessionToken;
        private readonly HttpContext _context;
        private readonly MainContext _dbContext;
        private Session associatedSession;
        public bool Valid { get; private set; }
        public SessionManager(HttpContext context, MainContext dbContext)
        {
            sessionToken = context.Request.Cookies.FirstOrDefault(x => x.Key == "sess").Value;
            if (sessionToken != null)
            {
                var task = dbContext.Sessions.FirstOrDefaultAsync(x => x.SessionToken == sessionToken);
                _context = context;
                _dbContext = dbContext;
                task.Wait();
                Valid = task.Result != null;
                if (Valid)
                {
                    associatedSession = task.Result;
                }
            }
            else
            {
                Valid = false;
            }
        }
        public async Task UpdateSession()
        {
            if (Valid)
            {
                associatedSession.Expired = DateTime.Now + TimeSpan.FromDays(366);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                var sess = new Session(DateTime.Now, "ddede");
                sess.Created = DateTime.Now;
                sess.Expired = DateTime.Now + TimeSpan.FromDays(366);
                await _dbContext.Sessions.AddAsync(sess);
                await _dbContext.SaveChangesAsync();
            }
        }
     }
}
