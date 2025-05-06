using LMS.Controllers;
using LMS.DTOs;
using LMS.DTOs.Shared;
using LMS.Exceptions;
using LMS.Models.Enums;
using LMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTest.Controllers
{
    [TestFixture]
    public class AuthControllerTest
    {
        private Mock<IUserService> _mockUserService;
        private AuthController _controller;

        [SetUp]
        public void Setup()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new AuthController(_mockUserService.Object);
        }

        [Test]
        public async Task Login_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var loginRequest = new LoginRequest { UserName = "testuser", Password = "password123" };
            var loginResponse = new LoginResponse { AccessToken = "test-token", RefreshToken = "refresh-token" };
            _mockUserService.Setup(x => x.LoginAsync(It.IsAny<LoginRequest>())).ReturnsAsync(loginResponse);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.InstanceOf<ApiResponse<LoginResponse>>());
            var apiResponse = okResult?.Value as ApiResponse<LoginResponse>;
            Assert.That(apiResponse?.IsSuccess, Is.True);
            Assert.That(apiResponse?.Message, Is.EqualTo("Login successful."));
            Assert.That(apiResponse?.Data, Is.EqualTo(loginResponse));
        }

        [Test]
        public async Task Login_NullRequest_ReturnsBadRequest()
        {
            // Arrange
            LoginRequest loginRequest = null;

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("Login request cannot be null."));
        }

        [Test]
        public async Task Login_ServiceThrowsException_ReturnsBadRequest()
        {
            // Arrange
            var loginRequest = new LoginRequest { UserName = "testuser", Password = "password123" };
            var exceptionMessage = "Invalid credentials";
            _mockUserService.Setup(x => x.LoginAsync(It.IsAny<LoginRequest>())).ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.InstanceOf<ApiResponse<string>>());
            var apiResponse = badRequestResult?.Value as ApiResponse<string>;
            Assert.That(apiResponse?.IsSuccess, Is.False);
            Assert.That(apiResponse?.Message, Is.EqualTo(exceptionMessage));
        }

        [Test]
        public async Task Login_ServiceThrowsResourceNotFoundException_ReturnsBadRequest()
        {
            // Arrange
            var loginRequest = new LoginRequest { UserName = "nonexistentuser", Password = "password123" };
            var exceptionMessage = "User not found";
            _mockUserService.Setup(x => x.LoginAsync(It.IsAny<LoginRequest>())).ThrowsAsync(new ResourceNotFoundException(exceptionMessage));

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.InstanceOf<ApiResponse<string>>());
            var apiResponse = badRequestResult?.Value as ApiResponse<string>;
            Assert.That(apiResponse?.IsSuccess, Is.False);
            Assert.That(apiResponse?.Message, Is.EqualTo(exceptionMessage));
        }

        [Test]
        public async Task Login_ServiceThrowsDatabaseConflictException_ReturnsBadRequest()
        {
            // Arrange
            var loginRequest = new LoginRequest { UserName = "testuser", Password = "wrongpassword" };
            var exceptionMessage = "Invalid password";
            _mockUserService.Setup(x => x.LoginAsync(It.IsAny<LoginRequest>())).ThrowsAsync(new DatabaseConflictException(exceptionMessage));

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.InstanceOf<ApiResponse<string>>());
            var apiResponse = badRequestResult?.Value as ApiResponse<string>;
            Assert.That(apiResponse?.IsSuccess, Is.False);
            Assert.That(apiResponse?.Message, Is.EqualTo(exceptionMessage));
        }

        [Test]
        public async Task Register_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var registerRequest = new RegisterRequest 
            { 

                UserName = "newuser", 
                Password = "password123", 
                Email = "newuser@example.com" ,
                Gender = Gender.Male,
                PhoneNumber = "1234567890",
            };
            _mockUserService.Setup(x => x.RegisterAsync(It.IsAny<RegisterRequest>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Register(registerRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.InstanceOf<ApiResponse<string>>());
            var apiResponse = okResult?.Value as ApiResponse<string>;
            Assert.That(apiResponse?.IsSuccess, Is.True);
            Assert.That(apiResponse?.Message, Is.EqualTo("User registered successfully."));
        }

        [Test]
        public async Task Register_ServiceThrowsException_ThrowsException()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                UserName = "existinguser",
                Password = "password123",
                Email = "existing@example.com"
            };
            var exceptionMessage = "User already exists";
            _mockUserService.Setup(x => x.RegisterAsync(It.IsAny<RegisterRequest>())).ThrowsAsync(new ResourceUniqueException(exceptionMessage));

            // Act & Assert
            var ex = Assert.ThrowsAsync<ResourceUniqueException>(() => _controller.Register(registerRequest));
            Assert.That(ex.Message, Is.EqualTo(exceptionMessage));
        }
    }
}