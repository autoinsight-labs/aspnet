using AutoInsightAPI.Dtos;
using FluentValidation;

namespace AutoInsightAPI.Validators
{
    public class CreateYardDtoValidator : AbstractValidator<CreateYardDto>
    {
        public CreateYardDtoValidator()
        {
            RuleFor(x => x.OwnerId).NotEmpty().WithMessage("OwnerId is required.");
            RuleFor(x => x.Address).NotNull().WithMessage("Address is required.")
                .SetValidator(new AddressDtoValidator());
        }
    }
}
