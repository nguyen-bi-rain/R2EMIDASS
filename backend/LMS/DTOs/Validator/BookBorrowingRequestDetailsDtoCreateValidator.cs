using FluentValidation;

namespace LMS.DTOs.Validator
{
    public class BookBorrowingRequestDetailsDtoCreateValidator : AbstractValidator<BookBorrowingRequestDetailsDtoCreate>
    {
        public BookBorrowingRequestDetailsDtoCreateValidator()
        {
            RuleFor(x => x.BookId).NotEmpty().WithMessage("BookId is required.");
        }
    }
}