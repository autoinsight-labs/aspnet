using AutoInsightAPI.Dtos;
using FluentValidation;

namespace AutoInsightAPI.Validators
{
    public class CreateModelDtoValidator : AbstractValidator<CreateModelDto>
    {
        public CreateModelDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.Year).GreaterThan(1900).WithMessage("Year must be greater than 1900.");
        }
    }
}
