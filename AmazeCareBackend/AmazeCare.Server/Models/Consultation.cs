namespace AmazeCare.Server.Models
{
        public class Consultation
        {
            public int ConsultationId { get; set; }
            public int AppointmentId { get; set; }
            public int PatientId { get; set; }
            public int DoctorId { get; set; }

            // Spec-required capture fields
            public string CurrentSymptoms { get; set; } = string.Empty;    
            public string? PhysicalExamination { get; set; }                
            public string? TreatmentPlan { get; set; }                     

            public string? Diagnosis { get; set; }
            public DateTime ConsultationDate { get; set; } = DateTime.UtcNow;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

            // Navigation properties
            public Appointment Appointment { get; set; } = null!;
            public Patient Patient { get; set; } = null!;
            public Doctor Doctor { get; set; } = null!;

        }
    
}
