using Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Repositeries;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Tests.TestRepository.UnitTest
{
    public class UserRepositoryUnitTests
    {
        private Mock<Store_215962135Context> GetMockContext()
        {
            var options = new DbContextOptionsBuilder<Store_215962135Context>().Options;
            return new Mock<Store_215962135Context>(options);
        }

        [Fact]
        public async Task GetById_UserExists_ReturnsUser()
        {
            // Arrange
            var userId = 1;
            var users = new List<User>
            {
                new User { UserId = userId, UserName = "test@test.com" }
            };

            var mockContext = GetMockContext();
            mockContext.Setup(ctx => ctx.Users).ReturnsDbSet(users);
            mockContext.Setup(ctx => ctx.Users.FindAsync(userId)).ReturnsAsync(users[0]);

            var repository = new UserRepository(mockContext.Object);

            // Act
            var result = await repository.GetById(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test@test.com", result.UserName);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsUser()
        {
            // Arrange
            var users = new List<User>
            {
                new User { UserName = "login@test.com", Password = "123" }
            };

            var mockContext = GetMockContext();
            mockContext.Setup(ctx => ctx.Users).ReturnsDbSet(users);

            var repository = new UserRepository(mockContext.Object);

            var userToLogin = new User { UserName = "login@test.com", Password = "123" };

            // Act
            var result = await repository.Login(userToLogin);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("login@test.com", result.UserName);
        }
    }
}