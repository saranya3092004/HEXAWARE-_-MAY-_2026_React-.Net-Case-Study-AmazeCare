using AmazeCare.Server.DTOs.Appointment_Dtos;
using FluentValidation;

namespace AmazeCare.Server.Validator.Appointment_Validators
{
    public class BookAppointmentRequestValidator : AbstractValidator<BookAppointmentRequest>
    {
        public BookAppointmentRequestValidator()
        {
            RuleFor(x => x.PatientId)
                .GreaterThan(0).WithMessage("A valid PatientId is required.");

            RuleFor(x => x.DoctorId)
                .GreaterThan(0).WithMessage("A valid DoctorId is required.");

            RuleFor(x => x.AppointmentDate)
                .NotEmpty().WithMessage("Appointment date is required.")
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Appointment date cannot be in the past.");

            RuleFor(x => x.TimeSlot)
                .NotEmpty().WithMessage("Time slot is required.")
                .Matches(@"^([01]\d|2[0-3]):[0-5]\d-([01]\d|2[0-3]):[0-5]\d$")
                .WithMessage("Time slot must be in HH:mm-HH:mm format (e.g. '10:00-10:30').");

            RuleFor(x => x.VisitType)
                .IsInEnum().WithMessage("Invalid visit type.");

            RuleFor(x => x.Reason)
                .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters.");
        }
    }
}
