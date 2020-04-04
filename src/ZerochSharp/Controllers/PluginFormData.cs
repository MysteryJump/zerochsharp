using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZerochSharp.Controllers
{
    public class PluginFormData
    {
        public IFormFile File { get; set; }
    }
}
