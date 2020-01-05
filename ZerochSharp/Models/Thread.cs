using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ZerochSharp.Controllers.Common;

namespace ZerochSharp.Models
{
    public class Thread
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ThreadId { get; set; }
        [Required]
        public string Title { get; set; }
        [NotMapped]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Response> Responses { get; set; }
        [Required]
        public DateTime Created { get; set; }
        [Required]
        [ForeignKey("Board")]
        public string BoardKey { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public DateTime Modified { get; set; }

        [Required]
        public int ResponseCount { get; set; }

        [NotMapped]
        public double Influence => Created != DateTime.MinValue ? ResponseCount / ((DateTime.Now - Created).TotalSeconds / 86400.0) : -1;

        public long DatKey { get; set; }
        [NotMapped]
        internal Board AssociatedBoard { get; set; }

        /// <summary>
        /// Initialize for write in Database.
        /// </summary>
        public void Initialize(string ip)
        {
            var time = DateTime.Now;
            Modified = Created = time;
            ResponseCount = 1;
            Author = Models.Author.GenerateAuthorId(ip, BoardKey);
            DatKey = new DateTimeOffset(time, new TimeSpan(+9, 0, 0)).ToUnixTimeSeconds();
        }
        /// <summary>
        /// Get thread from MainContext.
        /// </summary>
        /// <param name="boardKey">BoardKey of Thread.</param>
        /// <param name="threadId">Id of Thread</param>
        /// <param name="context">MainContext</param>
        /// <param name="isAdmin">This request from admin or not</param>
        /// <param name="datKey">This show using threadid as datkey</param>
        /// <returns>Target thread</returns>
        public static async Task<Thread> GetThreadAsync(string boardKey, long threadId, MainContext context, bool isAdmin = false, bool datKey = false)
        {
            var board = await context.Boards.FirstOrDefaultAsync(x => x.BoardKey == boardKey);
            if (board == null)
            {
                return null;
            }

            var aboned = Response.AbonedResponse(board.BoardDeleteName);
            var thread = datKey ? 
                await context.Threads.FirstOrDefaultAsync(x => x.BoardKey == boardKey && x.DatKey == threadId)
                : await context.Threads.FindAsync((int)threadId);
            thread.AssociatedBoard = board;
            if (thread == null || thread.BoardKey != boardKey)
            {
                return null;
            }
            if (datKey)
            {
                threadId = thread.ThreadId;
            }
            thread.Responses = await context.Responses.Where(x => x.ThreadId == (int)threadId).ToListAsync();
            var abonedList = new List<int>();
            var i = 0;
            foreach (var item in thread.Responses)
            {
                if (!isAdmin)
                {
                    item.HostAddress = null;
                }
                if (item.IsAboned)
                {
                    abonedList.Add(i);
                }
                i++;
            }
            foreach (var item in abonedList)
            {
                thread.Responses[item] = aboned;
            }
            return thread;
        }

#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
        public async Task CreateThreadAsync()
#pragma warning restore CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
        {
            throw new NotImplementedException();
        }

        public async Task CreateThread(string boardKey, ClientThread thread, MainContext context, string ip)
        {
            var board = await context.Boards.FirstOrDefaultAsync(x => x.BoardKey == boardKey);
            if (board == null)
            {
                // return BadRequest();
            }
            var body = new Thread
            {
                BoardKey = boardKey,
                Title = thread.Title
            };
            var response = new Response() { Body = thread.Response.Body, Mail = thread.Response.Mail, Name = thread.Response.Name };


            body.Initialize(ip);
            if (Startup.IsUsingLegacyMode)
            {
                if (await context.Threads.AnyAsync(x => x.DatKey == body.DatKey))
                {
                    // return BadRequest();
                }
            }
            var result = context.Threads.Add(body);
            await context.SaveChangesAsync();
            response.Initialize(result.Entity.ThreadId, ip, boardKey);
            Plugins.SharedPlugins.RunPlugins(PluginTypes.Thread, board, body, response);
            context.Responses.Add(response);
            await context.SaveChangesAsync();
        }
    }

    public class ClientThread
    {
        [Required]
        public ClientResponse Response { get; set; }
        [Required]
        public string Title { get; set; }
    }
}
