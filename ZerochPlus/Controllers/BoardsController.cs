using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ZerochPlus.Controllers.Common;
using ZerochPlus.Models;

namespace ZerochPlus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardsController : ControllerBase
    {
        private readonly MainContext _context;
        public static object lockObject = new object();
        public BoardsController(MainContext context)
        {
            _context = context;
        }

        // GET: api/Boards
        [HttpGet]
        public IEnumerable<Board> GetBoards()
        {

            return _context.Boards;
        }

        // GET: api/Boards/news7vip
        [HttpGet("{boardKey}")]
        public async Task<IActionResult> GetBoard([FromRoute] string boardKey)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var board = await _context.Boards.FirstOrDefaultAsync(x => x.BoardKey == boardKey);

            if (board == null)
            {
                return NotFound();
            }
            board.Child = await _context.Threads.Where(x => x.BoardKey == boardKey).OrderByDescending(x => x.Modified).ToListAsync();
            return Ok(board);
        }
        // GET: api/Boards/news7vip/1 (child id)
        [HttpGet("{boardKey}/{threadId}")]
        public async Task<IActionResult> GetThread([FromRoute]string boardKey, [FromRoute] int threadId)
        {
            var isAdmin = await IsAdminAsync();

            var thread = await _context.Threads.FindAsync(threadId);
            if (thread == null || thread.BoardKey != boardKey)
            {
                return NotFound();
            }

            thread.Responses = await _context.Responses.Where(x => x.ThreadId == threadId).ToListAsync();
            var abonedList = new List<int>();
            var i = 0;
            foreach (var item in thread.Responses)
            {
                item.Body = item.Body.Replace("<br>", "\n");
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
                thread.Responses[item] = Models.Response.AbonedResponse;
            }
            
            
            return Ok(thread);
        }

        // POST: api/Boards/news7vip
        [HttpPost("{boardKey}")]
        public async Task<IActionResult> CreateThread([FromRoute] string boardKey, [FromBody]ClientThread thread)
        {

            var body = new Thread
            {
                BoardKey = boardKey,
                Title = thread.Title
            };
            var response = new Response() { Body = thread.Response.Body, Mail = thread.Response.Mail, Name = thread.Response.Name };

            if (response == null)
            {
                return BadRequest();
            }

            var ip = IpManager.GetHostName(HttpContext.Connection);
            body.Initialize(ip);
            if (Startup.IsUsingLegacyMode)
            {
                if (await _context.Threads.AnyAsync(x => x.DatKey == body.DatKey))
                    return BadRequest();
            }

            var result = _context.Threads.Add(body);
            await _context.SaveChangesAsync();
            response.Initialize(result.Entity.ThreadId, ip, boardKey);
            _context.Responses.Add(response);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetThread), new { id = result.Entity.ThreadId }, result.Entity);
        }

        // POST: api/Boards/news7vip/1
        [HttpPost("{boardKey}/{threadId}")]
        public async Task<IActionResult> CreateResponse([FromRoute] string boardKey, [FromRoute] int threadId, [FromBody]ClientResponse body)
        {
            var thread = await _context.Threads.FirstOrDefaultAsync(x => (x.ThreadId == threadId && x.BoardKey == boardKey));
            if (thread == null)
            {
                return BadRequest();
            }
            var response = new Response() { Name = body.Name, Mail = body.Mail, Body = body.Body };

            lock (lockObject)
            {
                response.Initialize(threadId, IpManager.GetHostName(HttpContext.Connection), boardKey);
                _context.Responses.Add(response);
                thread.ResponseCount += 1; // この辺が非同期の影響をもろにうける 
                thread.Modified = response.Created;
            }
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetThread), new { id = threadId }, response);
        }

        [HttpDelete("{boardKey}/{threadId}/{responseId}")]
        public async Task<IActionResult> DeleteResponse([FromRoute] string boardKey, [FromRoute] int threadId, [FromRoute] int responseId, [FromQuery] bool remove)
        {
            if (!(await IsAdminAsync()))
            {
                return Unauthorized();
            }

            var response = await _context.Responses.FirstOrDefaultAsync(x => x.ThreadId == threadId && x.Id == responseId);
            if (response == null || (await _context.Threads.FindAsync(threadId)).BoardKey != boardKey)
            {
                return BadRequest();
            }
            if (!remove)
            {
                response.IsAboned = true;
            } 
            else
            {
                _context.Responses.Remove(response);
            }
            //response.Body = "あぼーん";
            //response.Author = "";
            //response.Created = DateTime.MinValue;
            //response.Name = "あぼーん";
            //response.Mail = "あぼーん";
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{boardKey}/{threadId}/{responseId}")]
        public async Task<IActionResult> EditResponse([FromRoute] string boardKey, [FromRoute] int threadId, [FromRoute] int responseId,[FromBody] Response response)
        {
            if (!await IsAdminAsync())
            {
                return Unauthorized();
            }
            _context.Responses.Update(response);
            await _context.SaveChangesAsync();
            return Ok(response);
            // throw new NotImplementedException();
        }

        [HttpDelete("{boardKey}/{threadKey}")]
        public async Task<IActionResult> DeleteThread([FromRoute] string boardKey, [FromRoute] int threadId)
        {
            if (!await IsAdminAsync())
            {
                return Unauthorized();
            }
            var thread = await _context.Threads.FirstOrDefaultAsync(x => x.BoardKey == boardKey && x.ThreadId == threadId);
            if (thread == null)
            {
                return NotFound();
            }
            else
            {
                _context.Threads.Remove(thread);
                await _context.SaveChangesAsync();
                return Ok();
            }
            
        }

        #region Unused region

        // PUT: api/Boards/5
        [HttpPut("{id}")]
        public IActionResult PutBoard([FromRoute] int id, [FromBody] Board board)
        {
            return BadRequest();
        }

        // POST: api/Boards
        [HttpPost]
        public async Task<IActionResult> PostBoard([FromBody] Board board)
        {
            if (!(await IsAdminAsync()))
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Boards.Add(board);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBoard", new { id = board.Id }, board);
        }

        // DELETE: api/Boards/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBoard([FromRoute] int id)
        {
            if (!(await IsAdminAsync()))
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var board = await _context.Boards.FindAsync(id);
            if (board == null)
            {
                return NotFound();
            }

            _context.Boards.Remove(board);
            await _context.SaveChangesAsync();

            return Ok(board);
        }
        #endregion

        private bool BoardExists(string boardKey)
        {
            return _context.Boards.Any(e => e.BoardKey == boardKey);
        }
        private async Task<bool> IsAdminAsync()
        {
            if (HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                var session = new UserSession();
                session.SessionToken = HttpContext.Request.Headers["Authorization"];
                return ((await session.GetSessionUserAsync(_context)).Authority & UserAuthority.Admin)
                    == UserAuthority.Admin;
            }
            return false;
        }

    }
}
