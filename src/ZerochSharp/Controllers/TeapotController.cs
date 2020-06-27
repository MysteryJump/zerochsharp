using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZerochSharp.Controllers
{
    [Route("/teapot/teapot")]
    [Controller]
    public class TeapotController : Controller
    {
        [Route("")]
        [Produces("text/html")]
        public IActionResult Index()
        {
            return StatusCode(418, Html);
        }

        private const string Html =
            @"<html><head><title>Zeroch Sharp</title></head><body><h1>Do you want to drink a cup of tea or coffee? I'm thirsty.</h1><span style=""width: 100%;font-size: 12rem"">☕</span></body></html>";
    }
}
