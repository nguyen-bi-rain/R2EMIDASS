using LMS.Data;
using LMS.Models.Enitties;
using LMS.Repositories.Base;
using LMS.Repositories.Interfaces;
using LMS.Specifications;
using Microsoft.EntityFrameworkCore;

namespace LMS.Repositories.Implements
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<(IEnumerable<Category> categories,int totalCount)> GetAllCategoriesAsync(ISpecification<Category> spec)
        {
            var result = SpecificationEvaluator<Category>.GetQueryWithCount(_context.Set<Category>().AsQueryable(), spec);
            return (await result.Query.ToListAsync(), result.TotalCount);
        }
    }
}