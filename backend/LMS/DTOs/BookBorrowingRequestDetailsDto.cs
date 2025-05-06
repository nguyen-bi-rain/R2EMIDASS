namespace LMS.DTOs
{
    public class BookBorrowingRequestDetailsDto
    {
        public int BookBorrowingRequestId { get; set; }
        public Guid BookId { get; set; }
        public BookDto Book { get; set; }
    }
    public class BookBorrowingRequestDetailsDtoCreate
    {
        public int BookBorrowingRequestId { get; set; }
        public Guid BookId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
    public class BookBorrowingRequestDetailsDtoUpdate
    {
        public int Id { get; set; }
        public Guid BookId { get; set; }
        
    }
}