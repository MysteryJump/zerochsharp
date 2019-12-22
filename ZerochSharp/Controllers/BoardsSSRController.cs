using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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
            
            var board = await _context.Boards.FirstOrDefaultAsync(x => x.BoardKey == boardKey);
            var indexHtml = await System.IO.File.ReadAllTextAsync("ReactClient/build/index.html");
            if (board == null)
            {
                return Ok(indexHtml);
            }
            indexHtml = indexHtml.Replace("ZerochSharp Client", board.BoardName);
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
            return Ok(indexHtml);
        }
    }
#endif
}