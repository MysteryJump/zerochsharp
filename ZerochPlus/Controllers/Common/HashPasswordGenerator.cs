using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;

namespace ZerochPlus.Controllers.Common
{
    public static class HashPasswordGenerator
    {
        public static string GeneratePasswordHash(string password, int userid)
        {
            //var salt = $":{userid}:salt;";
            //return BCrypt.Net.BCrypt.HashPassword(password, salt);
            return Models.HashGenerator.GenerateSHA512(password + ":" + userid);
        }
        public static bool VerifyPassword(string password, string hash ,int userid)
        {
            throw new NotImplementedException();
            var salt = $":{userid}:salt";
            //return BCrypt.Net.BCrypt.Verify(password, hash)
        }
    }
}
