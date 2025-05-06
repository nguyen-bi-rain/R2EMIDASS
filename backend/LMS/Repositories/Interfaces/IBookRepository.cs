using LMS.Models.Enitties;
using LMS.Repositories.Base;
using LMS.Specifications;

namespace LMS.Repositories.Interfaces
{
    public interface IBookRepository : IBaseRepository<Book>
    {
        Task<(IEnumerable<Book> Books, int TotalCount)> GetBookWithQueryAsync(ISpecification<Book> spec);
    }
}
