using AmazeCare.Server.Modules.DoctorModule.DTOs;
using FluentValidation;

namespace AmazeCare.Server.Modules.DoctorModule.Validator
{
    public class CreateDoctorRequestValidator : AbstractValidator<CreateDoctorRequest>
    {
        public CreateDoctorRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(150).WithMessage("Name cannot exceed 150 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be a valid email address.")
                .MaximumLength(200).WithMessage("Email cannot exceed 200 characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("PhoneNumber is required.")
                .Matches(@"^[6-9]\d{9}$").WithMessage("PhoneNumber must be a valid 10-digit mobile number.");

            RuleFor(x => x.Qualification)
                .NotEmpty().WithMessage("Qualification is required.")
                .MaximumLength(200).WithMessage("Qualification cannot exceed 200 characters.");

            RuleFor(x => x.Designation)
                .NotEmpty().WithMessage("Designation is required.")
                .MaximumLength(100).WithMessage("Designation cannot exceed 100 characters.");

            RuleFor(x => x.ExperienceYears)
                .GreaterThanOrEqualTo(0).WithMessage("ExperienceYears cannot be negative.")
                .LessThanOrEqualTo(70).WithMessage("ExperienceYears seems unreasonably high.");

        }
    }
}
