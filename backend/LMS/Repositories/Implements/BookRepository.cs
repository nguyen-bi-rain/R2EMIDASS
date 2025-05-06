using LMS.Data;
using LMS.Models.Enitties;
using LMS.Repositories.Base;
using LMS.Repositories.Interfaces;
using LMS.Specifications;
using Microsoft.EntityFrameworkCore;

namespace LMS.Repositories.Implements
{
    public class BookRepository : BaseRepository<Book>,IBookRepository
    {
        public BookRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<(IEnumerable<Book> Books, int TotalCount)> GetBookWithQueryAsync(ISpecification<Book> spec)
        {
            var result = SpecificationEvaluator<Book>.GetQueryWithCount(_context.Set<Book>().AsQueryable(), spec);
            return (await result.Query.ToListAsync(), result.TotalCount);
        }
    }
}
