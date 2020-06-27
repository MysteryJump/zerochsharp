using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZerochSharp.Models;

namespace ZerochSharp.Controllers.Legacy
{
    [Route("/{boardKey}/subject.txt")]
    [Controller]
    public class LegacyBoardsController : ControllerBase
    {
        private readonly MainContext _context;

        public LegacyBoardsController(MainContext context)
        {
            _context = context;
        }

        [Route("")]
        [Produces("text/plain; charset=shift_jis")]
        public async Task<string> Index([FromRoute] string boardKey)
        {
            if (!Startup.IsUsingLegacyMode)
            {
                return "";
            }
            var data = await _context.Threads.Where(x => x.BoardKey == boardKey && !x.Archived)
                                             .OrderByDescending(x => x.SageModified)
                                             .ToListAsync();
            var sb = new StringBuilder();
            foreach (var item in data)
            {
                sb.AppendLine($"{item.DatKey}.dat<>{item.Title} ({item.ResponseCount})");
            }

            var utf = Encoding.Default;
            var shiftJis = Encoding.GetEncoding("Shift_JIS");

            var bytes = utf.GetBytes(sb.ToString());
            var convertedBytes = Encoding.Convert(utf, shiftJis, bytes);
            var sss = shiftJis.GetString(convertedBytes);
            return sss;

        }
    }
}