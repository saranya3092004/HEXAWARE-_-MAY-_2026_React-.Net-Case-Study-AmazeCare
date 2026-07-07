using AmazeCare.Server.DTOs.AdminDtos;
using FluentValidation;

namespace AmazeCare.Server.Validator.Admin_Validators
{
    public class PatientReportRequestValidator : AbstractValidator<PatientReportRequest>
    {
        public PatientReportRequestValidator()
        {
            RuleFor(x => x)
                .Must(x => !x.FromDate.HasValue || !x.ToDate.HasValue || x.FromDate <= x.ToDate)
                .WithMessage("FromDate cannot be later than ToDate.");
        }
    }
}
