using LMS.Models.Enums;

namespace LMS.DTOs
{
    public class BookBorrowingRequestDto
    {
        public Guid RequestorId { get; set; }
        public Guid ApproverId { get; set; }
        public DateTime DateRequest { get; set; }
        public Status Status { get; set; }
        public DateTime DateExpired { get; set; }
        public List<BookBorrowingRequestDetailsDto> BookBorrowingRequestDetails { get; set; }
    }
    public class BookBorrowingRequestDtoCreate
    {
        public Guid RequestorId { get; set; }    
        public List<BookBorrowingRequestDetailsDtoCreate> BookBorrowingRequestDetails { get; set; }

    }
    public class BookBorrowingRequestDtoUpdate
    {
        public int Id { get; set; }
        public Guid RequestorId { get; set; }
        public Guid ApproverId { get; set; }
        public DateTime DateRequest { get; set; }
        public Status Status { get; set; }
    }
    
    public class BookBorrowingRequestResponse
    {
        public int Id { get; set; }
        public string RequestName { get; set; }
        public string ApproveName { get; set; }
        public DateTime DateRequest { get; set; }
        public Status? Status { get; set; }
        public DateTime DateExpired { get; set; }
        public DateTime UpdatedAt { get; set; }
        public IEnumerable<BookBorrowingRequestDetailsDto> BookBorrowingRequestDetails { get; set; }
    }

    public class BookBorrowingRequestDtoUpdateStatus
    {
        public int Id { get; set; }
        public Status Status { get; set; }
        public Guid ApproverId { get; set; }
    }
}