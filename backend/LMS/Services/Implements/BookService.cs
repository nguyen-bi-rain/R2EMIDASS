using AutoMapper;
using FluentValidation;
using LMS.DTOs;
using LMS.DTOs.Shared;
using LMS.Exceptions;
using LMS.Models.Enitties;
using LMS.Repositories.Interfaces;
using LMS.Services.Interfaces;
using LMS.Specifications;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services.Implements
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<BookRequest> _validator;

        public BookService(IBookRepository bookRepository, IMapper mapper, IValidator<BookRequest> validator)
        {
            _validator = validator;
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedList<BookResponse>> GetAllBookAsync(int pageSize = 10, int pageIndex = 1, string? searchTerm = null, int categoryId = 0)
        {
            var spec = new BookPaginationFilterSpecication(searchTerm, pageIndex, pageSize, categoryId);
            var result = await _bookRepository.GetBookWithQueryAsync(spec);
            if (result.Books == null || !result.Books.Any())
            {
                throw new ResourceNotFoundException("No books found.");
            }
            var model = _mapper.Map<IEnumerable<BookResponse>>(result.Books);
            return new PaginatedList<BookResponse>(model.ToList(), result.TotalCount, pageIndex, pageSize);
        }


        public async Task<BookDto> GetBookByIdAsync(Guid id)
        {
            var book = await _bookRepository.GetByIdAsync(id)
                ?? throw new ResourceNotFoundException("Book not found.");
            return _mapper.Map<BookDto>(book);
        }

        public async Task AddBookAsync(BookRequest bookCreateDto)
        {
            if (bookCreateDto == null) throw new ArgumentNullException(nameof(bookCreateDto));

            if (await _bookRepository.GetQuery().AnyAsync(b => b.Title == bookCreateDto.Title))
            {
                throw new ResourceUniqueException("Book already exists.");
            }

            var validationResult = await _validator.ValidateAsync(bookCreateDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException("Validation failed.", validationResult.Errors);
            }
            var book = _mapper.Map<Book>(bookCreateDto);
            await _bookRepository.AddAsync(book);
            if (await _bookRepository.SaveChangesAsync() < 0)
            {
                throw new DatabaseBadRequestException("Failed to create book.");
            }
        }

        public async Task UpdateBookAsync(Guid id, BookUpdateRequest bookUpdateDto)
        {
            if (bookUpdateDto == null) throw new ArgumentNullException(nameof(bookUpdateDto));

            var existingBook = await _bookRepository.GetByIdAsync(id)
                ?? throw new ResourceNotFoundException("Book not found.");
            await _bookRepository.BeginTransactionAsync();
            try
            {
                var quantityChange = bookUpdateDto.Quantity - existingBook.Quantity;
                existingBook.Available = Math.Max(existingBook.Available + quantityChange, 0);
                _mapper.Map(bookUpdateDto, existingBook);
                await _bookRepository.UpdateAsync(existingBook);

                if (await _bookRepository.SaveChangesAsync() < 0)
                {
                    throw new DatabaseBadRequestException("Failed to update book.");
                }

                await _bookRepository.CommitTransactionAsync();
            }
            catch
            {
                await _bookRepository.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task DeleteBookAsync(Guid id)
        {
            var book = await _bookRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Book not found.");
            await _bookRepository.DeleteAsync(book);
            if (book.Available < book.Quantity)
            {
                throw new DatabaseBadRequestException("Cannot delete this book, it is currently borrowed.");
            }
            if (await _bookRepository.SaveChangesAsync() < 0)
            {
                throw new DatabaseBadRequestException("Failed to delete book.");
            }
        }

        public async Task UpdateBookQuantityAsync(Guid bookId,int quantity)
        {
            var book = await _bookRepository.GetByIdAsync(bookId)
                ?? throw new KeyNotFoundException("Book not found.");
            if (book.Available != 0 && book.Available <= book.Quantity)
            {
                book.Available += quantity;
                await _bookRepository.UpdateAsync(book);
            }
            else
            {
                throw new DatabaseBadRequestException("Book is not available.");
            }
            await _bookRepository.SaveChangesAsync();   
        }
    }
}
