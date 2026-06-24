using FluentValidation;
using AmazeCare.Server.DTOs.Appointment_Dtos;
namespace AmazeCare.Server.Validator.Appointment_Validators
{
    public class RescheduleAppointmentRequestValidator : AbstractValidator<RescheduleAppointmentRequest>
    {
        public RescheduleAppointmentRequestValidator()
        {
            RuleFor(x => x.NewAppointmentDate)
                .Must(d => d.Date >= DateTime.UtcNow.Date)
                .WithMessage("New appointment date cannot be in the past.");

            RuleFor(x => x.NewTimeSlot)
                .NotEmpty().WithMessage("New time slot is required.")
                .Matches(@"^\d{2}:\d{2}-\d{2}:\d{2}$")
                .WithMessage("Time slot must be in the format HH:mm-HH:mm.");
        }
    }
}
