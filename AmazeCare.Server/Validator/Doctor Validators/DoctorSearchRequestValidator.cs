using AmazeCare.Server.Modules.DoctorModule.DTOs;
using FluentValidation;

namespace AmazeCare.Server.Modules.DoctorModule.Validator
{
    public class DoctorSearchRequestValidator : AbstractValidator<DoctorSearchRequest>
    {
        public DoctorSearchRequestValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(150).WithMessage("Name cannot exceed 150 characters.")
                .When(x => !string.IsNullOrEmpty(x.Name));

        }
    }
}
