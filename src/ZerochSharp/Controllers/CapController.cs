using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZerochSharp.Models;
using ZerochSharp.Services;

namespace ZerochSharp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CapController : ApiControllerBase
    {
        public CapController(MainContext context, PluginDependency dependency) : base(context, dependency)
        {

        }
    }
}
