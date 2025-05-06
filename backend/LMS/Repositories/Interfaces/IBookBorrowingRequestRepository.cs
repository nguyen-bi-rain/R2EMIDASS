using LMS.Models.Enitties;
using LMS.Repositories.Base;
using LMS.Specifications;

namespace LMS.Repositories.Interfaces
{
    public interface IBookBorrowingRequestRepository : IBaseRepository<BookBorrowingRequest>
    {
        Task<(IEnumerable<BookBorrowingRequest> query ,int totalCount)> GetPagedBookBorrowingRequestsAsync(ISpecification<BookBorrowingRequest> spec);
    }
}
