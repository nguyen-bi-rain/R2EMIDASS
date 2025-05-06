using LMS.Models.Enitties;

namespace LMS.Specifications
{
    public class CategorySpecification : BaseSpecification<Category>
    {
        public CategorySpecification(int pageNumber, int pageSize){
            ApplyPaging(pageNumber, pageSize);
            AddOrderBy(x => x.Name);
        }
    }
}