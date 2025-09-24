using AutoInsightAPI.Dtos;
using FluentValidation;

namespace AutoInsightAPI.Validators
{
    public class CreateYardEmployeeDtoValidator : AbstractValidator<CreateYardEmployeeDto>
    {
        public CreateYardEmployeeDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("A valid email is required.");
            RuleFor(x => x.Role).IsInEnum().WithMessage("Role must be a valid value.");
        }
    }
}
