using LMS.Models.Enitties;
using LMS.Repositories.Base;
using LMS.Specifications;

namespace LMS.Repositories.Interfaces
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<(IEnumerable<Category> categories,int totalCount)> GetAllCategoriesAsync(ISpecification<Category> spec);
    }
    
}