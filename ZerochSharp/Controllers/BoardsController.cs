using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ZerochSharp.Controllers.Common;
using ZerochSharp.Models;

namespace ZerochSharp.Controllers
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
        public async Task<IActionResult> GetBoard([FromRoute] string boardKey, [FromQuery] string isConfig)
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
            if (!string.IsNullOrWhiteSpace(isConfig))
            {
                return Ok(board);
            }
            board.Child = await _context.Threads.Where(x => x.BoardKey == boardKey).OrderByDescending(x => x.Modified).ToListAsync();
            return Ok(board);
        }
        // GET: api/Boards/news7vip/localrule
        [HttpGet("{boardKey}/localrule")]
        public async Task<IActionResult> GetBoardLocalrule([FromRoute] string boardKey)
        {
            var board = await _context.Boards.Where(x => x.BoardKey == boardKey).FirstOrDefaultAsync();
            if (board == null)
            {
                return NotFound();
            }
            var lr = board.GetLocalRule();
            if (lr == null)
            {
                return NotFound();
            }
            return Ok(new { Body = board.GetLocalRule() });
        }
        //GET: api/Boards/news7vip/billboard
        [HttpGet("{boardKey}/billboard")]
        public async Task<IActionResult> GetBoardBillBoardPath([FromRoute] string boardKey)
        {
            var board = await _context.Boards.Where(x => x.BoardKey == boardKey).FirstOrDefaultAsync();
            if (board == null)
            {
                return NotFound();
            }
            var billBoardHash = HashGenerator.GenerateSHA512($"{boardKey}_billboard");
            var exceptExt = new string[] { "jpeg", "jpg", "png", "gif", "webp" };
            foreach (var item in exceptExt)
            {
                if (System.IO.File.Exists($"wwwroot/images/{billBoardHash}.{item}".ToLower()))
                {
                    return Ok(new { Path = $"/images/{billBoardHash}.{item}".ToLower() });
                }
            }
            return NotFound();
        }

        [HttpPost("{boardKey}/setting")]
        public async Task<IActionResult> PostBoardSetting([FromRoute] string boardKey, [FromBody] JObject setting)
        {
            if (!await IsAdminAsync())
            {
                return Unauthorized();
            }
            var board = await _context.Boards.FirstOrDefaultAsync(x => x.BoardKey == boardKey);
            var boardType = typeof(Board);
            if (board == null)
            {
                return NotFound();
            }
            foreach (var item in setting)
            {
                var key = item.Key?.Select((c, i) =>
                {
                    if (i == 0)
                    {
                        return (char)(c - 32);
                    }
                    else
                    {
                        return c;
                    }
                });
                var keyStr = new string(key.ToArray());
                if (keyStr == "BoardKey")
                {
                    continue;
                }
                var value = item.Value.ToString();
                var info = boardType.GetProperty(keyStr);
                if (info == null)
                {
                    return BadRequest();
                }
                if (info.PropertyType == typeof(string))
                {
                    info.SetValue(board, value);
                }
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        // GET: api/Boards/news7vip/1 (child id)
        // [Route(@"{boardKey:regex(^(?!.*(\.js|\.css|\.ico|\.txt)).*$)}/")]
        [HttpGet(@"{boardKey}/{threadId:regex(\d+)}/")]
        public async Task<IActionResult> GetThread([FromRoute]string boardKey, [FromRoute] int threadId)
        {
            var isAdmin = await IsAdminAsync();
            var thread = await Thread.GetThreadAsync(boardKey, threadId, _context, isAdmin);
            if (thread == null)
            {
                return NotFound();
            }
            foreach (var item in thread.Responses)
            {
                item.Body = item.Body.Replace("<br>", "\n");
            }
            return Ok(thread);
        }

        // POST: api/Boards/news7vip
        [HttpPost("{boardKey}")]
        public async Task<IActionResult> CreateThread([FromRoute] string boardKey, [FromBody]ClientThread thread)
        {
            var board = await _context.Boards.FirstOrDefaultAsync(x => x.BoardKey == boardKey);
            if (board == null)
            {
                return BadRequest();
            }
            var body = new Thread
            {
                BoardKey = boardKey,
                Title = thread.Title
            };
            var response = new Response() { Body = thread.Response.Body, Mail = thread.Response.Mail, Name = thread.Response.Name };

            var ip = IpManager.GetHostName(HttpContext.Connection);
            body.Initialize(ip);
            if (Startup.IsUsingLegacyMode)
            {
                if (await _context.Threads.AnyAsync(x => x.DatKey == body.DatKey))
                {
                    return BadRequest();
                }
            }
            var result = _context.Threads.Add(body);
            await _context.SaveChangesAsync();
            response.Initialize(result.Entity.ThreadId, ip, boardKey);
            Plugins.SharedPlugins.RunPlugins(PluginTypes.Thread, board, body, response);
            _context.Responses.Add(response);
            var sess = new SessionManager(HttpContext, _context);
            await sess.UpdateSession();
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetThread), new { id = result.Entity.ThreadId }, result.Entity);
        }

        // POST: api/Boards/news7vip/1
        [HttpPost("{boardKey}/{threadId}")]
        public async Task<IActionResult> CreateResponse([FromRoute] string boardKey, [FromRoute] int threadId, [FromBody]ClientResponse body)
        {
            var board = await _context.Boards.FirstOrDefaultAsync(x => x.BoardKey == boardKey);
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
                thread.ResponseCount += 1;
                thread.Modified = response.Created;
            }
            Plugins.SharedPlugins.RunPlugins(PluginTypes.Response, board, thread, response);
            var sess = new SessionManager(HttpContext, _context);
            await sess.UpdateSession();
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetThread), new { id = threadId }, response);
        }

        [HttpDelete("{boardKey}/{threadId}/{responseId}")]
        public async Task<IActionResult> DeleteResponse([FromRoute] string boardKey, [FromRoute] int threadId,
                                                        [FromRoute] int responseId, [FromQuery] bool remove)
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

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{boardKey}/{threadId}/{responseId}")]
        public async Task<IActionResult> EditResponse([FromRoute] string boardKey, [FromRoute] int threadId,
                                                      [FromRoute] int responseId, [FromBody] Response response)
        {
            if (!await IsAdminAsync())
            {
                return Unauthorized();
            }
            _context.Responses.Update(response);
            await _context.SaveChangesAsync();
            return Ok(response);
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

        // DELETE: api/Boards/news7vip
        [HttpDelete("{boardKey}")]
        public async Task<IActionResult> DeleteBoard([FromRoute] string boardKey)
        {
            if (!(await IsAdminAsync()))
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var board = await _context.Boards.FirstOrDefaultAsync(x => x.BoardKey == boardKey);
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
                var session = new UserSession
                {
                    SessionToken = HttpContext.Request.Headers["Authorization"]
                };
                return (((await session.GetSessionUserAsync(_context))?.Authority ?? UserAuthority.Normal) & UserAuthority.Admin)
                    == UserAuthority.Admin;
            }
            return false;
        }

    }
}
