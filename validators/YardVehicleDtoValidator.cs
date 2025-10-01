using AutoInsightAPI.Dtos;
using FluentValidation;

namespace AutoInsightAPI.Validators
{
    public class YardVehicleDtoValidator : AbstractValidator<YardVehicleDto>
    {
        public YardVehicleDtoValidator()
        {
            RuleFor(x => x.Status).IsInEnum().WithMessage("Status must be a valid value.");
        }
    }
}
