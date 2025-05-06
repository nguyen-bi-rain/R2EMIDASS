using LMS.Controllers;
using LMS.DTOs;
using LMS.DTOs.Shared;
using LMS.Exceptions;
using LMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;


namespace UnitTest.Controllers
{
    [TestFixture]
    public class BookControllerTests
    {
        private Mock<IBookService> _mockBookService;
        private BookController _controller;

        [SetUp]
        public void Setup()
        {
            _mockBookService = new Mock<IBookService>();
            _controller = new BookController(_mockBookService.Object);
        }

        [Test]
        public async Task GetAllBooks_ReturnsOkWithBooks_WhenSuccessful()
        {
            // Arrange
            var pageSize = 10;
            var pageIndex = 1;
            var books = new PaginatedList<BookResponse>(new List<BookResponse>(), 0, pageIndex, pageSize);
            _mockBookService.Setup(service => service.GetAllBookAsync(pageSize, pageIndex, null, 0))
                .ReturnsAsync(books);

            // Act
            var result = await _controller.GetAllBooks(pageSize, pageIndex);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.InstanceOf<ApiResponse<PaginatedList<BookResponse>>>());
            var apiResponse = okResult.Value as ApiResponse<PaginatedList<BookResponse>>;
            Assert.That(apiResponse.Data, Is.EqualTo(books));
            Assert.That(apiResponse.Message, Is.EqualTo("Books retrieved successfully."));
        }

        [Test]
        public async Task GetAllBooks_WithSearchTerm_ReturnsFilteredBooks()
        {
            // Arrange
            var pageSize = 10;
            var pageIndex = 1;
            string searchTerm = "Programming";
            var books = new PaginatedList<BookResponse>(new List<BookResponse>(), 0, pageIndex, pageSize);
            _mockBookService.Setup(service => service.GetAllBookAsync(pageSize, pageIndex, searchTerm, 0))
                .ReturnsAsync(books);

            // Act
            var result = await _controller.GetAllBooks(pageSize, pageIndex, searchTerm);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            _mockBookService.Verify(s => s.GetAllBookAsync(pageSize, pageIndex, searchTerm, 0), Times.Once);
        }

        [Test]
        public async Task GetAllBooks_WithCategoryId_ReturnsFilteredBooks()
        {
            // Arrange
            var pageSize = 10;
            var pageIndex = 1;
            int categoryId = 5;
            var books = new PaginatedList<BookResponse>(new List<BookResponse>(), 0, pageIndex, pageSize);
            _mockBookService.Setup(service => service.GetAllBookAsync(pageSize, pageIndex, null, categoryId))
                .ReturnsAsync(books);

            // Act
            var result = await _controller.GetAllBooks(pageSize, pageIndex, null, categoryId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            _mockBookService.Verify(s => s.GetAllBookAsync(pageSize, pageIndex, null, categoryId), Times.Once);
        }

        [Test]
        public async Task CreateBook_ReturnsCreatedResult_WhenSuccessful()
        {
            // Arrange
            var bookRequest = new BookRequest { Title = "New Book" };
            _mockBookService.Setup(service => service.AddBookAsync(bookRequest))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateBook(bookRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedResult>());
            _mockBookService.Verify(s => s.AddBookAsync(bookRequest), Times.Once);
        }

        [Test]
        public async Task UpdateBook_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            var bookRequest = new BookUpdateRequest { Title = "Updated Book" };
            _mockBookService.Setup(service => service.UpdateBookAsync(id, bookRequest))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateBook(id, bookRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
            _mockBookService.Verify(s => s.UpdateBookAsync(id, bookRequest), Times.Once);
        }

        [Test]
        public async Task DeleteBook_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockBookService.Setup(service => service.DeleteBookAsync(id))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteBook(id);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
            _mockBookService.Verify(s => s.DeleteBookAsync(id), Times.Once);
        }

        [Test]
        public async Task GetBookById_ReturnsOkWithBook_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            var bookDto = new BookDto { Id = id, Title = "Test Book" };
            _mockBookService.Setup(service => service.GetBookByIdAsync(id))
                .ReturnsAsync(bookDto);

            // Act
            var result = await _controller.GetBookById(id);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.InstanceOf<ApiResponse<BookDto>>());
            var apiResponse = okResult.Value as ApiResponse<BookDto>;
            Assert.That(apiResponse.Data, Is.EqualTo(bookDto));
            Assert.That(apiResponse.Message, Is.EqualTo("Book retrieved successfully."));
        }

        [Test]
        public async Task GetBookById_WhenServiceThrowsResourceNotFoundException_PropagatesException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockBookService.Setup(service => service.GetBookByIdAsync(id))
                .ThrowsAsync(new ResourceNotFoundException("Book not found."));

            // Act & Assert
            var ex = Assert.ThrowsAsync<ResourceNotFoundException>(async () => 
                await _controller.GetBookById(id));
            Assert.That(ex.Message, Is.EqualTo("Book not found."));
        }

        [Test]
        public async Task UpdateBook_WithInvalidId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var bookRequest = new BookUpdateRequest();
            _mockBookService.Setup(service => service.UpdateBookAsync(id, bookRequest))
                .ThrowsAsync(new KeyNotFoundException("Book not found."));

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () => 
                await _controller.UpdateBook(id, bookRequest));
            Assert.That(ex.Message, Is.EqualTo("Book not found."));
        }
    }
}
