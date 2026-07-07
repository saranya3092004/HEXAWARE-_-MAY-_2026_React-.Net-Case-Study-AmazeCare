using AmazeCare.Server.DTOs.ConsultationDtos;
using FluentValidation;

namespace AmazeCare.Server.Validator.Consultation_Validators
{
    public class UpdateConsultationRequestValidator : AbstractValidator<UpdateConsultationRequest>
    {
        public UpdateConsultationRequestValidator()
        {
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
