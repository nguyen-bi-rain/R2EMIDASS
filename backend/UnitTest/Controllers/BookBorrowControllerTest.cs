using LMS.Controllers;
using LMS.DTOs;
using LMS.DTOs.Shared;
using LMS.Models.Enums;
using LMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTest.Controllers
{
    [TestFixture]
    public class BookBorrowControllerTest
    {
        private Mock<IBookBorrowingRequestService> _mockService;
        private BorrowBookController _controller;

        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IBookBorrowingRequestService>();
            _controller = new BorrowBookController(_mockService.Object);
        }

        // Tests for CreateBookBorrowingRequest
        [Test]
        public async Task CreateBookBorrowingRequest_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var request = new BookBorrowingRequestDtoCreate
            {
                RequestorId = Guid.NewGuid(),
                BookBorrowingRequestDetails = new List<BookBorrowingRequestDetailsDtoCreate>
                {
                    new BookBorrowingRequestDetailsDtoCreate { BookId = Guid.NewGuid() }
                }
            };
            
            _mockService.Setup(s => s.CreateBookBorrowingRequestAsync(It.IsAny<BookBorrowingRequestDtoCreate>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateBookBorrowingRequest(request);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var response = okResult.Value as ApiResponse<string>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.IsSuccess, Is.True);
            Assert.That(response.Data, Is.EqualTo("Book borrowing request created successfully."));
            
            _mockService.Verify(s => s.CreateBookBorrowingRequestAsync(request), Times.Once);
        }

        [Test]
        public async Task CreateBookBorrowingRequest_ServiceThrowsException_ThrowsException()
        {
            // Arrange
            var request = new BookBorrowingRequestDtoCreate
            {
                RequestorId = Guid.NewGuid(),
                BookBorrowingRequestDetails = new List<BookBorrowingRequestDetailsDtoCreate>
                {
                    new BookBorrowingRequestDetailsDtoCreate { BookId = Guid.NewGuid() }
                }
            };
            
            _mockService.Setup(s => s.CreateBookBorrowingRequestAsync(It.IsAny<BookBorrowingRequestDtoCreate>()))
                .ThrowsAsync(new InvalidOperationException("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => _controller.CreateBookBorrowingRequest(request));
            
            _mockService.Verify(s => s.CreateBookBorrowingRequestAsync(request), Times.Once);
        }

        // Tests for UpdateBookBorrowingRequest
        [Test]
        public async Task UpdateBookBorrowingRequest_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var updateStatus = new BookBorrowingRequestDtoUpdateStatus
            {
                Id = 1,
                Status = Status.Approved,
                ApproverId = Guid.NewGuid()
            };
            
            _mockService.Setup(s => s.UpdateBookBorrowingRequestStatusAsync(It.IsAny<BookBorrowingRequestDtoUpdateStatus>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateBookBorrowingRequest(updateStatus);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var response = okResult.Value as ApiResponse<string>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.IsSuccess, Is.True);
            Assert.That(response.Data, Is.EqualTo("Book borrowing request updated successfully."));
            
            _mockService.Verify(s => s.UpdateBookBorrowingRequestStatusAsync(updateStatus), Times.Once);
        }

        [Test]
        public async Task UpdateBookBorrowingRequest_ServiceThrowsException_ThrowsException()
        {
            // Arrange
            var updateStatus = new BookBorrowingRequestDtoUpdateStatus
            {
                Id = 1,
                Status = Status.Approved,
                ApproverId = Guid.NewGuid()
            };
            
            _mockService.Setup(s => s.UpdateBookBorrowingRequestStatusAsync(It.IsAny<BookBorrowingRequestDtoUpdateStatus>()))
                .ThrowsAsync(new KeyNotFoundException("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.UpdateBookBorrowingRequest(updateStatus));
            
            _mockService.Verify(s => s.UpdateBookBorrowingRequestStatusAsync(updateStatus), Times.Once);
        }

        // Tests for GetAllBookBorrowedRequest
        [Test]
        public async Task GetAllBookBorrowedRequest_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var paginatedList = new PaginatedList<BookBorrowingRequestResponse>(
                new List<BookBorrowingRequestResponse> { new BookBorrowingRequestResponse { Id = 1 } },
                1, 1, 10);
            
            _mockService.Setup(s => s.GetAllBookBorrowingRequestsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(paginatedList);

            // Act
            var result = await _controller.GetAllBookBorrowedRequest(10, 1, -1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var response = okResult.Value as ApiResponse<PaginatedList<BookBorrowingRequestResponse>>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.IsSuccess, Is.True);
            Assert.That(response.Data.TotalCount, Is.EqualTo(1));
            
            _mockService.Verify(s => s.GetAllBookBorrowingRequestsAsync(-1, 1, 10), Times.Once);
        }

        [Test]
        public async Task GetAllBookBorrowedRequest_ServiceThrowsException_ThrowsException()
        {
            // Arrange
            _mockService.Setup(s => s.GetAllBookBorrowingRequestsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new InvalidOperationException("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => _controller.GetAllBookBorrowedRequest(10, 1, -1));
            
            _mockService.Verify(s => s.GetAllBookBorrowingRequestsAsync(-1, 1, 10), Times.Once);
        }

        // Tests for DeleteBookBorrowingRequest
        [Test]
        public async Task DeleteBookBorrowingRequest_ValidId_ReturnsOkResult()
        {
            // Arrange
            int id = 1;
            
            _mockService.Setup(s => s.DeleteBookBorrowingRequestAsync(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteBookBorrowingRequest(id);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var response = okResult.Value as ApiResponse<string>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.IsSuccess, Is.True);
            Assert.That(response.Data, Is.EqualTo("Book borrowing request deleted successfully."));
            
            _mockService.Verify(s => s.DeleteBookBorrowingRequestAsync(id), Times.Once);
        }

        [Test]
        public async Task DeleteBookBorrowingRequest_ServiceThrowsException_ThrowsException()
        {
            // Arrange
            int id = 1;
            
            _mockService.Setup(s => s.DeleteBookBorrowingRequestAsync(It.IsAny<int>()))
                .ThrowsAsync(new KeyNotFoundException("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.DeleteBookBorrowingRequest(id));
            
            _mockService.Verify(s => s.DeleteBookBorrowingRequestAsync(id), Times.Once);
        }

        // Tests for GetBookBorrowingRequestById
        [Test]
        public async Task GetBookBorrowingRequestById_ValidId_ReturnsOkResult()
        {
            // Arrange
            int id = 1;
            var response = new BookBorrowingRequestResponse { Id = id };
            
            _mockService.Setup(s => s.GetBookBorrowingRequestByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetBookBorrowingRequestById(id);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var apiResponse = okResult.Value as ApiResponse<BookBorrowingRequestResponse>;
            Assert.That(apiResponse, Is.Not.Null);
            Assert.That(apiResponse.IsSuccess, Is.True);
            Assert.That(apiResponse.Data.Id, Is.EqualTo(id));
            
            _mockService.Verify(s => s.GetBookBorrowingRequestByIdAsync(id), Times.Once);
        }

        [Test]
        public async Task GetBookBorrowingRequestById_ServiceThrowsException_ThrowsException()
        {
            // Arrange
            int id = 1;
            
            _mockService.Setup(s => s.GetBookBorrowingRequestByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new KeyNotFoundException("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.GetBookBorrowingRequestById(id));
            
            _mockService.Verify(s => s.GetBookBorrowingRequestByIdAsync(id), Times.Once);
        }

        // Tests for GetBookBorrowingRequestsByUserId
        [Test]
        public async Task GetBookBorrowingRequestsByUserId_ValidUserId_ReturnsOkResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var paginatedList = new PaginatedList<BookBorrowingRequestResponse>(
                new List<BookBorrowingRequestResponse> { new BookBorrowingRequestResponse { Id = 1 } },
                1, 1, 10);
            
            _mockService.Setup(s => s.GetBookBorrowingRequestsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(paginatedList);

            // Act
            var result = await _controller.GetBookBorrowingRequestsByUserId(userId, 10, 1, -1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var response = okResult.Value as ApiResponse<PaginatedList<BookBorrowingRequestResponse>>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.IsSuccess, Is.True);
            Assert.That(response.Data.TotalCount, Is.EqualTo(1));
            
            _mockService.Verify(s => s.GetBookBorrowingRequestsByUserIdAsync(userId, 1, 10, -1), Times.Once);
        }

        [Test]
        public async Task GetBookBorrowingRequestsByUserId_ServiceThrowsException_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            
            _mockService.Setup(s => s.GetBookBorrowingRequestsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new InvalidOperationException("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => _controller.GetBookBorrowingRequestsByUserId(userId, 10, 1, -1));
            
            _mockService.Verify(s => s.GetBookBorrowingRequestsByUserIdAsync(userId, 1, 10, -1), Times.Once);
        }

        // Tests for GetCurrentMonthlyRequetsCount
        [Test]
        public async Task GetCurrentMonthlyRequetsCount_ValidUserId_ReturnsOkResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            int count = 2;
            
            _mockService.Setup(s => s.GetCurrentMonthlyRequetsCountAsync(It.IsAny<Guid>()))
                .ReturnsAsync(count);

            // Act
            var result = await _controller.GetCurrentMonthlyRequetsCount(userId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var response = okResult.Value as ApiResponse<string>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.IsSuccess, Is.True);
            Assert.That(response.Data, Is.EqualTo(count.ToString()));
            Assert.That(response.Message, Is.EqualTo("Current monthly requests count retrieved successfully."));
            
            _mockService.Verify(s => s.GetCurrentMonthlyRequetsCountAsync(userId), Times.Once);
        }

        [Test]
        public async Task GetCurrentMonthlyRequetsCount_ServiceThrowsException_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            
            _mockService.Setup(s => s.GetCurrentMonthlyRequetsCountAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new InvalidOperationException("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => _controller.GetCurrentMonthlyRequetsCount(userId));
            
            _mockService.Verify(s => s.GetCurrentMonthlyRequetsCountAsync(userId), Times.Once);
        }
    }
}