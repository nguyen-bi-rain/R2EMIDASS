using AutoMapper;
using FluentValidation;
using LMS.DTOs;
using LMS.DTOs.Shared;
using LMS.Exceptions;
using LMS.Models.Enitties;
using LMS.Repositories.Interfaces;
using LMS.Services.Implements;
using LMS.Specifications;
using Microsoft.EntityFrameworkCore;
using Moq;
using UnitTest.Common;


namespace LMS.Tests.Services
{
    [TestFixture]
    public class BookServiceTests
    {
        private Mock<IBookRepository> _bookRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IValidator<BookRequest>> _validatorMock;
        private BookService _bookService;

        [SetUp]
        public void SetUp()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _mapperMock = new Mock<IMapper>();
            _validatorMock = new Mock<IValidator<BookRequest>>();
            _bookService = new BookService(_bookRepositoryMock.Object, _mapperMock.Object, _validatorMock.Object);
        }

        [Test]
        public async Task GetAllBookAsync_ValidParameters_ReturnsPaginatedList()
        {
            // Arrange
            var pageSize = 10;
            var pageIndex = 1;
            var searchTerm = "test";
            var categoryId = 0;
            var books = new List<Book> { new Book { Id = Guid.NewGuid(), Title = "Test Book" } };
            var specResult = new PaginatedList<Book>(books, books.Count, pageIndex, pageSize);
            var bookResponses = new List<BookResponse> { new BookResponse { Id = books[0].Id, Title = "Test Book" } };
            _bookRepositoryMock.Setup(r => r.GetBookWithQueryAsync(It.IsAny<BookPaginationFilterSpecication>()))
                .ReturnsAsync((books, books.Count));
            _mapperMock.Setup(m => m.Map<IEnumerable<BookResponse>>(books)).Returns(bookResponses);

            // Act
            var result = await _bookService.GetAllBookAsync(pageSize, pageIndex, searchTerm, categoryId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<PaginatedList<BookResponse>>(result);
            Assert.That(result.Items.Count, Is.EqualTo(bookResponses.Count));
            Assert.That(result.TotalCount, Is.EqualTo(specResult.TotalCount));
        }

        [Test]
        public void GetAllBookAsync_NoBooksFound_ThrowsResourceNotFoundException()
        {
            // Arrange
            var emptyBooks = new List<Book>();
            int totalCount = 0;
            _bookRepositoryMock.Setup(r => r.GetBookWithQueryAsync(It.IsAny<BookPaginationFilterSpecication>()))
                .ReturnsAsync((emptyBooks, totalCount));

            // Act & Assert
            Assert.ThrowsAsync<ResourceNotFoundException>(async () =>
                await _bookService.GetAllBookAsync());
        }

        [Test]
        public async Task GetBookByIdAsync_ValidId_ReturnsBookDto()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = new Book { Id = bookId, Title = "Test Book" };
            var bookDto = new BookDto { Id = bookId, Title = "Test Book" };
            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync(book);
            _mapperMock.Setup(m => m.Map<BookDto>(book)).Returns(bookDto);

            // Act
            var result = await _bookService.GetBookByIdAsync(bookId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BookDto>(result);
            Assert.That(result.Id, Is.EqualTo(bookDto.Id));
        }

        [Test]
        public void GetBookByIdAsync_InvalidId_ThrowsResourceNotFoundException()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync((Book)null);

            // Act & Assert
            Assert.ThrowsAsync<ResourceNotFoundException>(async () =>
                await _bookService.GetBookByIdAsync(bookId));
        }

        [Test]
        public async Task AddBookAsync_ValidRequest_CreatesBook()
        {
            // Arrange
            var bookRequest = new BookRequest
            {
                Title = "New Book",
                CategoryId = 1,
                Author = "Author Name",
                Description = "Book Description",
                Quantity = 5,
                Available = 5
            };
            var book = new Book { Id = Guid.NewGuid(), Title = "New Book" };

            // Create an async queryable mock
            var mockBooks = new List<Book>().AsQueryable();
            var asyncEnumerable = new TestAsyncEnumerable<Book>(mockBooks);
            _bookRepositoryMock.Setup(r => r.GetQuery()).Returns(asyncEnumerable);

            _validatorMock.Setup(v => v.ValidateAsync(bookRequest, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mapperMock.Setup(m => m.Map<Book>(bookRequest)).Returns(book);
            _bookRepositoryMock.Setup(r => r.AddAsync(book)).ReturnsAsync(book);
            _bookRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _bookService.AddBookAsync(bookRequest);

            // Assert
            _bookRepositoryMock.Verify(r => r.AddAsync(book), Times.Once());
            _bookRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once());
        }

        [Test]
        public void AddBookAsync_NullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            BookRequest bookRequest = null;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _bookService.AddBookAsync(bookRequest));
        }

        [Test]
        public async Task AddBookAsync_DuplicateTitle_ThrowsResourceUniqueException()
        {
            // Arrange
            var bookRequest = new BookRequest
            {
                Title = "Existing Book",
                CategoryId = 1,
                Author = "Test Author",
                Description = "Test Description",
                Quantity = 1,
                Available = 1
            };

            var existingBook = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Existing Book"
            };

            var mockDbSet = new Mock<DbSet<Book>>();
            var books = new List<Book> { existingBook }.AsQueryable();

            mockDbSet.As<IAsyncEnumerable<Book>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<Book>(books.GetEnumerator()));

            mockDbSet.As<IQueryable<Book>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Book>(books.Provider));

            mockDbSet.As<IQueryable<Book>>()
                .Setup(m => m.Expression).Returns(books.Expression);
            mockDbSet.As<IQueryable<Book>>()
                .Setup(m => m.ElementType).Returns(books.ElementType);
            mockDbSet.As<IQueryable<Book>>()
                .Setup(m => m.GetEnumerator()).Returns(books.GetEnumerator());

            _bookRepositoryMock.Setup(r => r.GetQuery())
                              .Returns(mockDbSet.Object);

            _validatorMock.Setup(v => v.ValidateAsync(bookRequest, default))
                         .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            // Act & Assert
            Assert.ThrowsAsync<ResourceUniqueException>(async () =>
                await _bookService.AddBookAsync(bookRequest));

            // Verify
            _bookRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Never());
            _bookRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never());
        }
        [Test]
        public async Task UpdateBookAsync_ValidRequest_UpdatesBook()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var bookRequest = new BookUpdateRequest { Title = "Updated Book" };
            var existingBook = new Book { Id = bookId, Title = "Original Book" };
            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync(existingBook);
            _bookRepositoryMock.Setup(r => r.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map(bookRequest, existingBook)).Returns(existingBook);
            _bookRepositoryMock.Setup(r => r.UpdateAsync(existingBook)).Returns(Task.CompletedTask);
            _bookRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
            _bookRepositoryMock.Setup(r => r.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            await _bookService.UpdateBookAsync(bookId, bookRequest);

            // Assert
            _bookRepositoryMock.Verify(r => r.UpdateAsync(existingBook), Times.Once());
            _bookRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once());
            _bookRepositoryMock.Verify(r => r.CommitTransactionAsync(), Times.Once());
        }

        [Test]
        public void UpdateBookAsync_NullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            BookUpdateRequest bookRequest = null;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _bookService.UpdateBookAsync(bookId, bookRequest));
        }

        [Test]
        public void UpdateBookAsync_BookNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var bookRequest = new BookUpdateRequest { Title = "Updated Book" };
            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync((Book)null);

            // Act & Assert
            Assert.ThrowsAsync<ResourceNotFoundException>(async () =>
                await _bookService.UpdateBookAsync(bookId, bookRequest));
        }

        [Test]
        public async Task DeleteBookAsync_ValidId_DeletesBook()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = new Book { Id = bookId, Available = 5, Quantity = 5 };
            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync(book);
            _bookRepositoryMock.Setup(r => r.DeleteAsync(book)).Returns(Task.CompletedTask);
            _bookRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _bookService.DeleteBookAsync(bookId);

            // Assert
            _bookRepositoryMock.Verify(r => r.DeleteAsync(book), Times.Once());
            _bookRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once());
        }

        [Test]
        public void DeleteBookAsync_BookNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync((Book)null);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _bookService.DeleteBookAsync(bookId));
        }

        [Test]
        public void DeleteBookAsync_BookBorrowed_ThrowsDatabaseBadRequestException()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = new Book { Id = bookId, Available = 3, Quantity = 5 };
            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync(book);

            // Act & Assert
            Assert.ThrowsAsync<DatabaseBadRequestException>(async () =>
                await _bookService.DeleteBookAsync(bookId));
        }

        [Test]
        public async Task UpdateBookQuantityAsync_ValidRequest_UpdatesQuantity()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var quantity = 2;
            var book = new Book { Id = bookId, Available = 3, Quantity = 5 };
            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync(book);
            _bookRepositoryMock.Setup(r => r.UpdateAsync(book)).Returns(Task.CompletedTask);
            _bookRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _bookService.UpdateBookQuantityAsync(bookId, quantity);

            // Assert
            Assert.That(book.Available, Is.EqualTo(5));
            _bookRepositoryMock.Verify(r => r.UpdateAsync(book), Times.Once());
            _bookRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once());
        }

        [Test]
        public void UpdateBookQuantityAsync_BookNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync((Book)null);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _bookService.UpdateBookQuantityAsync(bookId, 2));
        }

        [Test]
        public void UpdateBookQuantityAsync_BookNotAvailable_ThrowsDatabaseBadRequestException()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = new Book { Id = bookId, Available = 0, Quantity = 5 };
            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync(book);

            // Act & Assert
            Assert.ThrowsAsync<DatabaseBadRequestException>(async () =>
                await _bookService.UpdateBookQuantityAsync(bookId, 2));
        }
    }
}