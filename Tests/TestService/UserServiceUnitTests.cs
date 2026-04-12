using AutoMapper;
using DTOs;
using Entities;
using Moq;
using Org.BouncyCastle.Crypto;
using Repositeries;
using Service;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class UserServiceUnitTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IUserPasswordService> _passServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UserService _userService;

        public UserServiceUnitTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _passServiceMock = new Mock<IUserPasswordService>();
            _mapperMock = new Mock<IMapper>();
            _userService = new UserService(_userRepoMock.Object, _passServiceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task AddUserServices_DuplicateUserName_ReturnsNull()
        {
            // Arrange
            var newUser = new User { UserName = "existing@test.com", Password = "StrongPassword123" };
            var existingUsersList = new List<User>
            {
                new User { UserName = "existing@test.com" }
            };

            _userRepoMock.Setup(repo => repo.GetUsers()).ReturnsAsync(existingUsersList);

            // Act
            var result = await _userService.addUserServices(newUser);

            // Assert
            Assert.Null(result);
            _userRepoMock.Verify(repo => repo.AddUser(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task AddUserServices_WeakPassword_ReturnsNull()
        {
            // Arrange
            var newUser = new User { UserName = "new@test.com", Password = "123" };
            var emptyUsersList = new List<User>(); // No duplicates

            _userRepoMock.Setup(repo => repo.GetUsers()).ReturnsAsync(emptyUsersList);
            _passServiceMock.Setup(pass => pass.Level(newUser.Password))
                            .Returns(new UserPassword { Strength = 1 });

            // Act
            var result = await _userService.addUserServices(newUser);

            // Assert
            Assert.Null(result);
            _userRepoMock.Verify(repo => repo.AddUser(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task AddUserServices_ValidUserAndStrongPassword_ReturnsUser()
        {
            // Arrange
            var newUser = new User { UserName = "new@test.com", Password = "StrongPassword123" };
            var emptyUsersList = new List<User>(); // No duplicates

            _userRepoMock.Setup(repo => repo.GetUsers()).ReturnsAsync(emptyUsersList);
            _passServiceMock.Setup(pass => pass.Level(newUser.Password))
                            .Returns(new UserPassword { Strength = 3 }); // Score >= 2
            _userRepoMock.Setup(repo => repo.AddUser(newUser)).ReturnsAsync(newUser);

            // Act
            var result = await _userService.addUserServices(newUser);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newUser.UserName, result.UserName);
            _userRepoMock.Verify(repo => repo.AddUser(newUser), Times.Once);
        }

        [Fact]
        public async Task GetUsers_ReturnsMappedUserDTOs()
        {
            // Arrange
            var users = new List<User> { new User { UserId = 1, UserName = "test1" } };
            var userDtos = new List<UserDTO> { new UserDTO { UserID = 1, UserName = "test1" } };

            _userRepoMock.Setup(repo => repo.GetUsers()).ReturnsAsync(users);
            _mapperMock.Setup(m => m.Map<IEnumerable<User>, IEnumerable<UserDTO>>(users)).Returns(userDtos);

            // Act
            var result = await _userService.GetUsers();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            _userRepoMock.Verify(repo => repo.GetUsers(), Times.Once);
        }

        [Fact]
        public async Task GetById_ReturnsMappedUserDTO()
        {
            // Arrange
            var user = new User { UserId = 5 };
            var userDto = new UserDTO { UserID = 5 };

            _userRepoMock.Setup(repo => repo.GetById(5)).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<User, UserDTO>(user)).Returns(userDto);

            // Act
            var result = await _userService.GetById(5);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.UserID);
        }

        [Fact]
        public async Task LoginServices_ReturnsUserFromRepository()
        {
            // Arrange
            var user = new User { UserName = "login@test.com", Password = "123" };
            _userRepoMock.Setup(repo => repo.Login(user)).ReturnsAsync(user);

            // Act
            var result = await _userService.loginServices(user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.UserName, result.UserName);
        }

        [Fact]
        public async Task Update_MapsAndCallsPutOnRepository()
        {
            // Arrange
            var userId = 10;
            var userDto = new UserDTO { UserID = userId, FirstName = "UpdatedName" };
            var mappedUser = new User { UserId = userId, FirstName = "UpdatedName" };

            _mapperMock.Setup(m => m.Map<User>(userDto)).Returns(mappedUser);
            _userRepoMock.Setup(repo => repo.Put(userId, mappedUser)).ReturnsAsync((Microsoft.AspNetCore.Mvc.ActionResult<User>)null);

            // Act
            await _userService.update(userDto, userId);

            // Assert
            _mapperMock.Verify(m => m.Map<User>(userDto), Times.Once);
            _userRepoMock.Verify(repo => repo.Put(userId, mappedUser), Times.Once);
        }
    }
}