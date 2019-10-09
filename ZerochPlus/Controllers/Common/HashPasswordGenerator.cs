using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZerochPlus.Controllers.Common
{
    public static class HashPasswordGenerator
    {
        public static string GeneratePasswordHash(string password, int userid)
        {
            return Models.HashGenerator.GenerateSHA512(password + ":" + userid);
        }
    }
}
