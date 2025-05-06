using FluentValidation;

namespace LMS.DTOs.Validator
{
    public class CategoryValidation : AbstractValidator<CategoryCreatDto>
    {
        public CategoryValidation()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .Length(3, 50).WithMessage("Name must be between 3 and 50 characters long.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .Length(1, 200).WithMessage("Description must be between 1 and 200 characters long.");
        }
    }
}
