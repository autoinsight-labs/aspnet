using AutoInsightAPI.Dtos;
using FluentValidation;

namespace AutoInsightAPI.Validators
{
    public class YardEmployeeDtoValidator : AbstractValidator<YardEmployeeDto>
    {
        public YardEmployeeDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");
            RuleFor(x => x.Role).IsInEnum().WithMessage("Role must be a valid value.");
        }
    }
}
