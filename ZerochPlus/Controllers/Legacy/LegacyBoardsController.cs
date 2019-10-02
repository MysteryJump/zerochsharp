using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZerochPlus.Models;

namespace ZerochPlus.Controllers.Legacy
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
            var data = await _context.Threads.Where(x => x.BoardKey == boardKey).OrderByDescending(x => x.Modified).ToListAsync();
            var sb = new StringBuilder();
            var ts = new TimeSpan(+9, 0, 0);
            foreach (var item in data)
            {
                sb.AppendLine($"{item.DatKey}.dat<>{item.Title} ({item.ResponseCount})");
            }

            var utf = Encoding.Default;
            var shiftJis = Encoding.GetEncoding("Shift_JIS");

            var ubytes = utf.GetBytes(sb.ToString());
            var bytests = Encoding.Convert(utf, shiftJis, ubytes);
            var sss = shiftJis.GetString(bytests);
            return sss;

        }
    }
}