using System;
using Xunit;
using Xunit.Abstractions;
using ZerochPlus.Controllers.Common;

namespace ZerochPlus.Test
{
    public class PasswordTest
    {
        private readonly ITestOutputHelper _output;

        public PasswordTest(ITestOutputHelper helper)
        {
            _output = helper;
        }
        [Fact]
        public void GenerateAndVerifyPasswordTest()
        {
            var (hashed, salt) = HashPasswordGenerator.GeneratePasswordHash("kainkain", 1);
            Assert.True(HashPasswordGenerator.VerifyPassword("kainkain", 1, hashed, salt));
            (hashed, salt) = HashPasswordGenerator.GeneratePasswordHash("gjpostrjhriosjgbiodj", 1333);
            Assert.True(HashPasswordGenerator.VerifyPassword("gjpostrjhriosjgbiodj", 1333, hashed, salt));
            Assert.False(HashPasswordGenerator.VerifyPassword("VerifyPassword", 1333, hashed, salt));
            (hashed, salt) = HashPasswordGenerator.GeneratePasswordHash("kain", 1);
            
            _output.WriteLine("hash:" + hashed);
            _output.WriteLine("salt:" + salt);

        }
    }
}
