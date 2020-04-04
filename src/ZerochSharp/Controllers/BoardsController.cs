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
using ZerochSharp.Services;

namespace ZerochSharp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardsController : ApiControllerBase
    {
        public static object lockObject = new object();

        public BoardsController(MainContext context, PluginDependency dependency) : base(context, dependency)
        {
        }

        // GET: api/Boards
        [HttpGet]
        public async Task<IEnumerable<Board>> GetBoards()
        {
            var isAdmin = await HasSystemAuthority(SystemAuthority.Admin);
            return _context.Boards.Select(new Func<Board, Board>((x) =>
            {
                if (!isAdmin)
                {
                    x.AutoRemovingPredicate = null;
                }
                return x;
            }));
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

            board.Children = await _context.Threads.Where(x => x.BoardKey == boardKey && !x.Archived)
                                                .OrderByDescending(x => x.SageModified)
                                                .ToListAsync();
            if (!await HasSystemAuthority(SystemAuthority.Admin
                    | SystemAuthority.BoardsManagement
                    | SystemAuthority.BoardSetting, boardKey))
            {
                board.AutoRemovingPredicate = null;
            }

            return Ok(board);
        }

        // GET: api/Boards/news7vip/archives?page=1
        [HttpGet("{boardKey}/archives")]
        public async Task<IActionResult> GetBoardArchives([FromRoute] string boardKey, [FromQuery] int page)
        {
            var board = await _context.Boards.FirstOrDefaultAsync(x => x.BoardKey == boardKey);
            if (page <= 0)
            {
                return BadRequest();
            }
            if (board == null)
            {
                return NotFound();
            }
            var archivedThreads = _context.Threads.Where(x => x.BoardKey == boardKey && x.Archived);

            var selectedArchivedThreads = await archivedThreads.OrderByDescending(x => x.Modified)
                                                               .Skip((page - 1) * 20)
                                                               .Take(20)
                                                               .ToListAsync();
            var count = await archivedThreads.CountAsync();
            if (!await HasSystemAuthority(SystemAuthority.Admin
                    | SystemAuthority.BoardsManagement
                    | SystemAuthority.BoardSetting, boardKey))
            {
                board.AutoRemovingPredicate = null;
            }
            board.Children = selectedArchivedThreads;
            board.Page = page;
            board.IsArchivedChild = true;
            board.ChildrenCount = count;
            return Ok(board);
        }

        // POST: api/Boards
        [HttpPost]
        public async Task<IActionResult> PostBoard([FromBody] Board board)
        {
            if (!await HasSystemAuthority(SystemAuthority.BoardsManagement))
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
            if (!await HasSystemAuthority(SystemAuthority.BoardsManagement))
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

        // GET: api/Boards/news7vip/localrule
        [HttpGet("{boardKey}/localrule")]
        public async Task<IActionResult> GetBoardLocalrule([FromRoute] string boardKey)
        {
            var board = await _context.Boards.Where(x => x.BoardKey == boardKey).FirstOrDefaultAsync();
            if (board == null)
            {
                return NotFound();
            }

            var lr = board.GetLocalRule() ?? string.Empty;

            return Ok(new { Body = lr });
        }

        // GET: api/Boards/news7vip/billboard
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
            if (!await HasSystemAuthority(SystemAuthority.BoardSetting, boardKey))
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
                else if (info.PropertyType == typeof(string[]))
                {
                    var archivingInfo = boardType.GetProperty("AutoRemovingPredicate");
                    var str = string.Empty;
                    foreach (var pred in item.Value)
                    {
                        str += pred.ToString() + ";";
                    }

                    archivingInfo.SetValue(board, str);
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
            var isAdmin = await HasSystemAuthority(SystemAuthority.ViewResponseDetail);
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
            try
            {
                var ip = IpManager.GetHostName(HttpContext.Connection, HttpContext.Request.Headers);
                var result = await thread.CreateThreadAsync(boardKey, ip, _context, pluginDependency);
                var sess = new SessionManager(HttpContext, _context);
                await sess.UpdateSession();
                return CreatedAtAction(nameof(GetThread), new { id = result.ThreadId }, result);
            }
            catch (BBSErrorException e)
            {
                var error = e.BBSError;
                return StatusCode(error.ResponseCode, error);
            }
        }

        // POST: api/Boards/news7vip/1
        [HttpPost("{boardKey}/{threadId}")]
        public async Task<IActionResult> CreateResponse([FromRoute] string boardKey, [FromRoute] int threadId, [FromBody]ClientResponse body)
        {
            try
            {
                var response =
                    await body.CreateResponseAsync(boardKey, threadId, IpManager.GetHostName(HttpContext.Connection, HttpContext.Request.Headers), _context, pluginDependency);

                var sess = new SessionManager(HttpContext, _context);
                await sess.UpdateSession();

                return CreatedAtAction(nameof(GetThread), new { id = threadId }, response);
            }
            catch (BBSErrorException e)
            {
                var error = e.BBSError;
                return StatusCode(error.ResponseCode, error);
            }

        }
        // PATCH: api/Boards/news7vip/2
        [HttpPatch("{boardKey}/{threadId}")]
        public async Task<IActionResult> PatchThreadStatus([FromRoute] string boardKey, [FromRoute] int threadId, [FromBody] JObject body)
        {
            var baseAuthority = SystemAuthority.Admin | SystemAuthority.BoardSetting | SystemAuthority.BoardsManagement | SystemAuthority.Owner;
            var target = _context.Threads.FirstOrDefaultAsync(x => x.BoardKey == boardKey && x.ThreadId == threadId);
            var patchableProps = typeof(Thread).GetProperties().Where(x => x.GetCustomAttributes(typeof(PatchableAttribute), false).Length > 0);
            foreach (var prop in patchableProps)
            {
                var propName = prop.Name.ToLower();
                foreach (var bItem in body)
                {
                    if (bItem.Key.ToLower() == propName)
                    {
                        if (propName == "stopped" && !await HasSystemAuthority(SystemAuthority.ThreadStop | baseAuthority, boardKey))
                        {
                            return Unauthorized();
                        }
                        else if (propName == "archived" && !await HasSystemAuthority(SystemAuthority.ThreadArchive | baseAuthority, boardKey))
                        {
                            return Unauthorized();
                        }
                        else if (!await HasSystemAuthority(baseAuthority,boardKey))
                        {
                            return Unauthorized();
                        }
                        var result = await target;
                        if (result == null)
                        {
                            return NotFound();
                        }
                        prop.SetValue(result, bItem.Value.ToObject(prop.PropertyType));
                    }
                }
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/boards/news7vip/1/2
        [HttpDelete("{boardKey}/{threadId}/{responseId}")]
        public async Task<IActionResult> DeleteResponse([FromRoute] string boardKey, [FromRoute] int threadId,
                                                        [FromRoute] int responseId, [FromQuery] bool isRemove)
        {
            var response = await _context.Responses.FirstOrDefaultAsync(x => x.ThreadId == threadId && x.Id == responseId);
            if (response == null || (await _context.Threads.FindAsync(threadId)).BoardKey != boardKey)
            {
                return NotFound();
            }

            if (!isRemove)
            {
                if (!await HasSystemAuthority(SystemAuthority.AboneResponse))
                {
                    return Unauthorized();
                }

                response.IsAboned = true;
            }
            else
            {
                if (await HasSystemAuthority(SystemAuthority.RemoveResponse))
                {
                    return Unauthorized();
                }

                _context.Responses.Remove(response);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{boardKey}/{threadId}/{responseId}")]
        public async Task<IActionResult> EditResponse([FromRoute] string boardKey, [FromRoute] int threadId,
                                                      [FromRoute] int responseId, [FromBody] Response response)
        {
            if (!await HasSystemAuthority(SystemAuthority.EditResponse))
            {
                return Unauthorized();
            }

            _context.Responses.Update(response);
            await _context.SaveChangesAsync();
            return Ok(response);
        }

        [HttpDelete("{boardKey}/{threadKey}")]
        public async Task<IActionResult> DeleteThread([FromRoute] string boardKey, [FromRoute] int threadId, [FromQuery] bool isRemove)
        {
            var thread = await _context.Threads.FirstOrDefaultAsync(x => x.BoardKey == boardKey && x.ThreadId == threadId);
            if (thread == null)
            {
                return NotFound();
            }
            else
            {
                if (isRemove)
                {
                    _context.Threads.Remove(thread);
                }
                else
                {
                    if (await HasSystemAuthority(SystemAuthority.ThreadArchive))
                    {
                        thread.Archived = true;
                    }
                }

                await _context.SaveChangesAsync();
                return Ok();
            }

        }

        private bool BoardExists(string boardKey)
        {
            return _context.Boards.Any(e => e.BoardKey == boardKey);
        }
    }

}
