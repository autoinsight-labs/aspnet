using AutoInsightAPI.Dtos;
using FluentValidation;

namespace AutoInsightAPI.Validators
{
    public class CreateYardVehicleDtoValidator : AbstractValidator<CreateYardVehicleDto>
    {
        public CreateYardVehicleDtoValidator()
        {
            RuleFor(x => x.Status).IsInEnum().WithMessage("Status must be a valid value.");
            RuleFor(x => x.EnteredAt).NotNull().WithMessage("EnteredAt is required.");
            
            // Deve ter VehicleId OU Vehicle, mas não ambos
            RuleFor(x => x)
                .Must(x => !string.IsNullOrEmpty(x.VehicleId) ^ (x.Vehicle != null))
                .WithMessage("Either VehicleId or Vehicle must be provided, but not both.");

            // Se Vehicle for fornecido, valida seus campos usando o validador específico
            When(x => x.Vehicle != null, () => {
                RuleFor(x => x.Vehicle!).SetValidator(new CreateVehicleDtoValidator());
            });

            // Se VehicleId for fornecido, valida que não está vazio
            When(x => !string.IsNullOrEmpty(x.VehicleId), () => {
                RuleFor(x => x.VehicleId).NotEmpty().WithMessage("VehicleId cannot be empty when provided.");
            });
        }
    }
}
