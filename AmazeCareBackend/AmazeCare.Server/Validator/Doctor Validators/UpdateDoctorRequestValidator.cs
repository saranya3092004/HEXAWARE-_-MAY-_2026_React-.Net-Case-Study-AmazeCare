using AmazeCare.Server.Modules.DoctorModule.DTOs;
using FluentValidation;

namespace AmazeCare.Server.Modules.DoctorModule.Validator
{
    public class UpdateDoctorRequestValidator : AbstractValidator<UpdateDoctorRequest>
    {
        public UpdateDoctorRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(150).WithMessage("Name cannot exceed 150 characters.");

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
