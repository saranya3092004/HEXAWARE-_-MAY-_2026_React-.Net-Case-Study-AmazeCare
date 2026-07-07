namespace AmazeCare.Server.Modules.PatientModule.DTOs
{
    public class ConsultationSummary
    {
        public int ConsultationId { get; set; }
        public int AppointmentId { get; set; }
        public DateTime ConsultationDate { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string? Diagnosis { get; set; }
        public string CurrentSymptoms { get; set; } = string.Empty;
        public bool HasPrescription { get; set; }
        public int LabTestCount { get; set; }
    }
}
