using LMS.Data;
using LMS.Models.Enitties;
using LMS.Repositories.Base;
using LMS.Repositories.Interfaces;
using LMS.Specifications;
using Microsoft.EntityFrameworkCore;

namespace LMS.Repositories.Implements
{
    public class BookBorrowingRequestRepository : BaseRepository<BookBorrowingRequest>, IBookBorrowingRequestRepository
    {
        public BookBorrowingRequestRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<(IEnumerable<BookBorrowingRequest> query,int totalCount)> GetPagedBookBorrowingRequestsAsync(ISpecification<BookBorrowingRequest> spec)
        {
            var result = SpecificationEvaluator<BookBorrowingRequest>.GetQueryWithCount(_context.Set<BookBorrowingRequest>().AsQueryable(), spec);
            return (await result.Query.Include(x => x.BookBorrowingRequestDetails).ThenInclude(x => x.Book).ToListAsync(), result.TotalCount);
        }
    }
}
