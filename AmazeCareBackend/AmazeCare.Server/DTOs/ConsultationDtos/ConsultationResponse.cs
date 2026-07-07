namespace AmazeCare.Server.DTOs.ConsultationDtos
{
    public class ConsultationResponse
    {
        public int ConsultationId { get; set; }
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string CurrentSymptoms { get; set; } = string.Empty;
        public string? PhysicalExamination { get; set; }
        public string? TreatmentPlan { get; set; }
        public string? Diagnosis { get; set; }
        public DateTime ConsultationDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
