using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using ZerochSharp.Models;
using ZerochSharp.Models.Caps;
using ZerochSharp.Services;

namespace ZerochSharp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CapGroupController : ApiControllerBase
    {
        public CapGroupController(MainContext context, PluginDependency dependency) : base(context, dependency)
        {

        }

        // POST: api/capgroup
        [HttpPost]
        public async Task<IActionResult> CreateCapGroup([FromBody] CapGroup capGroup)
        {
            await Context.CapGroups.AddAsync(capGroup);
            await Context.SaveChangesAsync();
            return CreatedAtAction("GetCapGroups", capGroup);
        }
        // DELETE: api/capgroup/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCapGroup([FromRoute] int id)
        {
            Context.CapGroups.RemoveRange(Context.CapGroups.Where(x => x.Id == id));
            await Context.SaveChangesAsync();
            return Ok();
        }

        // PUT: api/capgroup/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCapGroupChanges([FromBody] JObject obj, [FromRoute] int id)
        {
            var capGroup = await Context.CapGroups.FirstOrDefaultAsync(x => x.Id == id);
            var capGroupType = typeof(CapGroup);
            foreach (var item in obj)
            {
                var key = capGroupType.GetProperty(item.Key);
                if (key == null)
                {
                    continue;
                }
                key.SetValue(capGroup, item.Key);
            }

            return Ok();
        }

        // PUT: api/capgroup/1/boards
        [HttpPut("{id}/boards")]
        public async Task<IActionResult> PutRangeOfCapGroupChanges([FromBody] JArray obj, [FromRoute] int id)
        {
            var isAcceptable = true;
            var boardKeys = new List<int>();
            foreach (var item in obj)
            {
                var board = await Context.Boards.FirstOrDefaultAsync(x => x.BoardKey == item.ToString());
                if (board == null)
                {
                    isAcceptable = false;
                }
                else
                {
                    boardKeys.Add(board.Id);
                }
            }

            if (!isAcceptable)
            {
                return BadRequest("this board system doesn't have some boards inferred from given board key");
            }

            Context.CapGroupBoards.RemoveRange(Context.CapGroupBoards.Where(x => x.CapGroupId == id));   
            await Context.SaveChangesAsync();

            await Context.CapGroupBoards.AddRangeAsync(boardKeys.Select(x => new CapGroupBoardPair()
            {
                BoardId = x,
                CapGroupId = id
            }));
            await Context.SaveChangesAsync();
            return Ok();
        }

        // GET: api/capgroup/
        [HttpGet]
        public async Task<IActionResult> GetCapGroups([FromQuery] string boardKey = null)
        {
            if (string.IsNullOrWhiteSpace(boardKey))
            {
                return Ok(await Context.CapGroups.ToListAsync());
            }
            else
            {

                var items = await Context.CapGroups.Join(Context.CapGroupBoards,
                    x => x.Id,
                    x => x.CapGroupId,
                    (group, pair) => new
                    {
                        Name = group.CapGroupName,
                        group.Id,
                        group.Description,
                        pair.BoardId
                    })
                    .Join(Context.Boards, x => x.BoardId, x => x.Id, (x, y) => new
                    {
                        x.Name,
                        x.Id,
                        x.Description,
                        y.BoardKey
                    })
                    .Where(x => x.BoardKey == boardKey)
                    .ToListAsync();
                return Ok(items);
            }
        }

        // GET: api/capgroup/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCapGroupById([FromRoute] int id)
        {
            var items = await Context.CapGroups.Where(x => x.Id == id)
                .Join(Context.CapGroupCaps,
                    x => x.Id,
                    x => x.CapGroupId,
                    (group, pair) => new
                    {
                        CapGroupInfo = group,
                        pair.CapId
                    })
                .Join(Context.Caps, x => x.CapId, x => x.Id, (groupCap, cap) => new
                {
                    groupCap.CapGroupInfo,
                    Cap = cap
                })
                .Join(Context.CapGroupBoards,
                    capGroupCap => capGroupCap.CapGroupInfo.Id,
                    capGroupBoard => capGroupBoard.CapGroupId,
                    (capGroupCap, capBoard) => new
                    {
                        capGroupCap.CapGroupInfo,
                        capBoard.BoardId,
                        capGroupCap.Cap
                    })
                .ToListAsync();
            var capGroup = items.First().CapGroupInfo;
            return Ok(new
            {
                CapGroup = capGroup,
                Caps = items.Select(x => x.Cap).Distinct(new CapEqualityComparer()),
                BoardIds = items.Select(x => x.BoardId).Distinct()
            });
        }
        private class CapEqualityComparer : IEqualityComparer<Cap>
        {
            public bool Equals(Cap x, Cap y)
            {
                if (x == null && y == null)
                {
                    return true;
                }

                if (x == null || y == null)
                {
                    return false;
                }
                return x.CapId == y.CapId;
            }

            public int GetHashCode(Cap obj)
            {
                return obj.Id;
            }
        }
    }
}
