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
            var thread = await Thread.GetThreadAsync(boardKey, long.Parse(datKey), _context, datKey: true);
            bool isfirst = true;
            var sb = new StringBuilder();
 
            foreach (var item in thread.Responses)
            {
                var date = item.Created.ToString("yyyy/MM/dd(ddd) HH:mm:ss.FF");
                if (string.IsNullOrEmpty(item.Name))
                {
                    item.Name = thread.AssociatedBoard.BoardDefaultName;
                }
                if (isfirst)
                {
                    sb.AppendLine($"{item.Name}<>{item.Mail}<>{date} ID:{item.Author}<> {item.Body.Replace("\n", "<br>")} <> {thread.Title}");
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