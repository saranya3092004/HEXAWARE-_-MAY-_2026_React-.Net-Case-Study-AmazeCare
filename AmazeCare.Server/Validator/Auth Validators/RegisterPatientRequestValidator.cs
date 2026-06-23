using AmazeCare.Server.Modules.Auth.DTOs;
using FluentValidation;

namespace AmazeCare.Server.Modules.Auth.Validators
{
    public class RegisterPatientRequestValidator : AbstractValidator<RegisterPatientRequest>
    {
        public RegisterPatientRequestValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .Length(2, 100).WithMessage("Full name must be between 2 and 100 characters.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Invalid phone number format.");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format.")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Password)
                 .NotEmpty().WithMessage("Password is required.")
                 .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                 .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                 .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                 .Matches("[0-9]").WithMessage("Password must contain at least one digit.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required.");

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Invalid gender value.");
        }
    }
}