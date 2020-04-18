using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZerochSharp.Models;

namespace ZerochSharp.Controllers
{
    [Route("/teapot/teapot")]
    [Controller]
    public class TeapotController : Controller
    {
        private readonly MainContext _context;
        public TeapotController(MainContext context)
        {
            _context = context;
        }
        [Route("")]
        [Produces("text/html")]
        public IActionResult Index()
        {
            return StatusCode(418, html);
        }

        private const string html = @"<html><head><title>Zeroch Sharp</title></head><body><h1>Do you want to drink a cup of tea or coffee? I'm thirsty.</h1><span style=""width: 100%;font-size: 12rem"">☕</span></body></html>";
    }
}
