using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZerochSharp.Models
{
    public class MainContext : DbContext
    {
        public MainContext(DbContextOptions options) : base(options) { }
        public DbSet<Board> Boards { get; set; }
        public DbSet<Thread> Threads { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<Session> Sessions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // modelBuilder.Entity<User>().HasIndex(x => x.UserId).IsUnique(true);

            modelBuilder.Entity<Board>().HasData(new[]
            {
                new Board(){ BoardKey = "news7vip", BoardName = "裏VIP" , Id = 1,BoardDefaultName="以下、名無しにかわりまして裏VIP(´・ω・`)がお送りします" },
                new Board(){ BoardKey = "coffeehouse", BoardName="雑談ルノワール", Id = 2,BoardDefaultName="雑談うんちー" }
            });
        }
        public static DbContextOptionsBuilder InitializeDbBuilder(DbContextOptionsBuilder options,string connectionString, string serverVersion, string serverTypeStr)
        {
            var serverType = (ServerType)Enum.ToObject(typeof(ServerType), int.Parse(serverTypeStr));
            options.UseMySql(connectionString,
                                    mysqlOptions =>
                                    {
                                        mysqlOptions.ServerVersion(new Version(serverVersion), serverType);
                                    });
            return options;
        }
    }
}
