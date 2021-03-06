﻿using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace ZerochSharp.Models
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
        [NotMapped]
        public string UserName { get; set; }
        [NotMapped]
        public SystemAuthority SystemAuthority { get; private set; }
        [NotMapped]
        public User User { get; set; }

        private static async Task<UserSession> CheckSession(MainContext context, string sessionToken)
        {
            if (string.IsNullOrWhiteSpace(sessionToken))
            {
                return null;
            }
            var session = await context.UserSessions.FirstOrDefaultAsync(x => x.SessionToken == sessionToken);
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == session.UserId);
            session.User = user;
            session.SystemAuthority = user.SystemAuthority;
            session.UserName = user.UserId;
            if (session == null || session.Expired < DateTime.Now)
            {
                return null;
            }
            else
            {
                return session;
            }
        }

        public async Task<User> GetSessionUserAsync(MainContext context)
        {
            if (string.IsNullOrEmpty(SessionToken))
            {
                throw new InvalidOperationException();
            }
            var session = await CheckSession(context, SessionToken);
            if (session != null)
            {
                try
                {
                    var user = await context.Users.FindAsync(session.UserId);
                    if (user == null)
                    {
                        throw new ApplicationException();
                    }
                    return user;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
