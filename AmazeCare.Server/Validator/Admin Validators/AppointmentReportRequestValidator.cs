using AmazeCare.Server.DTOs.AdminDtos;
using FluentValidation;

namespace AmazeCare.Server.Validator.Admin_Validators
{
    public class AppointmentReportRequestValidator : AbstractValidator<AppointmentReportRequest>
    {
        public AppointmentReportRequestValidator()
        {
            RuleFor(x => x)
                .Must(x => !x.FromDate.HasValue || !x.ToDate.HasValue || x.FromDate <= x.ToDate)
                .WithMessage("FromDate cannot be later than ToDate.");

            RuleFor(x => x.DoctorId)
                .GreaterThan(0).When(x => x.DoctorId.HasValue)
                .WithMessage("DoctorId must be a valid positive number.");
        }
    }
}
