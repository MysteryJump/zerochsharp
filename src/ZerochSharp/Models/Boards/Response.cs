using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ZerochSharp.Services;

namespace ZerochSharp.Models.Boards
{
    public class Response : IResponse
    {
        [Key]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public int Id { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public DateTime Created { get; set; }
        [Required]
        [ForeignKey("Thread")]
        public int ThreadId { get; set; }
        [Required]
        public string Body { get; set; }
        public string Mail { get; set; }
        public string Name { get; set; }
        [Required]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string HostAddress { get; set; }

        public bool IsAboned { get; set; }

        /// <summary>
        /// Initialize for write in database.
        /// </summary>
        /// <param name="threadId">Parent thread ID</param>
        /// <param name="host">Author host address</param>
        /// <param name="boardKey">Board key</param>
        public void Initialize(int threadId, string host, string boardKey)
        {
            HostAddress = host;
            Created = DateTime.Now;
            ThreadId = threadId;
            Author = Models.Author.GenerateAuthorId(host, boardKey);
            IsAboned = false;
        }

        public static readonly Func<string, Response> AbonedResponse = name => new Response
        {
            Author = "",
            Mail = name,
            Name = name,
            Body = name,
            IsAboned = true
        };

    }

    public class ClientResponse : IResponse
    {
        private static readonly object LockObject = new object();
        [Required]
        public string Body { get; set; }
        public string Mail { get; set; }
        public string Name { get; set; }

        public async Task<Response> CreateResponseAsync(string boardKey, long threadId, string hostAddress, MainContext context,
                                        PluginDependency pluginDependency, Session session, bool isLegacy = false)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var board = await context.Boards.FirstOrDefaultAsync(x => x.BoardKey == boardKey);
            if (board == null)
            {
                throw new BBSErrorException(BBSErrorType.BBSNotFoundBoardError);
            }

            await board.InitializeForValidation(context);
            if (board.IsRestricted(hostAddress.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())))
            {
                throw new BBSErrorException(BBSErrorType.BBSRestrictedUserError);
            }
            if (board.HasProhibitedWords(Body))
            {
                throw new BBSErrorException(BBSErrorType.BBSProhibitedWordError);
            }
            var thread = await context.Threads.FirstOrDefaultAsync(x => isLegacy ? x.DatKey == threadId : x.ThreadId == threadId);
            if (thread == null)
            {
                throw new BBSErrorException(BBSErrorType.BBSNotFoundThreadError);
            }
            else if (string.IsNullOrWhiteSpace(Body))
            {
                throw new BBSErrorException(BBSErrorType.BBSNoContentError);
            }
            else if (thread.Stopped)
            {
                throw new BBSErrorException(BBSErrorType.BBSThreadStoppedError);
            }

            await using var transaction = await context.Database.BeginTransactionAsync();
            var response = new Response() { Body = Body, Mail = Mail, Name = Name };
            lock (LockObject)
            {
                response.Initialize(thread.ThreadId, hostAddress, board.BoardKey);
                // ReSharper disable once MethodHasAsyncOverload
                context.Responses.Add(response);
                thread.ResponseCount++;
                thread.Modified = response.Created;
            }
            await pluginDependency.RunPlugin(PluginTypes.Response, response, thread, board, session, context);

            if (!Mail.StartsWith("sage"))
            {
                thread.SageModified = thread.Modified;
            }
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return response;
        }
    }
    public interface IResponse
    {
        [Required]
        public string Body { get; set; }
        public string Mail { get; set; }
        public string Name { get; set; }
    }
}
