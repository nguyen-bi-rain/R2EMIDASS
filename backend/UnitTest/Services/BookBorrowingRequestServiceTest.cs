using AutoMapper;
using LMS.DTOs;
using LMS.DTOs.Shared;
using LMS.Exceptions;
using LMS.Models.Enitties;
using LMS.Models.Enums;
using LMS.Repositories.Interfaces;
using LMS.Services.Implements;
using LMS.Services.Interfaces;
using LMS.Specifications;
using Moq;
using UnitTest.Common;

namespace UnitTest.Services
{
    [TestFixture]
    public class BookBorrowingRequestServiceTest
    {
        private Mock<IBookBorrowingRequestRepository> _bookBorrowingRequestRepositoryMock;
        private Mock<IBookBorrowingRequestDetailsRepository> _bookBorrowingRequestDetailsRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IBookService> _bookServiceMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<ISendEmailSerivce> _sendEmailServiceMock;
        private BookBorrowingRequestService _service;

        [SetUp]
        public void SetUp()
        {
            _bookBorrowingRequestRepositoryMock = new Mock<IBookBorrowingRequestRepository>();
            _bookBorrowingRequestDetailsRepositoryMock = new Mock<IBookBorrowingRequestDetailsRepository>();
            _mapperMock = new Mock<IMapper>();
            _bookServiceMock = new Mock<IBookService>();
            _userServiceMock = new Mock<IUserService>();
            _sendEmailServiceMock = new Mock<ISendEmailSerivce>();

            _service = new BookBorrowingRequestService(
                _sendEmailServiceMock.Object,
                _userServiceMock.Object,
                _bookBorrowingRequestRepositoryMock.Object,
                _bookServiceMock.Object,
                _mapperMock.Object,
                _bookBorrowingRequestDetailsRepositoryMock.Object);
        }

        [Test]
        public async Task CreateBookBorrowingRequestAsync_ValidRequest_ShouldCreateSuccessfully()
        {
            // Arrange
            var requestDto = new BookBorrowingRequestDtoCreate
            {
                RequestorId = Guid.NewGuid(),
                BookBorrowingRequestDetails = new List<BookBorrowingRequestDetailsDtoCreate>
                {
                    new BookBorrowingRequestDetailsDtoCreate { BookId = Guid.NewGuid() }
                }
            };

            var requestEntity = new BookBorrowingRequest();
            _mapperMock.Setup(m => m.Map<BookBorrowingRequest>(requestDto)).Returns(requestEntity);
            _bookBorrowingRequestRepositoryMock.Setup(r => r.AddAsync(It.IsAny<BookBorrowingRequest>())).ReturnsAsync(requestEntity);
            _bookBorrowingRequestRepositoryMock.Setup(r => r.GetQuery())
                .Returns(new TestAsyncEnumerable<BookBorrowingRequest>(new List<BookBorrowingRequest>()).AsQueryable());
            _bookBorrowingRequestRepositoryMock.Setup(r => r.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _bookBorrowingRequestRepositoryMock.Setup(r => r.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            await _service.CreateBookBorrowingRequestAsync(requestDto);

            // Assert
            _bookBorrowingRequestRepositoryMock.Verify(r => r.BeginTransactionAsync(), Times.Once);
            _bookBorrowingRequestRepositoryMock.Verify(r => r.CommitTransactionAsync(), Times.Once);
            _bookServiceMock.Verify(s => s.UpdateBookQuantityAsync(It.IsAny<Guid>(), -1), Times.Once);
        }

        [Test]
        public void CreateBookBorrowingRequestAsync_ExceedsMonthlyLimit_ShouldThrowException()
        {
            // Arrange
            var requestDto = new BookBorrowingRequestDtoCreate
            {
                RequestorId = Guid.NewGuid(),
                BookBorrowingRequestDetails = new List<BookBorrowingRequestDetailsDtoCreate>
            {
                new BookBorrowingRequestDetailsDtoCreate { BookId = Guid.NewGuid() }
            }
            };

            _bookBorrowingRequestRepositoryMock.Setup(r => r.GetQuery())
            .Returns(new TestAsyncEnumerable<BookBorrowingRequest>(new List<BookBorrowingRequest>
            {
                new() { RequestorId = requestDto.RequestorId, DateRequest = DateTime.Now.AddDays(-1) },
                new() { RequestorId = requestDto.RequestorId, DateRequest = DateTime.Now.AddDays(-2) },
                new() { RequestorId = requestDto.RequestorId, DateRequest = DateTime.Now.AddDays(-3) }
            }).AsQueryable());

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateBookBorrowingRequestAsync(requestDto));
        }

        [Test]
        public async Task DeleteBookBorrowingRequestAsync_ValidId_ShouldDeleteSuccessfully()
        {
            // Arrange
            var requestId = 1;
            var request = new BookBorrowingRequest { Id = requestId };
            _bookBorrowingRequestRepositoryMock.Setup(r => r.GetByIdAsync(requestId)).ReturnsAsync(request);

            // Act
            await _service.DeleteBookBorrowingRequestAsync(requestId);

            // Assert
            _bookBorrowingRequestRepositoryMock.Verify(r => r.DeleteAsync(request), Times.Once);
            _bookBorrowingRequestRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void DeleteBookBorrowingRequestAsync_InvalidId_ShouldThrowException()
        {
            // Arrange
            var requestId = 1;
            _bookBorrowingRequestRepositoryMock.Setup(r => r.GetByIdAsync(requestId)).ReturnsAsync((BookBorrowingRequest)null);

            // Act & Assert
            Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.DeleteBookBorrowingRequestAsync(requestId));
        }

        [Test]
        public async Task GetBookBorrowingRequestByIdAsync_ValidId_ShouldReturnRequest()
        {
            // Arrange
            var requestId = 1;
            var request = new BookBorrowingRequest { Id = requestId };
            _bookBorrowingRequestRepositoryMock.Setup(r => r.GetQuery())
                .Returns(new TestAsyncEnumerable<BookBorrowingRequest>(new List<BookBorrowingRequest> { request }).AsQueryable());

            _mapperMock.Setup(m => m.Map<BookBorrowingRequestResponse>(request)).Returns(new BookBorrowingRequestResponse());

            // Act
            var result = await _service.GetBookBorrowingRequestByIdAsync(requestId);

            // Assert
            Assert.IsNotNull(result);
            _mapperMock.Verify(m => m.Map<BookBorrowingRequestResponse>(request), Times.Once);
        }

        [Test]
        public void GetBookBorrowingRequestByIdAsync_InvalidId_ShouldThrowException()
        {
            // Arrange
            var requestId = 1;
            _bookBorrowingRequestRepositoryMock.Setup(r => r.GetQuery())
                .Returns(new TestAsyncEnumerable<BookBorrowingRequest>(new List<BookBorrowingRequest>()).AsQueryable());

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetBookBorrowingRequestByIdAsync(requestId));
        }

        [Test]
        public async Task UpdateBookBorrowingRequestStatusAsync_ValidRequest_ShouldUpdateSuccessfully()
        {
            // Arrange
            var requestId = 1;
            var requestorId = Guid.NewGuid();
            var approverId = Guid.NewGuid();
            var bookId = Guid.NewGuid();

            var request = new BookBorrowingRequest
            {
                Id = requestId,
                RequestorId = requestorId,
                BookBorrowingRequestDetails = new List<BookBorrowingRequestDetails>
            {
                new BookBorrowingRequestDetails { BookId = bookId }
            }
            };

            var updateStatusDto = new BookBorrowingRequestDtoUpdateStatus
            {
                Id = requestId,
                Status = Status.Rejected,
                ApproverId = approverId
            };

            var requestor = new UserDto { Id = requestorId, UserName = "RequestorUser", Email = "requestor@example.com" };
            var approver = new UserDto { Id = approverId, UserName = "ApproverUser" };

            _bookBorrowingRequestRepositoryMock.Setup(r => r.GetQuery())
            .Returns(new TestAsyncEnumerable<BookBorrowingRequest>(new List<BookBorrowingRequest> { request }).AsQueryable());
            _userServiceMock.Setup(s => s.GetUserByIdAsync(requestorId)).ReturnsAsync(requestor);
            _userServiceMock.Setup(s => s.GetUserByIdAsync(approverId)).ReturnsAsync(approver);
            _bookBorrowingRequestRepositoryMock.Setup(r => r.UpdateAsync(request)).Returns(Task.CompletedTask);
            _bookBorrowingRequestRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
            _bookServiceMock.Setup(s => s.UpdateBookQuantityAsync(bookId, 1)).Returns(Task.CompletedTask);
            _sendEmailServiceMock.Setup(s => s.SendEmalWithBodyAsync(
            It.Is<string>(email => email == requestor.Email),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<EmailBodyModel>())).Returns(Task.CompletedTask);

            // Act
            await _service.UpdateBookBorrowingRequestStatusAsync(updateStatusDto);

            // Assert
            _bookBorrowingRequestRepositoryMock.Verify(r => r.UpdateAsync(request), Times.Once);
            _bookBorrowingRequestRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            _bookServiceMock.Verify(s => s.UpdateBookQuantityAsync(bookId, 1), Times.Once);
            _sendEmailServiceMock.Verify(s => s.SendEmalWithBodyAsync(
            requestor.Email,
            It.Is<string>(subject => subject.Contains("Rejected")),
            It.IsAny<string>(),
            It.Is<EmailBodyModel>(body => body.Status == "Rejected" && body.UserName == requestor.UserName)), Times.Once);
        }

        [Test]
        public async Task GetAllBookBorrowingRequestsAsync_ValidStatus_ShouldReturnRequests()
        {
            // Arrange
            var status = Status.Waiting;
            var pageIndex = 1;
            var pageSize = 10;
            var requests = new List<BookBorrowingRequest>
            {
            new BookBorrowingRequest
            {
                Id = 1,
                RequestorId = Guid.NewGuid(),
                ApproverId = Guid.NewGuid(),
                Status = status,
                DateRequest = DateTime.Now,
                DateExpired = DateTime.Now.AddDays(30),
                BookBorrowingRequestDetails = new List<BookBorrowingRequestDetails>()
            },
            new BookBorrowingRequest
            {
                Id = 2,
                RequestorId = Guid.NewGuid(),
                ApproverId = null,
                Status = status,
                DateRequest = DateTime.Now,
                DateExpired = DateTime.Now.AddDays(30),
                BookBorrowingRequestDetails = new List<BookBorrowingRequestDetails>()
            }
            };

            
            var queryableResult = requests.Count;
            _bookBorrowingRequestRepositoryMock
            .Setup(r => r.GetPagedBookBorrowingRequestsAsync(It.IsAny<BookBorrowingRequestPagingSpecification>()))
            .ReturnsAsync((requests, queryableResult));

            _userServiceMock
            .Setup(s => s.GetUserByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => new UserDto { Id = id, UserName = $"User_{id}" });

            _mapperMock
            .Setup(m => m.Map<IEnumerable<BookBorrowingRequestDetailsDto>>(It.IsAny<IEnumerable<BookBorrowingRequestDetails>>()))
            .Returns(new List<BookBorrowingRequestDetailsDto>());

            // Act
            var result = await _service.GetAllBookBorrowingRequestsAsync((int)status, pageIndex, pageSize);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Items.Count, Is.EqualTo(2));
            Assert.That(result.Items[0].RequestName, Is.EqualTo("User_" + requests[0].RequestorId));
            Assert.That(result.Items[0].ApproveName, Is.EqualTo("User_" + requests[0].ApproverId));
            Assert.That(result.Items[1].RequestName, Is.EqualTo("User_" + requests[1].RequestorId));
            Assert.IsNull(result.Items[1].ApproveName);
            _bookBorrowingRequestRepositoryMock.Verify(r => r.GetPagedBookBorrowingRequestsAsync(It.IsAny<BookBorrowingRequestPagingSpecification>()), Times.Once);
            _userServiceMock.Verify(s => s.GetUserByIdAsync(It.IsAny<Guid>()), Times.Exactly(3));
            _mapperMock.Verify(m => m.Map<IEnumerable<BookBorrowingRequestDetailsDto>>(It.IsAny<IEnumerable<BookBorrowingRequestDetails>>()), Times.Exactly(2));
        }

        [Test]
        public async Task GetBookBorrowingRequestsByUserIdAsync_ValidUserId_ShouldReturnRequests()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var pageIndex = 1;
            var pageSize = 10;
            var approverId = Guid.NewGuid();
            var requests = new List<BookBorrowingRequest>
        {
        new BookBorrowingRequest { Id = 1, RequestorId = userId, Status = Status.Waiting,ApproverId = null },
        new BookBorrowingRequest { Id = 2, RequestorId = userId, Status = Status.Approved, ApproverId = approverId },
        };

            var paginatedResult = new PaginatedList<BookBorrowingRequest>(requests, requests.Count, pageIndex, pageSize);
            _bookBorrowingRequestRepositoryMock.Setup(r => r.GetPagedBookBorrowingRequestsAsync(It.IsAny<BookBorrowingRequestPagingSpecification>()))
            .ReturnsAsync((requests, requests.Count));
            _userServiceMock.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(new UserDto { Id = userId, UserName = "TestUser" });
            _userServiceMock.Setup(x => x.GetUserByIdAsync(approverId)).ReturnsAsync(new UserDto { Id = userId, UserName = "TestUser" });
            _mapperMock.Setup(m => m.Map<IEnumerable<BookBorrowingRequestResponse>>(requests))
            .Returns(new List<BookBorrowingRequestResponse>
            {
            new BookBorrowingRequestResponse { Id = 1 },
            new BookBorrowingRequestResponse { Id = 2 }
            });

            // Act
            var result = await _service.GetBookBorrowingRequestsByUserIdAsync(userId, pageIndex, pageSize);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Items.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task UpdateBookBorrowingRequestAsync_ValidRequest_ShouldUpdateSuccessfully()
        {
            // Arrange
            var requestDto = new BookBorrowingRequestDtoUpdate
            {
                Id = 1,
                Status = Status.Approved
            };

            var existingRequest = new BookBorrowingRequest { Id = 1 };
            _bookBorrowingRequestRepositoryMock.Setup(r => r.GetByIdAsync(requestDto.Id)).ReturnsAsync(existingRequest);
            _bookBorrowingRequestRepositoryMock.Setup(r => r.UpdateAsync(existingRequest)).Returns(Task.CompletedTask);
            _bookBorrowingRequestRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _service.UpdateBookBorrowingRequestAsync(requestDto);

            // Assert
            _bookBorrowingRequestRepositoryMock.Verify(r => r.UpdateAsync(existingRequest), Times.Once);
            _bookBorrowingRequestRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void UpdateBookBorrowingRequestAsync_InvalidId_ShouldThrowException()
        {
            // Arrange
            var requestDto = new BookBorrowingRequestDtoUpdate { Id = 1 };
            _bookBorrowingRequestRepositoryMock.Setup(r => r.GetByIdAsync(requestDto.Id)).ReturnsAsync((BookBorrowingRequest)null);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateBookBorrowingRequestAsync(requestDto));
        }

        [Test]
        public async Task GetCurrentMonthlyRequetsCountAsync_ValidUserId_ShouldReturnCount()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var requests = new List<BookBorrowingRequest>
        {
        new BookBorrowingRequest { RequestorId = userId, DateRequest = DateTime.Now.AddDays(-1) },
        new BookBorrowingRequest { RequestorId = userId, DateRequest = DateTime.Now.AddDays(-2) }
        };

            _bookBorrowingRequestRepositoryMock.Setup(r => r.GetQuery())
            .Returns(new TestAsyncEnumerable<BookBorrowingRequest>(requests).AsQueryable());

            // Act
            var result = await _service.GetCurrentMonthlyRequetsCountAsync(userId);

            // Assert
            Assert.That(result, Is.EqualTo(2));
        }
    }
}