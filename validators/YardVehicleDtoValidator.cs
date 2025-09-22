using AutoInsightAPI.Dtos;
using FluentValidation;

namespace AutoInsightAPI.Validators
{
    public class YardVehicleDtoValidator : AbstractValidator<YardVehicleDto>
    {
        public YardVehicleDtoValidator()
        {
            RuleFor(x => x.Status).IsInEnum().WithMessage("Status must be a valid value.");
            RuleFor(x => x.EnteredAt).NotNull().WithMessage("EnteredAt is required.");
            RuleFor(x => x.Vehicle).NotNull().WithMessage("Vehicle is required.");
            RuleFor(x => x.Vehicle.Id).NotEmpty().When(x => x.Vehicle != null).WithMessage("Vehicle Id is required.");
        }
    }
}
