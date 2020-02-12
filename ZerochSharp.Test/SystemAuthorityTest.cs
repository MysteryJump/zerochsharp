using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using ZerochSharp.Models;

namespace ZerochSharp.Test
{
    public class SystemAuthorityTest
    {
        private User mockUser;
        public SystemAuthorityTest()
        {
            mockUser = new User();
        }
        [Fact]
        public void SystemAdminTest()
        {
            mockUser.SystemAuthority = SystemAuthority.Admin;
            foreach (SystemAuthority auth in Enum.GetValues(typeof(SystemAuthority)))
            {
                if (auth != SystemAuthority.Owner)
                {
                    Assert.True(mockUser.HasSystemAuthority(auth));
                }
                else
                {
                    Assert.False(mockUser.HasSystemAuthority(auth));
                }
            }
        }
        [Fact]
        public void OwnerTest()
        {
            mockUser.SystemAuthority = SystemAuthority.Owner;
            foreach (SystemAuthority auth in Enum.GetValues(typeof(SystemAuthority)))
            {
                Assert.True(mockUser.HasSystemAuthority(auth));
            }
        }
        [Fact]
        public void CapTest()
        {
            mockUser.SystemAuthority = SystemAuthority.CapUserSetting;
            foreach (SystemAuthority auth in Enum.GetValues(typeof(SystemAuthority)))
            {
                if (auth != SystemAuthority.CapUserSetting)
                {
                    Assert.False(mockUser.HasSystemAuthority(auth));
                }
                else
                {
                    Assert.True(mockUser.HasSystemAuthority(auth));
                }
            }
        }
        [Fact]
        public void BoardsManagementTest()
        {
            mockUser.SystemAuthority = SystemAuthority.BoardsManagement;
            foreach (SystemAuthority auth in Enum.GetValues(typeof(SystemAuthority)))
            {
                if (auth != SystemAuthority.CapUserSetting && auth != SystemAuthority.Owner && auth != SystemAuthority.Admin)
                {
                    Assert.True(mockUser.HasSystemAuthority(auth));
                }
                else
                {
                    Assert.False(mockUser.HasSystemAuthority(auth));
                }
            }
        }
        [Fact]
        public void InnerBoardTest1()
        {
            mockUser.SystemAuthority = SystemAuthority.EditResponse & SystemAuthority.ThreadArchive;
            mockUser.ControllableBoards = new[] { "news7vip" };

            foreach (SystemAuthority auth in Enum.GetValues(typeof(SystemAuthority)))
            {
                if (auth == SystemAuthority.EditResponse || auth == SystemAuthority.ThreadArchive)
                {
                    Assert.True(mockUser.HasSystemAuthority(auth, "news7vip"));
                }
                else
                {
                    Assert.False(mockUser.HasSystemAuthority(auth));
                }
            }
        }
        [Fact]
        public void InnerBoardTest2()
        {
            mockUser.SystemAuthority = SystemAuthority.ThreadStop & SystemAuthority.RemoveResponse;
            mockUser.ControllableBoards = new[] { "news7vip" };

            foreach (SystemAuthority auth in Enum.GetValues(typeof(SystemAuthority)))
            {
                Assert.False(mockUser.HasSystemAuthority(auth));
            }
        }
        [Fact]
        public void InnerBoardTest3()
        {
            mockUser.SystemAuthority = SystemAuthority.ThreadArchive & SystemAuthority.ThreadStop
                & SystemAuthority.EditResponse & SystemAuthority.ViewResponseDetail & SystemAuthority.BoardSetting
                & SystemAuthority.RemoveResponse & SystemAuthority.AboneResponse;
            mockUser.ControllableBoards = new[] { "news7vip" };
            foreach (SystemAuthority auth in Enum.GetValues(typeof(SystemAuthority)))
            {
                if (auth == SystemAuthority.ThreadArchive
                    || auth == SystemAuthority.ThreadStop
                    || auth == SystemAuthority.EditResponse
                    || auth == SystemAuthority.ViewResponseDetail
                    || auth == SystemAuthority.BoardSetting
                    || auth == SystemAuthority.RemoveResponse
                    || auth == SystemAuthority.AboneResponse)
                {
                    Assert.True(mockUser.HasSystemAuthority(auth, "news7vip"));
                }
                else
                {
                    Assert.False(mockUser.HasSystemAuthority(auth, "news7vip"));
                }
            }
        }
        [Fact]
        public void MultiBoardTest1()
        {
            mockUser.SystemAuthority = SystemAuthority.BoardSetting;
            mockUser.ControllableBoards = new[] { "news7vip", "coffeehouse", "huntingblow", "blessingunion" };
            string boardKey = "news7vip";
            foreach (SystemAuthority auth in Enum.GetValues(typeof(SystemAuthority)))
            {
                if (auth == SystemAuthority.BoardsManagement
                    || auth == SystemAuthority.Admin
                    || auth == SystemAuthority.CapUserSetting
                    || auth == SystemAuthority.Owner)
                {
                    Assert.False(mockUser.HasSystemAuthority(auth, boardKey));
                }
                else
                {
                    Assert.True(mockUser.HasSystemAuthority(auth, boardKey));
                }
            }
        }
        [Fact]
        public void MultiBoardTest2()
        {
            mockUser.SystemAuthority = SystemAuthority.BoardSetting;
            mockUser.ControllableBoards = new[] { "reactingworld", "startingend", "refandingmusic" };
            string boardKey = "jumpingboard";
            foreach (SystemAuthority auth in Enum.GetValues(typeof(SystemAuthority)))
            {
                Assert.False(mockUser.HasSystemAuthority(auth, boardKey));
            }
        }


        [Fact]
        public void RemoveResponseTest()
        {
            mockUser.SystemAuthority = SystemAuthority.RemoveResponse;
            var boardKey = "news";
            mockUser.ControllableBoards = new[] { boardKey };
            foreach (SystemAuthority auth in Enum.GetValues(typeof(SystemAuthority)))
            {
                if (auth == SystemAuthority.RemoveResponse || auth == SystemAuthority.AboneResponse)
                {
                    Assert.True(mockUser.HasSystemAuthority(auth, boardKey));
                }
                else
                {
                    Assert.False(mockUser.HasSystemAuthority(auth, boardKey));
                }
            }
        }

    }
}
