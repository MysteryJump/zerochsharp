using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using ZerochSharp.Models;

namespace ZerochSharp.Test
{
    public class UserTest
    {
        [Fact]
        public void IsValidUserNameTest()
        {
            var validList = new[]
            {
                "tghiwisghioh1245",
                "CarlvonClausewitz",
                "xx__HANNIBAL__xx",
                "SirWilfridLaurier"
            };
            var invalidList = new[]
            {
                "er3",
                "geiotgjeiogj;++++",
                "____++++++-----"
            };
            for (int i = 0; i < validList.Length; i++)
            {
                var user = new User();
                user.UserId = validList[i];
                Assert.True(user.IsValidUserName());
            }
            for (int i = 0; i < invalidList.Length; i++)
            {
                var user = new User();
                user.UserId = invalidList[i];
                Assert.False(user.IsValidUserName());
            }
        }
    }
}
