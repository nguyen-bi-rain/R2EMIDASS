using LMS.Models.Enitties.Base;

namespace LMS.Models.Enitties
{
    public class Category : BaseEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; } 
        public virtual ICollection<Book> Books { get; set; }
    }
}
