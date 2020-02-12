using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ZerochSharp.Controllers.Common;
using ZerochSharp.Services;

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
        [JsonIgnore]
        public bool Archived { get; set; }
        [JsonIgnore]
        public DateTime SageModified { get; set; }

        /// <summary>
        /// Initialize for write in Database.
        /// </summary>
        public void Initialize(string ip)
        {
            var time = DateTime.Now;
            SageModified = Modified = Created = time;
            ResponseCount = 1;
            Author = Models.Author.GenerateAuthorId(ip, BoardKey);

            DatKey = new DateTimeOffset(DateTime.SpecifyKind(time, DateTimeKind.Unspecified), new TimeSpan(+9, 0, 0)).ToUnixTimeSeconds();
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
        public static async Task<Thread> GetThreadAsync(string boardKey, long threadId, MainContext context,
                                                        bool canViewHostAddress = false, bool datKey = false)
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
                if (!canViewHostAddress)
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
        
        
    }

    public class ClientThread
    {
        [Required]
        public IResponse Response { get; set; }
        [Required]
        public string Title { get; set; }
        public async Task<Thread> CreateThreadAsync(string boardKey, string hostAddress, MainContext context,
                                                    PluginDependency pluginDependency)
        {
            var board = await context.Boards.FirstOrDefaultAsync(x => x.BoardKey == boardKey);
            if (board == null)
            {
                throw new BBSErrorException(BBSErrorType.BBSNotFoundBoardError);
            }
            var thread = new Thread() { BoardKey = boardKey, Title = Title };
            thread.Initialize(hostAddress);
            if (Startup.IsUsingLegacyMode && context.Threads.Any(x => x.BoardKey == boardKey && x.DatKey == thread.DatKey))
            {
                throw new BBSErrorException(BBSErrorType.BBSSameDatKeyError);
            }
            if (string.IsNullOrWhiteSpace(Response.Body))
            {
                throw new BBSErrorException(BBSErrorType.BBSNoContentError);
            }
            if (string.IsNullOrWhiteSpace(Title))
            {
                throw new BBSErrorException(BBSErrorType.BBSNoTitleError);
            }
            var result = context.Threads.Add(thread);
            await context.SaveChangesAsync();
            var response = new Response() { Body = Response.Body, Name = Response.Name, Mail = Response.Mail };
            response.Initialize(result.Entity.ThreadId, hostAddress, boardKey);
            await pluginDependency.RunPlugin(PluginTypes.Thread, response, thread, board, context);
            context.Responses.Add(response);
            await context.SaveChangesAsync();
            thread.Responses = new List<Response>() { response };
            return thread;
        }
    }
}
