using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text;
using System.Threading.Tasks;
using ZerochSharp.Models;

namespace ZerochSharp.Controllers
{
    // only release packages
#if RELEASE
    [Route(@"{boardKey:regex(^(?!.*(\.js|\.css|\.ico|\.txt)).*$)}/")]
    [Controller]
    public class BoardsSSRController : Controller
    {
        private MainContext _context;
        public BoardsSSRController(MainContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Produces("text/html")]
        public async Task<IActionResult> Index([FromRoute] string boardKey)
        {
            var indexHtml = await System.IO.File.ReadAllTextAsync("ReactClient/build/index.html");

            var ua = HttpContext.Request.Headers["User-Agent"];
            if (IsMonazilla(ua))
            {
                return Ok(indexHtml);
            }
            var board = await _context.Boards.FirstOrDefaultAsync(x => x.BoardKey == boardKey);
            if (board == null)
            {
                return Ok(indexHtml);
            }
            indexHtml = indexHtml.Replace("ZerochSharp Client", board.BoardName);
            indexHtml = InjectOgpData(indexHtml, board);
            //            indexHtml = indexHtml.Replace("{}",
            //                @"{
            //  ""@context"": ""https://schema.org/"", 
            //  ""@type"": ""BreadcrumbList"",
            //  ""itemListElement"": [{
            //                ""@type"": ""ListItem"", 
            //    ""position"": 1, 
            //    ""name"": """ + board.BoardName + @""",
            //    ""item"": """ + Startup.BBSBaseUrl + board.BoardKey + @"""
            //  }]
            //}");
            return Content(indexHtml, "text/html", Encoding.GetEncoding("Shift_JIS"));
        }
        private bool IsMonazilla(string uaString)
        {
            return uaString.ToLower().IndexOf("monazilla") >= 0;
        }
        private string InjectOgpData(string indexHtml, Board board)
        {
            return indexHtml.Replace("%OGP:PAGE_TITLE%", $"{board.BoardName} | {Startup.SiteName}")
                            .Replace("%OGP:PAGE_TYPE%", "article")
                            .Replace("%OGP:SITE_TITLE", Startup.SiteName)
                            .Replace("%OGP:PAGE_DESCRIPTION%", board.BoardSubTitle)
                            .Replace("%OGP:FACEBOOK_APP_ID%", "") // facebook app_id
                            .Replace("%OGP:TWITTER_CARD_TYPE%", "summary");
        }
    }
#endif

}