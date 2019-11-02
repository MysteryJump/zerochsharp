using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using ZerochPlus.Models;

namespace ZerochPlus.Controllers
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
            var indexHtml = await System.IO.File.ReadAllTextAsync("ClientApp/dist/index.html");
            indexHtml = indexHtml.Replace("Zeroch Sharp", board?.BoardName ?? "Zeroch Sharp");
            return Ok(indexHtml);
        }
    }
#endif
}