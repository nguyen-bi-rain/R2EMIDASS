using LMS.Data;
using LMS.Models.Enitties;
using LMS.Repositories.Base;
using LMS.Repositories.Interfaces;

namespace LMS.Repositories.Implements
{
    public class BookBorrowingRequestDetailsRepository : BaseRepository<BookBorrowingRequestDetails>, IBookBorrowingRequestDetailsRepository
    {
        public BookBorrowingRequestDetailsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async  Task CreateBulkAsync(IEnumerable<BookBorrowingRequestDetails> bookBorrowingRequestDetails)
        {
            await _context.BookBorrowingRequestDetails.AddRangeAsync(bookBorrowingRequestDetails);
        }
    }


}
