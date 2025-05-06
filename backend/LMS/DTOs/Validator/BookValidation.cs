using FluentValidation;

namespace LMS.DTOs.Validator
{
    public class BookValidation : AbstractValidator<BookRequest>
    {
        public BookValidation()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .Length(3, 100).WithMessage("Title must be between 3 and 100 characters long.");
            RuleFor(x => x.Author)
                .NotEmpty().WithMessage("Author is required.")
                .Length(3, 50).WithMessage("Author must be between 3 and 50 characters long.");
            RuleFor(x => x.PublishedDate)
                .NotEmpty().WithMessage("Published date is required.")
                .LessThan(DateTime.Now).WithMessage("Published date must be in the past.");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .Length(5, 500).WithMessage("Description must be between 10 and 500 characters long.");
            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category ID is required.");
            RuleFor(x => x.Quantity)
                .NotEmpty().WithMessage("Quantity is required.")
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
            RuleFor(x => x.Available)
                .NotEmpty().WithMessage("Available is required.")
                .GreaterThanOrEqualTo(0).WithMessage("Available must be greater than or equal to 0.")
                .LessThanOrEqualTo(x => x.Quantity).WithMessage("Available must be less than or equal to Quantity.");
            RuleFor(x => x.PublishedDate)
                .NotEmpty().WithMessage("Published date is required.")
                .LessThan(DateTime.Now).WithMessage("Published date must be in the past.");
            

        }
    }
}