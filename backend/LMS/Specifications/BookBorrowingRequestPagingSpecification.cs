using LMS.Models.Enitties;
using LMS.Models.Enums;

namespace LMS.Specifications
{
    public class BookBorrowingRequestPagingSpecification : BaseSpecification<BookBorrowingRequest>
    {
        public BookBorrowingRequestPagingSpecification(int pageIndex, int pageSize,int status, Guid? userId)
        {
            if(userId != null )
            {
                Criteria = x => x.RequestorId == userId;
            }
            if(status >= 0 && status <= 2)
            {
                Criteria = x => x.Status == (Status)status;
            }

            AddOrderByDescending(x => x.DateRequest);   

            AddInclude(x => x.BookBorrowingRequestDetails);
            ApplyPaging(pageIndex, pageSize);
        }
    }
}