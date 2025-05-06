using LMS.Models.Enitties.Base;

namespace LMS.Models.Enitties
{
    public class BookBorrowingRequestDetails : BaseEntity<int>
    {
        public int BookBorrowingRequestId { get; set; }
        public BookBorrowingRequest BookBorrowingRequest { get; set; }
        public Guid BookId { get; set; }
        public Book Book { get; set; }

    }
}
