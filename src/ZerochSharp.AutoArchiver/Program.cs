using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using ZerochSharp.Models;

namespace ZerochSharp.AutoArchiver
{
    class Program
    {
        static void Main(string[] args)
        {
            // for debug
            args = new string[] { "server=localhost;userid=root;password=rootpassword;database=zerochp;", "10.3.9", "1" };
            // end
            string connectionStr = args[0];
            string serverVersion = args[1];
            string serverType = args[2];
            var builder = new DbContextOptionsBuilder();
            MainContext.InitializeDbBuilder(builder, connectionStr, serverVersion, serverType);
            var context = new MainContext(builder.Options);
            var timer = new System.Timers.Timer()
            {
                Interval = 60 * 1000
            };
            foreach (var board in context.Boards)
            {
                AutoRemovingPredicatePool.Predications.Add(board.BoardKey, AutoRemovingPredicate.Build(board));
            }
            Console.WriteLine("All predications loaded");
            var lockObj = new object();
            Console.WriteLine("Starting AutoThread Archiving: Interval 1000milliseconds (1sec)");
            timer.Elapsed += (e, args) =>
            {
                lock (lockObj)
                {
                    timer.Stop();
                    var boards = context.Boards.ToList();
                    foreach (var board in boards)
                    {
                        var threads = context.Threads.Where(x => x.BoardKey == board.BoardKey);
                        var targetThreads = AutoRemovingPredicatePool.Predications[board.BoardKey]
                            .FilterRemoveThread(threads).ToList();
                        foreach (var target in targetThreads)
                        {
                            threads.FirstOrDefault(x => x.ThreadId == target.ThreadId).Archived = true;
                        }
                    }
                    context.SaveChanges();
                    timer.Start();
                }
            };
            timer.Start();
            System.Threading.Thread.Sleep(Timeout.Infinite);
        }
    }
}
