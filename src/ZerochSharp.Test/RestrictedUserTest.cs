using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using ZerochSharp.Models;

namespace ZerochSharp.Test
{
    public class RestrictedUserTest
    {
        [Fact]
        public void RegexTest()
        {
            var candidates = "regex:/(192|191).168.0.1/;";
            var board = new Board();
            board.RestrictedUsers = candidates;
            Assert.True(board.IsRestricted(new[] { "192.168.0.1" }));
            Assert.False(board.IsRestricted(new[] { "192.56.23.1" }));
            candidates = "regex:/14a7:[a-z]5f0:1f21:(84f|563):.{4}:dad7:[eafb]5b8:.{3}/i;";
            board.RestrictedUsers = candidates;
            Assert.True(board.IsRestricted(new[] { "14a7:A5f0:1f21:563:aaaa:dad7:e5b8:e53" }));
            Assert.False(board.IsRestricted(new[] { "14a7:A5f0:1f21:363:aaaa:dad7:e5b8:e53" }));
        }

        [Fact]
        public void TextTest()
        {
            var candidates = "192.168.*.*";
            var board = new Board();
            board.RestrictedUsers = candidates;
            Assert.True(board.IsRestricted(new[] { "192.168.0.1" }));
            Assert.False(board.IsRestricted(new[] { "192.162.0.1" }));
            board.RestrictedUsers = "14a7:a5f0:1f21:363:aaaa:*:e5b8:e53";
            Assert.True(board.IsRestricted(new[] { "14a7:a5f0:1f21:363:aaaa:aaaa:e5b8:e53" }));
            Assert.False(board.IsRestricted(new[] { "14a7:a5f0:1f21:363:aaba:dad7:e5b8:e53" }));
        }
    }
}
