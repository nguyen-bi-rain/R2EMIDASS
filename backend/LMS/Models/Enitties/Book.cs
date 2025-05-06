using System.ComponentModel.DataAnnotations;
using LMS.Models.Enitties.Base;

namespace LMS.Models.Enitties;
public class Book : BaseEntity<Guid>
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }
    public int Available { get; set; }
    public DateTime PublishedDate { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public ICollection<BookBorrowingRequestDetails> BookBorrowingRequestDetails { get; set; }

    // Add this property for concurrency handling
    [Timestamp]
    public byte[] RowVersion { get; set; }
}
