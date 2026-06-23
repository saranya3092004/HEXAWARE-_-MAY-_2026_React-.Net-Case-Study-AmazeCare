using AmazeCare.Server.Modules.Auth.DTOs;
using FluentValidation;

namespace AmazeCare.Server.Modules.Auth.Validators
{
    public class EmailLoginRequestValidator : AbstractValidator<EmailLoginRequest>
    {
        public EmailLoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}