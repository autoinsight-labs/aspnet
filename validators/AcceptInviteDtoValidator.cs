using AutoInsightAPI.Dtos;
using FluentValidation;

namespace AutoInsightAPI.Validators
{
    public class AcceptInviteDtoValidator : AbstractValidator<AcceptInviteDto>
    {
        public AcceptInviteDtoValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required to accept the invite.");
        }
    }
}
