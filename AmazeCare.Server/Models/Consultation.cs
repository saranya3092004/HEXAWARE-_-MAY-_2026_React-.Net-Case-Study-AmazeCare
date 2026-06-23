namespace AmazeCare.Server.Models
{
        public class Consultation
        {
            public int ConsultationId { get; set; }
            public int AppointmentId { get; set; }
            public int PatientId { get; set; }
            public int DoctorId { get; set; }

            // Spec-required capture fields
            public string CurrentSymptoms { get; set; } = string.Empty;     // field 1
            public string? PhysicalExamination { get; set; }                // field 2 — vital signs, appearance
            public string? TreatmentPlan { get; set; }                      // field 3 — recommended tests

            public string? Diagnosis { get; set; }
            public DateTime ConsultationDate { get; set; } = DateTime.UtcNow;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

            // Navigation properties
            public Appointment Appointment { get; set; } = null!;
            public Patient Patient { get; set; } = null!;
            public Doctor Doctor { get; set; } = null!;
            public Prescription? Prescription { get; set; }
            public ICollection<LabTest> LabTests { get; set; } = new List<LabTest>();
        }
    
}
