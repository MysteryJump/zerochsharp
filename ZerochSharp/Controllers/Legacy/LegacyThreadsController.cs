using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZerochSharp;
using ZerochSharp.Models;

namespace ZerochSharp.Controllers
{
    [Route("/{boardKey}/dat/{datKey}.dat")]
    [Controller]
    public class LegacyThreadsController : ControllerBase
    {
        private readonly MainContext _context;

        public LegacyThreadsController(MainContext context)
        {
            _context = context;
        }

        [Route("")]
        [Produces("text/plain; charset=shift_jis")]
        public async Task<string> Index([FromRoute] string boardKey, [FromRoute]string datKey)
        {
            if (!Startup.IsUsingLegacyMode)
            {
                return "";
            }
            var data = await _context.Threads.FirstOrDefaultAsync(x => x.BoardKey == boardKey && x.DatKey == long.Parse(datKey));
            var board = await _context.Boards.FirstOrDefaultAsync(x => x.BoardKey == boardKey);
            var aboned = Models.Response.AbonedResponse(board.BoardDeleteName);
            if (data == null)
            {
                return "";
            }
            bool isfirst = true;

            var sb = new StringBuilder();
            var responses = await _context.Responses.Where(x => x.ThreadId == data.ThreadId).OrderBy(x => x.Created).ToListAsync();
            var abonedList = new List<int>();
            var i = 0;
            foreach (var item in responses)
            {
                if (item.IsAboned)
                {
                    abonedList.Add(i);
                }
                i++;
            }
            foreach (var item in abonedList)
            {
                responses[item] = aboned;
            }
            foreach (var item in responses)
            {
                var date = item.Created.ToString("yyyy/MM/dd(ddd) HH:mm:ss.FF");
                if (string.IsNullOrEmpty(item.Name))
                {
                    item.Name = board.BoardDefaultName;
                }
                if (isfirst)
                {
                    sb.AppendLine($"{item.Name}<>{item.Mail}<>{date} ID:{item.Author}<> {item.Body.Replace("\n", "<br>")} <> {data.Title}");
                    isfirst = false;
                }
                else
                {
                    sb.AppendLine($"{item.Name}<>{item.Mail}<>{date} ID:{item.Author}<> {item.Body.Replace("\n", "<br>")} <>");
                }
            }
            return sb.ToString();
        }
    }
}