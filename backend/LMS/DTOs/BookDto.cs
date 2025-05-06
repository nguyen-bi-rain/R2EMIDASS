using System.ComponentModel.DataAnnotations;

namespace LMS.DTOs
{
    public class BookResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }

        public int Quantity { get; set; }
        public int Available { get; set; }
        public DateTime PublishedDate { get; set; }
        public string CategoryName { get; set; }
    }
    
    public class BookDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public int Available { get; set; }
        public DateTime PublishedDate { get; set; }
        public int CategoryId { get; set; }
    }

    public class BookRequest
    {
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        
        public string Description { get; set; }
        public int Quantity { get; set; }
        public int Available { get; set; }
        public DateTime PublishedDate { get; set; }
        public int CategoryId { get; set; }
    }
    public class BookUpdateRequest
    {
        public string Title { get; set; }
        public string Author { get; set; }
        
        public string Description { get; set; }
        public int Quantity { get; set; }
        public DateTime PublishedDate { get; set; }
        public int CategoryId { get; set; }
    }
}