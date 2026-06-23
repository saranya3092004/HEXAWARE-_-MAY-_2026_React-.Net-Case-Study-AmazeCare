using AmazeCare.Server.Modules.Auth.DTOs;
using FluentValidation;

namespace AmazeCare.Server.Modules.Auth.Validators
{
    public class StaffLoginRequestValidator : AbstractValidator<StaffLoginRequest>
    {
        public StaffLoginRequestValidator()
        {
            RuleFor(x => x.StaffCode)
                .NotEmpty().WithMessage("Staff code is required.")
                .Matches(@"^(DOC|ADM|LT)-.+$").WithMessage("Staff code must start with DOC-, ADM-, or LT-.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}