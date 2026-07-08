using AmazeCare.Server.Modules.PatientModule.DTOs;
using FluentValidation;

namespace AmazeCare.Server.Modules.PatientModule.Validators
{
    public class RegisterWalkInPatientRequestValidator : AbstractValidator<RegisterWalkInPatientRequest>
    {
        public RegisterWalkInPatientRequestValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .Length(2, 100).WithMessage("Full name must be between 2 and 100 characters.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Invalid phone number format.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required.");
            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Invalid gender value.");

            RuleFor(x => x.Address)
                .MaximumLength(300).WithMessage("Address cannot exceed 300 characters.");
        }
    }
}