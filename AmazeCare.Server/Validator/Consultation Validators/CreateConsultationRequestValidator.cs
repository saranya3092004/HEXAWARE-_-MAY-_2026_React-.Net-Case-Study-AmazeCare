using AmazeCare.Server.DTOs.ConsultationDtos;
using FluentValidation;

namespace AmazeCare.Server.Validator.Consultation_Validators
{
    public class CreateConsultationRequestValidator : AbstractValidator<CreateConsultationRequest>
    {
        public CreateConsultationRequestValidator()
        {
            RuleFor(x => x.AppointmentId)
                .GreaterThan(0).WithMessage("A valid appointment must be specified.");

            RuleFor(x => x.CurrentSymptoms)
                .NotEmpty().WithMessage("Current symptoms are required.")
                .MaximumLength(1000).WithMessage("Current symptoms cannot exceed 1000 characters.");

            RuleFor(x => x.PhysicalExamination)
                .MaximumLength(1000).WithMessage("Physical examination notes cannot exceed 1000 characters.");

            RuleFor(x => x.TreatmentPlan)
                .MaximumLength(1000).WithMessage("Treatment plan cannot exceed 1000 characters.");

            RuleFor(x => x.Diagnosis)
                .MaximumLength(1000).WithMessage("Diagnosis cannot exceed 1000 characters.");
        }
    }
}
