using AutoMapper;
using DTOs;
using Entities;
using Moq;
using Repositeries;
using Service;
using System.Collections.Generic;
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
        public async Task AddUserServices_ValidUser_ReturnsMappedUserPublicDTO()
        {
            // Arrange
            var newUserDto = new UserRegisterDTO("new@test.com", "StrongPassword123", "First", "Last", "Phone", "Address");
            var mappedUser = new User { UserName = "new@test.com", Password = "StrongPassword123" };
            var returnedDto = new UserPublicDTO("new@test.com", "First", "Last", 1, "Phone", "Address", "Role");

            _mapperMock.Setup(m => m.Map<UserRegisterDTO, User>(It.IsAny<UserRegisterDTO>())).Returns(mappedUser);
            _userRepoMock.Setup(repo => repo.AddUser(It.IsAny<User>())).ReturnsAsync(mappedUser);
            _mapperMock.Setup(m => m.Map<User, UserPublicDTO>(It.IsAny<User>())).Returns(returnedDto);

            // Act
            var result = await _userService.addUserServices(newUserDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(returnedDto.UserName, result.UserName);
            _userRepoMock.Verify(repo => repo.AddUser(mappedUser), Times.Once);
        }

        [Fact]
        public async Task GetUsers_ReturnsMappedUserPublicDTOs()
        {
            // Arrange
            var users = new List<User> { new User { UserId = 1, UserName = "test1" } };
            var userDtos = new List<UserPublicDTO> { new UserPublicDTO("test1", "First", "Last", 1, "Phone", "Address", "Role") };

            _userRepoMock.Setup(repo => repo.GetUsers()).ReturnsAsync(users);
            _mapperMock.Setup(m => m.Map<IEnumerable<User>, IEnumerable<UserPublicDTO>>(users)).Returns(userDtos);

            // Act
            var result = await _userService.GetUsers();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            _userRepoMock.Verify(repo => repo.GetUsers(), Times.Once);
        }

        [Fact]
        public async Task GetById_ReturnsMappedUserPublicDTO()
        {
            // Arrange
            var user = new User { UserId = 5 };
            var userDto = new UserPublicDTO("test", "First", "Last", 5, "Phone", "Address", "Role");

            _userRepoMock.Setup(repo => repo.GetById(5)).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<User, UserPublicDTO>(user)).Returns(userDto);

            // Act
            var result = await _userService.GetById(5);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.UserId);
        }

        [Fact]
        public async Task IsUserNameExists_ReturnsTrue_WhenUserExists()
        {
            // Arrange
            _userRepoMock.Setup(repo => repo.IsUserNameExists("test@test.com")).ReturnsAsync(true);

            // Act
            var result = await _userService.IsUserNameExists("test@test.com");

            // Assert
            Assert.True(result);
            _userRepoMock.Verify(repo => repo.IsUserNameExists("test@test.com"), Times.Once);
        }

        [Fact]
        public async Task LoginServices_ReturnsUserPublicDTO_WhenSuccessful()
        {
            // Arrange
            var userLoginDto = new UserLoginDTO("login@test.com", "123");
            var mappedUser = new User { UserName = "login@test.com", Password = "123" };
            var returnedDto = new UserPublicDTO("login@test.com", "First", "Last", 1, "Phone", "Address", "Role");

            _mapperMock.Setup(m => m.Map<UserLoginDTO, User>(It.IsAny<UserLoginDTO>())).Returns(mappedUser);
            _userRepoMock.Setup(repo => repo.Login(It.IsAny<User>())).ReturnsAsync(mappedUser);
            _mapperMock.Setup(m => m.Map<User, UserPublicDTO>(It.IsAny<User>())).Returns(returnedDto);

            // Act
            var result = await _userService.loginServices(userLoginDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(returnedDto.UserName, result.UserName);
        }

        [Fact]
        public async Task LoginServices_ReturnsNull_WhenLoginFails()
        {
            // Arrange
            var userLoginDto = new UserLoginDTO("wrong@test.com", "wrong");
            var mappedUser = new User { UserName = "wrong@test.com", Password = "wrong" };

            _mapperMock.Setup(m => m.Map<UserLoginDTO, User>(It.IsAny<UserLoginDTO>())).Returns(mappedUser);
            _userRepoMock.Setup(repo => repo.Login(It.IsAny<User>())).ReturnsAsync((User)null);

            // Act
            var result = await _userService.loginServices(userLoginDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Update_MapsAndCallsPutOnRepository()
        {
            // Arrange
            var userId = 10;
            var userDto = new UserRegisterDTO("update@test.com", "pass", "UpdatedName", "Last", "Phone", "Address");
            var mappedUser = new User { UserId = userId, FirstName = "UpdatedName" };

            _mapperMock.Setup(m => m.Map<UserRegisterDTO, User>(It.IsAny<UserRegisterDTO>())).Returns(mappedUser);
            _userRepoMock.Setup(repo => repo.Put(userId, It.IsAny<User>())).ReturnsAsync((User)null);

            // Act
            await _userService.update(userDto, userId);

            // Assert
            _mapperMock.Verify(m => m.Map<UserRegisterDTO, User>(It.IsAny<UserRegisterDTO>()), Times.Once);
            _userRepoMock.Verify(repo => repo.Put(userId, It.IsAny<User>()), Times.Once);
        }
    }
}