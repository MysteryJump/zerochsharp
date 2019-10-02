using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ZerochPlus.Models
{
    public class UserSession
    {
        public string SessionToken { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime Expired { get; set; }
        [JsonIgnore]
        public int UserId { get; set; }
        [JsonIgnore]
        [Key]
        public long Id { get; set; }

        public static async Task<UserSession> CheckSession(MainContext context, string sessionToken)
        {
            if (string.IsNullOrWhiteSpace(sessionToken))
            {
                return null;
            }
            var session = await context.UserSessions.FirstOrDefaultAsync(x => x.SessionToken == sessionToken);
            if (session == null || session.Expired < DateTime.Now)
            {
                return null;
            }
            else
            {
                return session;
            }
        }
    }
}
