using AmazeCare.Server.DTOs.Appointment_Dtos;
using FluentValidation;

namespace AmazeCare.Server.Validator.Appointment_Validators
{
    public class AppointmentFilterRequestValidator : AbstractValidator<AppointmentFilterRequest>
    {
        public AppointmentFilterRequestValidator()
        {
            RuleFor(x => x)
                .Must(x => !x.FromDate.HasValue || !x.ToDate.HasValue || x.FromDate <= x.ToDate)
                .WithMessage("FromDate cannot be later than ToDate.");
        }
    }
}
