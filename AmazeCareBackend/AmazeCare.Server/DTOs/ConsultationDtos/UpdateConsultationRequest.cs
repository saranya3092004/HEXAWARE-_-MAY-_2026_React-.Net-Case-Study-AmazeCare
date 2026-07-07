namespace AmazeCare.Server.DTOs.ConsultationDtos
{
    public class UpdateConsultationRequest
    {
        public string CurrentSymptoms { get; set; } = string.Empty;
        public string? PhysicalExamination { get; set; }
        public string? TreatmentPlan { get; set; }
        public string? Diagnosis { get; set; }
    }
}
