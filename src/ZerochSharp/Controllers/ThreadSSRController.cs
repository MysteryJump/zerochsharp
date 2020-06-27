using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZerochSharp.Models;

// ReSharper disable once EmptyNamespace
namespace ZerochSharp.Controllers
{
#if RELEASE
    // only release package
    [Route(@"{boardKey:regex(^(?!.*(\.js|\.css|\.ico|\.txt)).*$)}/{threadId}")]
    [Controller]
    public class ThreadSSRController : Controller
    {
        private MainContext _context;
        public ThreadSSRController(MainContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Produces("text/html")]
        public async Task<IActionResult> Index([FromRoute] string boardKey, [FromRoute] int threadId)
        {
            var indexHtml = await System.IO.File.ReadAllTextAsync("ReactClient/build/index.html");
            var board = await _context.Boards.FirstOrDefaultAsync(x => x.BoardKey == boardKey);
            var thread = await _context.Threads.FirstOrDefaultAsync(x => x.ThreadId == threadId && x.BoardKey == boardKey);
            var firstResponse = await _context.Responses.FirstOrDefaultAsync(x => x.ThreadId == threadId);
            return Ok(InjectOgpData(indexHtml, board, thread, firstResponse));
        }
        private string InjectOgpData(string indexHtml, Board board, Thread thread, Response firstResponse)
        {
            return indexHtml.Replace("%OGP:PAGE_TITLE%", $"{thread.Title} ({board.BoardName}) | {Startup.SiteName}")
                            .Replace("%OGP:PAGE_TYPE%", "article")
                            .Replace("%OGP:SITE_TITLE", Startup.SiteName)
                            .Replace("%OGP:PAGE_DESCRIPTION%", firstResponse.Body)
                            .Replace("%OGP:FACEBOOK_APP_ID%", "") // facebook app_id
                            .Replace("%OGP:TWITTER_CARD_TYPE%", "summary");
        }
    }
#endif
}
