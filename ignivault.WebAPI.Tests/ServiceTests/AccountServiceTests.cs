using ignivault.WebAPI.Data.Entities;
using ignivault.WebAPI.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ignivault.WebAPI.Tests.ServiceTests
{
    public class AccountServiceTests
    {
        [Fact]
        public async Task GetProfileAsync_ShouldReturnCorrectProfileDto()
        {
            var store = new Mock<IUserStore<LoginUser>>();
            var mockUserManager = new Mock<UserManager<LoginUser>>(store.Object, null, null, null, null, null, null, null, null);
            var mockActivityLogger = new Mock<IUserActivityLogger>();
            var testUserId = "user123";
            var mockUser = new LoginUser
            {
                Id = testUserId,
                UserName = "testuser",
                Email = "test@example.com"
            };

            mockUserManager.Setup(um => um.FindByIdAsync(testUserId)).ReturnsAsync(mockUser);

            var accountService = new AccountService(mockUserManager.Object, null, null, null, mockActivityLogger.Object);

            var result = await accountService.GetProfileAsync(testUserId);

            Assert.NotNull(result);
            Assert.Equal(mockUser.UserName, result.Username);
            Assert.Equal(mockUser.Email, result.Email);
        }
    }
}
