using LMS.Controllers;
using LMS.DTOs;
using LMS.DTOs.Shared;
using LMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTest.Controllers
{
    [TestFixture]
    public class CategoryControllerTests
    {
        private Mock<ICategoryService> _mockCategoryService;
        private CategoryController _controller;

        [SetUp]
        public void Setup()
        {
            _mockCategoryService = new Mock<ICategoryService>();
            _controller = new CategoryController(_mockCategoryService.Object);
        }

        [Test]
        public async Task GetAllCategories_ReturnsOkResult()
        {
            // Arrange
            var expectedData = new PaginatedList<CategoryDto>(
                new List<CategoryDto> { new CategoryDto { Id = 1, Name = "Test Category" } },
                1, 10, 1);
            
            _mockCategoryService.Setup(service => service.GetAllAsync(10, 1))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetAllCategories();

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            var apiResponse = (ApiResponse<PaginatedList<CategoryDto>>)okResult.Value;
            Assert.That(apiResponse.Data, Is.EqualTo(expectedData));
            Assert.That(apiResponse.Message, Is.EqualTo("Category list retrieved successfully."));
        }

        [Test]
        public async Task GetCategoryById_ReturnsOkResult()
        {
            // Arrange
            var categoryId = 1;
            var expectedCategory = new CategoryDto { Id = categoryId, Name = "Test Category" };
            
            _mockCategoryService.Setup(service => service.GetByIdAsync(categoryId))
                .ReturnsAsync(expectedCategory);

            // Act
            var result = await _controller.GetCategoryById(categoryId);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            var apiResponse = (ApiResponse<CategoryDto>)okResult.Value;
            Assert.That(apiResponse.Data, Is.EqualTo(expectedCategory));
            Assert.That(apiResponse.Message, Is.EqualTo("Category retrieved successfully."));
        }

        [Test]
        public async Task AddCategory_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var categoryDto = new CategoryCreatDto { Name = "New Category" };
            
            _mockCategoryService.Setup(service => service.AddAsync(categoryDto))
                .ReturnsAsync(categoryDto);

            // Act
            var result = await _controller.AddCategory(categoryDto);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            var apiResponse = (ApiResponse<CategoryCreatDto>)okResult.Value;
            Assert.That(apiResponse.Data, Is.EqualTo(categoryDto));
            Assert.That(apiResponse.Message, Is.EqualTo("Category created successfully."));
        }

        [Test]
        public async Task AddCategory_WithInvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var categoryDto = new CategoryCreatDto { Name = "" };
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.AddCategory(categoryDto);

            // Assert
            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = (BadRequestObjectResult)result;
            var apiResponse = (ApiResponse<string>)badRequestResult.Value;
            Assert.That(apiResponse.Message, Is.EqualTo("Invalid data."));
        }

        [Test]
        public async Task DeleteCategory_ReturnsOkResult()
        {
            // Arrange
            var categoryId = 1;
            
            _mockCategoryService.Setup(service => service.DeleteAsync(categoryId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteCategory(categoryId);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var okResult = (OkObjectResult)result;
            var apiResponse = (ApiResponse<string>)okResult.Value;
            Assert.That(apiResponse.Message, Is.EqualTo("Category deleted successfully."));
        }

        [Test]
        public async Task UpdateCategory_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var categoryDto = new CategoryUpdateDto { Id = 1, Name = "Updated Category" };
            
            _mockCategoryService.Setup(service => service.UpdateAsync(categoryDto))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateCategory(categoryDto);

            // Assert
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        public async Task UpdateCategory_WithInvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var categoryDto = new CategoryUpdateDto { Id = 1, Name = "" };
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.UpdateCategory(categoryDto);

            // Assert
            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = (BadRequestObjectResult)result;
            var apiResponse = (ApiResponse<string>)badRequestResult.Value;
            Assert.That(apiResponse.Message, Is.EqualTo("Invalid data."));
        }
    }
}
