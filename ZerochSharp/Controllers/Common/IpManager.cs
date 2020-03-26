using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ZerochSharp.Controllers.Common
{
    public class IpManager
    {
        private static string GetHostName(ConnectionInfo connectionInfo)
        {
            try
            {
                var ip = connectionInfo.RemoteIpAddress.MapToIPv4().ToString();
                return ip;
            }
            catch (SocketException)
            {
                return null;
            }
        }
        private static string GetHostName(IHeaderDictionary headers)
        {
            if (headers.TryGetValue("CF-Connecting-IP", out var ip) && Startup.IsUsingCloudflare)
            {
                return ip;
            }
            else
            {
                return null;
            }

        }
        public static string GetHostName(ConnectionInfo connectionInfo, IHeaderDictionary headers)
        {
            var data = GetHostName(headers);
            if (data != null)
            {
                return data;
            }
            else
            {
                return GetHostName(connectionInfo);
            }
        }
    }
}
