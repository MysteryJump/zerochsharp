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
using ZerochSharp.Services;

namespace ZerochSharp.Controllers.Legacy
{
    [Route("/test/bbs.cgi")]
    [Controller]
    public class LegacyBbsCgiController : ControllerBase
    {

        private readonly MainContext _context;
        private readonly PluginDependency pluginDependency;
        public LegacyBbsCgiController(MainContext context, PluginDependency plugin)
        {
            pluginDependency = plugin;
            _context = context;
        }
        [Route("")]
        [Produces("text/plain; charset=shift_jis")]
        [HttpPost()]
        public async Task<IActionResult> Index()
        {
            using var sr = new StreamReader(Request.Body);
            BbsCgiRequest req = null;
            string hostAddress = IpManager.GetHostName(HttpContext.Connection, HttpContext.Request.Headers);
            var str = await sr.ReadToEndAsync();
            try
            {
                req = new BbsCgiRequest(str, _context, HttpContext.Connection, HttpContext.Request.Headers, pluginDependency);
                var sess = new SessionManager(HttpContext, _context);
                await sess.UpdateSession();
                await req.ApplyRequest();
            }
            catch (BBSErrorException e)
            {
                // OKなの腹立つわ (5chの仕様)
                return Ok(FormatErrorText(e.BBSError, req?.BoardKey, req?.Name, req?.Mail, req?.Body, hostAddress));
            }

            return Ok(OkString);
        }

        private string FormatErrorText(BBSError error, string boardKey, string name, string mail, string body, string hostAddress)
        {
            return ErrorString.Replace("{{errorText}}", error.ErrorMessage + $" (Error Code: {error.ErrorCode})")
                .Replace("{{hostAddress}}", hostAddress)
                .Replace("{{name}}", name)
                .Replace("{{mail}}", mail)
                .Replace("{{body}}", body)
                .Replace("{{boardKey}}", boardKey);
        }
        private const string OkString = @"<html lang=""ja"">
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
</html>";

        private const string ErrorString = @"
<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"" ""http://www.w3.org/TR/html4/loose.dtd"">
<html lang=""ja"">
<head>
 
 <meta http-equiv=""Content-Type"" content=""text/html; charset=Shift_JIS"">
 
 <title>ＥＲＲＯＲ！</title>
 
</head>
<!--nobanner-->
<body>
<!-- 2ch_X:error -->
<div style=""margin-bottom:2em;"">
<font size=""+1"" color=""#FF0000""><b>ＥＲＲＯＲ：{{errorText}}</b></font>
</div>

<blockquote>
ホスト<b>{{hostAddress}}</b><br>
<br>
名前： {{name}}<b></b><br>
E-mail： {{mail}}<br>
内容：{{body}}<br>

<br>
<br>
</blockquote>
<hr>
<div class=""reload"">こちらでリロードしてください。<a href=""../{{boardKey}}/"">&nbsp;GO!</a></div>
<div align=""right"">Zeroch Sharp</div>
</body>
</html>";

        private class BbsCgiRequest
        {
            public string Body { get; set; }
            public string BoardKey { get; set; }
            public string DatKey { get; set; }
            public string Name { get; set; }
            public string Mail { get; set; }
            public string Title { get; set; }

            public bool IsThread { get; set; } = true;

            private readonly MainContext _context;
            private readonly IHeaderDictionary _headers;
            private readonly ConnectionInfo _connectionInfo;
            private readonly PluginDependency _dependency;
            public BbsCgiRequest(string rawText, MainContext context, ConnectionInfo connectionInfo, IHeaderDictionary headers ,PluginDependency plugin)
            {
                _dependency = plugin;
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
                _headers = headers;
                _context = context;
                _connectionInfo = connectionInfo;
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
                var clientThread = new ClientThread()
                {
                    Title = Title,
                    Response = new ClientResponse() { Body = Body, Mail = Mail, Name = Name }
                };
                var ip = IpManager.GetHostName(_connectionInfo, _headers);

                await clientThread.CreateThreadAsync(BoardKey, ip, _context, _dependency);
            }
            private async Task ApplyResponseRequest()
            {
                if (!long.TryParse(DatKey, out var key))
                {
                    throw new BBSErrorException(BBSErrorType.BBSInvalidThreadKeyError);
                }
                var clientResponse = new ClientResponse() { Body = Body, Name = Name, Mail = Mail };
                await clientResponse.CreateResponseAsync(BoardKey, key, IpManager.GetHostName(_connectionInfo, _headers), _context, _dependency, true);
            }
        }
    }

}