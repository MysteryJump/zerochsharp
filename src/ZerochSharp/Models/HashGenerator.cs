using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ZerochSharp.Models
{
    public class HashGenerator
    {
        public static string GenerateSHA512(string target)
        {
            var data = Encoding.UTF8.GetBytes(target);
            var alg = SHA512.Create();
            var hashs = alg.ComputeHash(data);
            alg.Dispose();
            var sb = new StringBuilder();
            hashs.ToList().ForEach(x => sb.Append(x.ToString("X2")));
            return sb.ToString();
        }
        
    }
}
