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
        protected readonly MainContext Context;
        protected readonly PluginDependency PluginDependency;
        protected User CurrentUser { get; private set; }
        private bool _isExecutedGetSessionUser;

        protected ApiControllerBase(MainContext context, PluginDependency dependency)
        {
            Context = context;
            PluginDependency = dependency;
            CurrentUser = null;
        }
        protected async Task GetSessionUserAsync()
        {
            if (!_isExecutedGetSessionUser)
            {
                if (HttpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    var session = new UserSession
                    {
                        SessionToken = HttpContext.Request.Headers["Authorization"]
                    };
                    CurrentUser = await session.GetSessionUserAsync(Context);
                }
                _isExecutedGetSessionUser = true;
            }
        }
        protected async Task<bool> HasSystemAuthority(SystemAuthority authority, string boardKey = null)
        {
            await GetSessionUserAsync();
            return CurrentUser?.HasSystemAuthority(authority, boardKey) ?? false;
        }
    }
}
