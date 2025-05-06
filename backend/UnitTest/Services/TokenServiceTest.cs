using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LMS.Models.Enitties;
using LMS.Repositories.Interfaces;
using LMS.Services.Implements;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace UnitTest.Services
{
    [TestFixture]
    public class TokenServiceTest
    {
        private Mock<IUserRepository> _mockUserRepository;
        private TokenService _tokenService;

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository>();

            Environment.SetEnvironmentVariable("JWTSETTINGS__KEY", "TestSecretKey12345678901234567890");
            Environment.SetEnvironmentVariable("JWTSETTINGS__ISSUER", "TestIssuer");
            Environment.SetEnvironmentVariable("JWTSETTINGS__AUDIENCE", "TestAudience");
            Environment.SetEnvironmentVariable("JWTSETTINGS__EXPIRETIME", "60");

            _tokenService = new TokenService(_mockUserRepository.Object);
        }

        [Test]
        public void GenerateRefreshToken_ShouldReturnBase64String()
        {
            // Act
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Assert
            Assert.IsNotNull(refreshToken);
            Assert.DoesNotThrow(() => Convert.FromBase64String(refreshToken));
        }

        [Test]
        public async Task GenerateTokenAsync_ShouldReturnValidJwtToken()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid() };
            _mockUserRepository.Setup(repo => repo.GetUserRoleAsync(user.Id)).ReturnsAsync("Admin");

            // Act
            var token = await _tokenService.GenerateTokenAsync(user);

            // Assert
            Assert.IsNotNull(token);
            var handler = new JwtSecurityTokenHandler();
            Assert.DoesNotThrow(() => handler.ReadJwtToken(token));
        }

        [Test]
        public void GetPrincipalFromExpiredToken_ShouldThrowException_WhenTokenIsInvalid()
        {
            // Arrange
            var invalidToken = "InvalidToken";

            // Act & Assert
            Assert.Throws<SecurityTokenException>(() => _tokenService.GetPrincipalFromExpiredToken(invalidToken));
        }

        [Test]
        public async Task GetClaimsAsync_ShouldReturnClaimsWithRole_WhenUserHasRole()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid() };
            _mockUserRepository.Setup(repo => repo.GetUserRoleAsync(user.Id)).ReturnsAsync("SUPER_USER");

            // Act
            var claims = await InvokePrivateMethodAsync<List<Claim>>("GetClaimsAsync", user);

            // Assert
            Assert.That(claims, Is.Not.Null);
            Assert.That(claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "SUPER_USER"), Is.True);
        }

        [Test]
        public async Task GetClaimsAsync_ShouldReturnClaimsWithoutRole_WhenUserHasNoRole()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid() };
            _mockUserRepository.Setup(repo => repo.GetUserRoleAsync(user.Id)).ReturnsAsync(string.Empty);

            // Act
            var claims = await InvokePrivateMethodAsync<List<Claim>>("GetClaimsAsync", user);

            // Assert
            Assert.IsNotNull(claims);
            Assert.That(claims.Any(c => c.Type == ClaimTypes.Role), Is.False);
        }

        private async Task<T> InvokePrivateMethodAsync<T>(string methodName, params object[] parameters)
        {
            var method = typeof(TokenService).GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (method == null)
            {
                throw new MissingMethodException($"Method {methodName} not found in TokenService.");
            }

            var result = method.Invoke(_tokenService, parameters);
            if (result is Task<T> taskResult)
            {
                return await taskResult;
            }

            return (T)result;
        }
    }
}