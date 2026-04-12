using Entities;
using Repositeries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests.TestRepository.IntegrationTest
{
    [Collection("Database Collection")]
    public class UserRepositoryIntegrationTests : IAsyncLifetime
    {
        private readonly Store_215962135Context _dbContext;
        private readonly UserRepository _userRepository;

        public UserRepositoryIntegrationTests(DatabaseFixture databaseFixture)
        {
            _dbContext = databaseFixture.Context;
            _userRepository = new UserRepository(_dbContext);
        }

        public async Task InitializeAsync() => await ClearDatabase();
        public async Task DisposeAsync() => await ClearDatabase();

        private async Task ClearDatabase()
        {
            _dbContext.ChangeTracker.Clear();
            _dbContext.Users.RemoveRange(_dbContext.Users);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task AddUser_NewUser_SavesToDatabase()
        {
            // Arrange
            var user = new User
            {
                UserName = "integration@user.com",
                Password = "SecurePass123",
                FirstName = "First",
                LastName = "Last"
            };

            // Act
            var result = await _userRepository.AddUser(user);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.UserId > 0);
            Assert.Equal("User", result.Role);

            var dbUser = _dbContext.Users.FirstOrDefault(u => u.UserId == result.UserId);
            Assert.NotNull(dbUser);
        }
        [Fact]
        public async Task GetUsers_Integration_ReturnsAllUsers()
        {
            // Arrange
            await _dbContext.Users.AddAsync(new User { UserName = "user1@test.com", Password = "123", FirstName = "A", LastName = "B" });
            await _dbContext.Users.AddAsync(new User { UserName = "user2@test.com", Password = "123", FirstName = "C", LastName = "D" });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetUsers();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count() >= 2);
        }

        [Fact]
        public async Task GetById_Integration_ReturnsCorrectUser()
        {
            // Arrange
            var user = new User { UserName = "specific@test.com", Password = "123", FirstName = "A", LastName = "B" };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetById(user.UserId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("specific@test.com", result.UserName);
        }
    }
}