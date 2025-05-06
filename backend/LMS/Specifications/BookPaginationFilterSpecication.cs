using LMS.Models.Enitties;

namespace LMS.Specifications
{
    public class BookPaginationFilterSpecication : BaseSpecification<Book>
    {
        public  BookPaginationFilterSpecication(string? searchTerm,int pageIndex,int pageSize,int categoryId) 
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                Criteria = b => b.Title.ToLower().Contains(searchTerm.ToLower());
            }
            if(categoryId > 0){
                Criteria = b => b.CategoryId == categoryId;
            }

            ApplyPaging(pageIndex, pageSize);
            AddInclude(b => b.Category);
        }
    }
}
