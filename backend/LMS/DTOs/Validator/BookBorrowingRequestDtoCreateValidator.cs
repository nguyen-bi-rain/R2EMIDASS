using FluentValidation;

namespace LMS.DTOs.Validator
{ 
    public class BookBorrowingRequestDtoCreateValidator : AbstractValidator<BookBorrowingRequestDtoCreate>
    {
        public BookBorrowingRequestDtoCreateValidator()
        {
            RuleFor(x => x.RequestorId).NotEmpty().WithMessage("RequestorId is required.");
            RuleFor(x => x.BookBorrowingRequestDetails).NotEmpty().WithMessage("BookBorrowingRequestDetails is required.");
            RuleForEach(x => x.BookBorrowingRequestDetails).SetValidator(new BookBorrowingRequestDetailsDtoCreateValidator());
        }
    }
    
}