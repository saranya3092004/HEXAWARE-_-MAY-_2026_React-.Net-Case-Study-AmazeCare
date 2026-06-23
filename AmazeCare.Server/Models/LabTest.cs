namespace AmazeCare.Server.Models
{
    public class LabTest
    {
        public int LabTestId { get; set; }
        public int ConsultationId { get; set; }     // ordered during this consultation
        public int PatientId { get; set; }
        public int DoctorId { get; set; }           // who ordered the test
        public int TestCatalogId { get; set; }

        public DateTime OrderedDate { get; set; } = DateTime.UtcNow;


        // Navigation properties
        // LabTest → Consultation (many tests per consultation)
        public Consultation Consultation { get; set; } = null!;
        // LabTest → Patient (direct FK for quick patient queries)
        public Patient Patient { get; set; } = null!;
        // LabTest → Doctor (who ordered)
        public Doctor Doctor { get; set; } = null!;
  
       //// LabTest → LabTestCatalog (what test it is)
       public LabTestCatalog TestCatalog { get; set; } = null!;
    }
}