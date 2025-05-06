using System.ComponentModel.DataAnnotations.Schema;
using LMS.Models.Enitties.Base;
using LMS.Models.Enums;

namespace LMS.Models.Enitties
{
    public class BookBorrowingRequest : BaseEntity<int>
    {
        [ForeignKey(nameof(Requestor))]
        public Guid RequestorId { get; set; }
        [ForeignKey(nameof(Approver))]

        public Guid? ApproverId { get; set; }
        public User Requestor { get; set; }
        public User Approver { get; set; }
        public DateTime DateRequest { get; set; }
        public Status? Status { get; set; } 
        public DateTime DateExpired { get; set; }
        public ICollection<BookBorrowingRequestDetails> BookBorrowingRequestDetails { get; set; }
    }
}
