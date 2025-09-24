using AutoInsightAPI.Dtos;
using FluentValidation;

namespace AutoInsightAPI.Validators
{
    public class CreateYardVehicleDtoValidator : AbstractValidator<CreateYardVehicleDto>
    {
        public CreateYardVehicleDtoValidator()
        {
            RuleFor(x => x.Status).IsInEnum().WithMessage("Status must be a valid value.");
            
            RuleFor(x => x)
                .Must(x => !string.IsNullOrEmpty(x.VehicleId) ^ (x.Vehicle != null))
                .WithMessage("Either VehicleId or Vehicle must be provided, but not both.");

            When(x => x.Vehicle != null, () => {
                RuleFor(x => x.Vehicle!).SetValidator(new CreateVehicleDtoValidator());
            });

            When(x => !string.IsNullOrEmpty(x.VehicleId), () => {
                RuleFor(x => x.VehicleId).NotEmpty().WithMessage("VehicleId cannot be empty when provided.");
            });
        }
    }
}
