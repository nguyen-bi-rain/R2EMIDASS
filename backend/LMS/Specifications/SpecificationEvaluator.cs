using Microsoft.EntityFrameworkCore;

namespace LMS.Specifications
{
    public class SpecificationEvaluator<T> where T : class
    {
        public static (IQueryable<T> Query, int TotalCount) GetQueryWithCount(IQueryable<T> inputQuery, ISpecification<T> spec)
        {
            IQueryable<T> query = inputQuery;

            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);

            int totalCount = query.Count();

            if (spec.OrderBy != null)
                query = query.OrderBy(spec.OrderBy);

            if (spec.OrderByDescending != null)
                query = query.OrderByDescending(spec.OrderByDescending);

            if (spec.IsPagingEnabled)
            {
                if (spec.Take.Value != -1)
                {

                    query = query.Skip((spec.Skip.Value - 1) * spec.Take.Value).Take(spec.Take.Value);
                }
            }

            if (spec.Includes != null)
            {
                foreach (var include in spec.Includes)
                {
                    query = query.Include(include);
                }
            }
            

            return (query, totalCount);
        }
    }

}