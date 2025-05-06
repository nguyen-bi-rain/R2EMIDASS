using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using LMS.DTOs;
using LMS.Exceptions;
using LMS.Helpers;
using LMS.Models.Enitties;
using LMS.Repositories.Interfaces;
using LMS.Services.Implements;
using LMS.Services.Interfaces;
using Moq;

namespace UnitTest.Services
{
    [TestFixture]
    public class UserServiceTest
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<ITokenService> _tokenServiceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IHashPasswordHelper> _hashPasswordHelperMock;
        private UserService _userService;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _tokenServiceMock = new Mock<ITokenService>();
            _mapperMock = new Mock<IMapper>();
            _hashPasswordHelperMock = new Mock<IHashPasswordHelper>();
            _userService = new UserService(_userRepositoryMock.Object, _tokenServiceMock.Object, _mapperMock.Object, _hashPasswordHelperMock.Object);
        }

        [Test]
        public async Task GetUserByIdAsync_ValidId_ReturnsUserDto()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId };
            var userDto = new UserDto { Id = userId };

            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);
            _mapperMock.Setup(mapper => mapper.Map<UserDto>(user)).Returns(userDto);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.That(result, Is.EqualTo(userDto));
            _userRepositoryMock.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
            _mapperMock.Verify(mapper => mapper.Map<UserDto>(user), Times.Once);
        }

        [Test]
        public void GetUserByIdAsync_InvalidId_ThrowsArgumentException()
        {
            // Arrange
            var invalidId = Guid.Empty;

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _userService.GetUserByIdAsync(invalidId));
            Assert.That(ex.Message, Is.EqualTo("Invalid user ID (Parameter 'id')"));
        }

        [Test]
        public void GetUserByIdAsync_UserNotFound_ThrowsResourceNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((User)null);

            // Act & Assert
            Assert.ThrowsAsync<ResourceNotFoundException>(() => _userService.GetUserByIdAsync(userId));
        }

        [Test]
        public async Task LoginAsync_ValidCredentials_ReturnsLoginResponse()
        {
            // Arrange
            var loginRequest = new LoginRequest { UserName = "testuser", Password = "password" };
            var user = new User { UserName = "testuser", PasswordHash = "hashedpassword" };
            var accessToken = "accessToken";
            var refreshToken = "refreshToken";

            _userRepositoryMock.Setup(repo => repo.GetUserByUserName(loginRequest.UserName)).ReturnsAsync(user);
            _tokenServiceMock.Setup(service => service.GenerateTokenAsync(user)).ReturnsAsync(accessToken);
            _tokenServiceMock.Setup(service => service.GenerateRefreshToken()).Returns(refreshToken);
            _hashPasswordHelperMock.Setup(helper => helper.VerifyPassword(loginRequest.Password, user.PasswordHash))
                .Returns(true);

            // Act
            var result = await _userService.LoginAsync(loginRequest);

            // Assert
            Assert.That(result.AccessToken, Is.EqualTo(accessToken));
            Assert.That(result.RefreshToken, Is.EqualTo(refreshToken));
            _userRepositoryMock.Verify(repo => repo.UpdateAsync(user), Times.Once);
        }

        [Test]
        public void LoginAsync_InvalidPassword_ThrowsDatabaseConflictException()
        {
            // Arrange
            var loginRequest = new LoginRequest { UserName = "testuser", Password = "wrongpassword" };
            var user = new User { UserName = "testuser", PasswordHash = "hashedpassword" };

            _userRepositoryMock.Setup(repo => repo.GetUserByUserName(loginRequest.UserName)).ReturnsAsync(user);
            _hashPasswordHelperMock.Setup(helper => helper.VerifyPassword(loginRequest.Password, user.PasswordHash))
                .Returns(false);

            // Act & Assert
            Assert.ThrowsAsync<DatabaseConflictException>(() => _userService.LoginAsync(loginRequest));
        }

        [Test]
        public async Task RegisterAsync_ValidRequest_AddsUser()
        {
            // Arrange
            var registerRequest = new RegisterRequest { UserName = "newuser", Password = "password" };
            var user = new User { UserName = "newuser" };

            _userRepositoryMock.Setup(repo => repo.GetUserByUserName(registerRequest.UserName)).ReturnsAsync((User)null);
            _mapperMock.Setup(mapper => mapper.Map<User>(registerRequest)).Returns(user);
            _userRepositoryMock.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _userService.RegisterAsync(registerRequest);

            // Assert
            _userRepositoryMock.Verify(repo => repo.AddAsync(user), Times.Once);
        }

        [Test]
        public void RegisterAsync_UserAlreadyExists_ThrowsResourceUniqueException()
        {
            // Arrange
            var registerRequest = new RegisterRequest { UserName = "existinguser" };
            var existingUser = new User { UserName = "existinguser" };

            _userRepositoryMock.Setup(repo => repo.GetUserByUserName(registerRequest.UserName)).ReturnsAsync(existingUser);

            // Act & Assert
            Assert.ThrowsAsync<ResourceUniqueException>(() => _userService.RegisterAsync(registerRequest));
        }

        [Test]
        public async Task RefreshTokenAsync_ValidToken_ReturnsNewTokens()
        {
            // Arrange
            var refreshTokenRequest = new RefreshTokenRequest { AccessToken = "expiredAccessToken", RefreshToken = "validRefreshToken" };
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, RefreshToken = "validRefreshToken", RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(10) };
            var newAccessToken = "newAccessToken";
            var newRefreshToken = "newRefreshToken";

            _tokenServiceMock.Setup(service => service.GetPrincipalFromExpiredToken(refreshTokenRequest.AccessToken))
                .Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(JwtRegisteredClaimNames.Sid, userId.ToString()) })));
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);
            _tokenServiceMock.Setup(service => service.GenerateTokenAsync(user)).ReturnsAsync(newAccessToken);
            _tokenServiceMock.Setup(service => service.GenerateRefreshToken()).Returns(newRefreshToken);

            // Act
            var result = await _userService.RefreshTokenAsync(refreshTokenRequest);

            // Assert
            Assert.That(result.AccessToken, Is.EqualTo(newAccessToken));
            Assert.That(result.RefreshToken, Is.EqualTo(newRefreshToken));
            _userRepositoryMock.Verify(repo => repo.UpdateAsync(user), Times.Once);
            _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void RefreshTokenAsync_InvalidRefreshToken_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var refreshTokenRequest = new RefreshTokenRequest { AccessToken = "expiredAccessToken", RefreshToken = "invalidRefreshToken" };
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, RefreshToken = "validRefreshToken", RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(10) };

            _tokenServiceMock.Setup(service => service.GetPrincipalFromExpiredToken(refreshTokenRequest.AccessToken))
                .Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(JwtRegisteredClaimNames.Sid, userId.ToString()) })));
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userService.RefreshTokenAsync(refreshTokenRequest));
        }
    }
}