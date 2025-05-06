using LMS.DTOs;
using LMS.DTOs.Shared;

namespace LMS.Services.Interfaces
{
    public interface IBookService
    {
        Task<PaginatedList<BookResponse>> GetAllBookAsync(int pageSize = 10, int pageIndex = 1, string? searchTerm = null, int categoryId = 0);
        Task<BookDto> GetBookByIdAsync(Guid id);
        Task AddBookAsync(BookRequest  bookCreateDto);
        Task UpdateBookAsync(Guid id, BookUpdateRequest bookUpdateDto);
        Task DeleteBookAsync(Guid id);
        Task UpdateBookQuantityAsync(Guid bookId,int quantity);

    }
}
