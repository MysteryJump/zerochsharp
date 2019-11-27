using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Web;
using ZerochSharp.Models;
using ZerochSharp.Controllers.Common;

namespace ZerochSharp.Controllers.Legacy
{
    [Route("/test/bbs.cgi")]
    [Controller]
    public class LegacyBbsCgiController : ControllerBase
    {
        private readonly MainContext _context;

        public LegacyBbsCgiController(MainContext context)
        {
            _context = context;
        }
        [Route("")]
        // [Consumes("application/x-www-form-urlencoded")]
        [Produces("text/plain; charset=shift_jis")]
        [HttpPost()]
        public async Task<IActionResult> Index()
        {
            // var forms = HttpContext.Request.Form;
            //var body = HttpContext.Request.Body;
            using var sr = new StreamReader(Request.Body);
            var str = await sr.ReadToEndAsync();
            try
            {
                var req = new BbsCgiRequest(str, _context, HttpContext.Connection);
                await req.ApplyRequest();
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message == "Body or Title (if you make thread) is neeeded.")
                {
                    return BadRequest(ex.Message);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch
            {
                return BadRequest();
            }
            return Ok(@"<html lang=""ja"">
<head>
<title>書きこみました。</title>
<meta http-equiv=""Content-Type"" content=""text/html; charset=shift_jis"">
</head>
<body>書きこみが終わりました。<br><br>
画面を切り替えるまでしばらくお待ち下さい。<br><br>
<br><br><br><br><br>
<center>
</center>
</body>
</html>");


        }

        public class BbsCgiRequest
        {
            public string Body { get; set; }
            public string BoardKey { get; set; }
            public string DatKey { get; set; }
            public string Name { get; set; }
            public string Mail { get; set; }
            public string Title { get; set; }

            public bool IsThread { get; set; } = true;

            private readonly MainContext _context;
            private readonly ConnectionInfo _connectionInfo;
            public BbsCgiRequest(string rawText, MainContext context, ConnectionInfo connectionInfo)
            {
                var splittedKeys = rawText.Split('&');
                var sjis = Encoding.GetEncoding("Shift-JIS");
                foreach (var item in splittedKeys)
                {
                    if (!item.Contains("="))
                        throw new ArgumentException();
                    var keyValues = item.Split('=').ToList();

                    var key = keyValues[0];
                    switch (key)
                    {
                        case "bbs":
                            BoardKey = keyValues[1];
                            break;
                        case "key":
                            DatKey = keyValues[1];
                            break;
                        case "FROM":
                            Name = HttpUtility.UrlDecode(keyValues[1], sjis);
                            break;
                        case "mail":
                            Mail = HttpUtility.UrlDecode(keyValues[1], sjis);
                            break;
                        case "MESSAGE":
                            Body = HttpUtility.UrlDecode(keyValues[1], sjis);
                            break;
                        case "subject":
                            Title = HttpUtility.UrlDecode(keyValues[1], sjis);
                            break;
                        case "submit":
                            if (keyValues[1] == "%90V%8BK%83X%83%8C%83b%83h%8D%EC%90%AC")
                            {
                                IsThread = true;
                            }
                            else if (keyValues[1] == "%8F%91%82%AB%8D%9E%82%DE")
                            {
                                IsThread = false;
                            }
                            break;
                    }
                }
                _context = context;
                _connectionInfo = connectionInfo;
                if (string.IsNullOrEmpty(Body) || (IsThread && string.IsNullOrEmpty(Title)))
                {
                    throw new InvalidOperationException("Body or Title (if you make thread) is neeeded.");
                }

                
            }

            public async Task ApplyRequest()
            {

                if (IsThread)
                {
                    await ApplyThreadRequest();
                }
                else
                {
                    await ApplyResponseRequest();
                }


            }

            private async Task ApplyThreadRequest()
            {
                var board = await _context.Boards.FirstOrDefaultAsync(x => x.BoardKey == BoardKey);
                var thread = new Thread();
                var ip = IpManager.GetHostName(_connectionInfo);
                thread.Initialize(ip);
                thread.BoardKey = BoardKey;
                thread.Title = Title;
                var response = new Response() { Body = Body, Mail = Mail, Name = Name };
                var result = _context.Threads.Add(thread);
                await _context.SaveChangesAsync();
                response.Initialize(result.Entity.ThreadId, ip, BoardKey);
                _context.Responses.Add(response);
                Plugins.SharedPlugins.RunPlugins(PluginTypes.Thread, board, thread, response);
                await _context.SaveChangesAsync();
            }
            private async Task ApplyResponseRequest()
            {
                var board = await _context.Boards.FirstOrDefaultAsync(x => x.BoardKey == BoardKey);

                var response = new Response();
                var targetThread = await _context.Threads.Where(x => x.DatKey == long.Parse(DatKey)).FirstOrDefaultAsync();
                response.Initialize(targetThread.ThreadId, IpManager.GetHostName(_connectionInfo), BoardKey);
                response.Body = Body;
                response.Name = Name;
                response.Mail = Mail;
                _context.Responses.Add(response);
                targetThread.ResponseCount++;
                targetThread.Modified = response.Created;
                Plugins.SharedPlugins.RunPlugins(PluginTypes.Thread, board, targetThread, response);

                await _context.SaveChangesAsync();
            }
        }
    }

}