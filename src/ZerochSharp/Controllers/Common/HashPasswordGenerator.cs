using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace ZerochSharp.Controllers.Common
{
    public static class HashPasswordGenerator
    {
        public static (string hashed, string salt) GeneratePasswordHash(string password, int userid)
        {
            using (var saltGen = RandomNumberGenerator.Create())
            {
                var salt = new byte[256 / 8];
                saltGen.GetBytes(salt);
                var saltStr = Convert.ToBase64String(salt);
                return (GeneratePasswordHashBySalt(password, userid, saltStr), saltStr);
            }
        }

        public static string GeneratePasswordHashBySalt(string password, int userid, string salt)
        {
            var userSaltStr = Models.HashGenerator.GenerateSHA512(userid.ToString());
            string hashed = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(password, Encoding.UTF8.GetBytes(salt + userSaltStr), KeyDerivationPrf.HMACSHA512, 1 << 11, 512));
            return hashed;
        }
        public static bool VerifyPassword(string password, int userid, string hash, string salt) =>
            GeneratePasswordHashBySalt(password, userid, salt) == hash;
    }
}
