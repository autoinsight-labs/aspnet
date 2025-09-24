using AutoInsightAPI.Dtos;
using FluentValidation;

namespace AutoInsightAPI.Validators
{
    public class CreateVehicleDtoValidator : AbstractValidator<CreateVehicleDto>
    {
        public CreateVehicleDtoValidator()
        {
            RuleFor(x => x.Plate).NotEmpty().WithMessage("Plate is required.");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");
            
            // Deve ter ModelId OU Model, mas não ambos
            RuleFor(x => x)
                .Must(x => !string.IsNullOrEmpty(x.ModelId) ^ (x.Model != null))
                .WithMessage("Either ModelId or Model must be provided, but not both.");

            // Se Model for fornecido, valida seus campos
            When(x => x.Model != null, () => {
                RuleFor(x => x.Model!.Name).NotEmpty().WithMessage("Model Name is required.");
                RuleFor(x => x.Model!.Year).GreaterThan(1900).WithMessage("Model Year must be greater than 1900.");
            });

            // Se ModelId for fornecido, valida que não está vazio
            When(x => !string.IsNullOrEmpty(x.ModelId), () => {
                RuleFor(x => x.ModelId).NotEmpty().WithMessage("ModelId cannot be empty when provided.");
            });
        }
    }
}
