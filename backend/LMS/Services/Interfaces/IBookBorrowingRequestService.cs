using LMS.DTOs;
using LMS.DTOs.Shared;

namespace LMS.Services.Interfaces
{
    public interface IBookBorrowingRequestService
    {
        Task CreateBookBorrowingRequestAsync(BookBorrowingRequestDtoCreate bookBorrowingRequest);
        Task UpdateBookBorrowingRequestAsync(BookBorrowingRequestDtoUpdate bookBorrowingRequest);
        Task DeleteBookBorrowingRequestAsync(int id);
        Task<BookBorrowingRequestResponse> GetBookBorrowingRequestByIdAsync(int id);
        Task<PaginatedList<BookBorrowingRequestResponse>> GetAllBookBorrowingRequestsAsync(int status ,int pageIndex=1, int pageSize=10);
        Task<PaginatedList<BookBorrowingRequestResponse>> GetBookBorrowingRequestsByUserIdAsync(Guid userId, int pageIndex = 1, int pageSize = 10, int status = -1);
        Task UpdateBookBorrowingRequestStatusAsync(BookBorrowingRequestDtoUpdateStatus bookBorrowingRequestStatus);
        Task<int> GetCurrentMonthlyRequetsCountAsync(Guid userId);

    }
}
