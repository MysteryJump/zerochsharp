using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZerochSharp.Models;
using ZerochSharp.Services;

namespace ZerochSharp.Controllers
{
    
    public abstract class ApiControllerBase : ControllerBase
    {
        protected readonly MainContext _context;
        protected readonly PluginDependency pluginDependency;
        protected User CurrentUser { get; private set; }
        private bool isExcutedGetSessionUser = false;
        public ApiControllerBase(MainContext context, PluginDependency dependency)
        {
            _context = context;
            pluginDependency = dependency;
            CurrentUser = null;
        }
        protected async Task GetSessionUserAsync()
        {
            if (!isExcutedGetSessionUser)
            {
                if (HttpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    var session = new UserSession
                    {
                        SessionToken = HttpContext.Request.Headers["Authorization"]
                    };
                    CurrentUser = await session.GetSessionUserAsync(_context);
                }
                isExcutedGetSessionUser = true;
            }
        }
        protected async Task<bool> HasSystemAuthority(SystemAuthority authority, string boardKey = null)
        {
            await GetSessionUserAsync();
            return CurrentUser?.HasSystemAuthority(authority, boardKey) ?? false;
        }
    }
}
