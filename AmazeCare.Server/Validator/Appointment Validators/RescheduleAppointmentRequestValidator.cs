using AmazeCare.Server.DTOs.Appointment_Dtos;
using FluentValidation;

namespace AmazeCare.Server.Validator.Appointment_Validators
{
    public class RescheduleAppointmentRequestValidator : AbstractValidator<RescheduleAppointmentRequest>
    {
        public RescheduleAppointmentRequestValidator()
        {
            RuleFor(x => x.NewAppointmentDate)
                .NotEmpty().WithMessage("New appointment date is required.")
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("New appointment date cannot be in the past.");

            RuleFor(x => x.NewTimeSlot)
                .NotEmpty().WithMessage("New time slot is required.")
                .Matches(@"^([01]\d|2[0-3]):[0-5]\d-([01]\d|2[0-3]):[0-5]\d$")
                .WithMessage("Time slot must be in HH:mm-HH:mm format (e.g. '10:00-10:30').");
        }
    }
}
