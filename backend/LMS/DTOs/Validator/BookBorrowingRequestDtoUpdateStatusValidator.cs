using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace LMS.DTOs.Validator
{
    public class BookBorrowingRequestDtoUpdateStatusValidator : AbstractValidator<BookBorrowingRequestDtoUpdateStatus>
    {
        public BookBorrowingRequestDtoUpdateStatusValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
            RuleFor(x => x.Status).IsInEnum().WithMessage("Status is invalid.");
            RuleFor(x => x.ApproverId).NotEmpty().WithMessage("ApproverId is required.");
        }
    }
}