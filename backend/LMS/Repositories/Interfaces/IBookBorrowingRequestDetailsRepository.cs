using LMS.Models.Enitties;
using LMS.Repositories.Base;

namespace LMS.Repositories.Interfaces
{
    public interface IBookBorrowingRequestDetailsRepository : IBaseRepository<BookBorrowingRequestDetails>
    {
        Task CreateBulkAsync(IEnumerable<BookBorrowingRequestDetails> bookBorrowingRequestDetails);
    }
}
