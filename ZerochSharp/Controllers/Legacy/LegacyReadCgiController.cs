using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZerochSharp.Models;

namespace ZerochSharp.Controllers.Legacy
{
    [Route("/test/read.cgi/{boardKey}/{threadKey}")]
    [Controller]
    public class LegacyReadCgiController : ControllerBase
    {
        private MainContext _context;
        public LegacyReadCgiController(MainContext context)
        {
            _context = context;
        }

        [Route("")]
        public async Task<IActionResult> GetReadCgi([FromRoute] string boardKey, [FromRoute] int threadKey)
        {
            var thread = await _context.Threads.FirstOrDefaultAsync(x => x.DatKey == threadKey && x.BoardKey == boardKey);
            if (thread != null)
            {
                return base.LocalRedirectPermanent($"~/{boardKey}/{thread.ThreadId}/");
            }
            else
            {
                return NotFound();
            }
        }
    }
}
