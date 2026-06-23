using AmazeCare.Server.DTOs.Appointment_Dtos;
using FluentValidation;

namespace AmazeCare.Server.Validator.Appointment_Validators
{
    public class RejectAppointmentRequestValidator : AbstractValidator<RejectAppointmentRequest>
    {
        public RejectAppointmentRequestValidator()
        {
            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("A reason is required to reject an appointment.")
                .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters.");
        }
    }

    public class CancelAppointmentRequestValidator : AbstractValidator<CancelAppointmentRequest>
    {
        public CancelAppointmentRequestValidator()
        {
            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("A reason is required to cancel an appointment.")
                .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters.");
        }
    }
}
