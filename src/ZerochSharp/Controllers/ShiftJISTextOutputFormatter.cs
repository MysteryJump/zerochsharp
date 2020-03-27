using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ZerochSharp.Controllers
{
    public class ShiftJISTextOutputFormatter : TextOutputFormatter
    {
        private static readonly Encoding DefaultEncoding = Encoding.GetEncoding("Shift_JIS");

        protected override bool CanWriteType(Type type)
        {
            if (typeof(string).IsAssignableFrom(type))
            {
                return base.CanWriteType(type);
            }
            return false;
        }

        public ShiftJISTextOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/plain; charset=shift_jis"));
            SupportedEncodings.Add(DefaultEncoding);
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            return context.HttpContext.Response.WriteAsync(context.Object as string, DefaultEncoding);
        }
    }
}
