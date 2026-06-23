namespace AmazeCare.Server.Models
{
    public enum FoodInstruction
    {
        BeforeFood,     
        AfterFood,      
        WithFood,       
        NoRestriction
    }

    public class Prescription
    {
        public int PrescriptionId { get; set; }

    
        public int ConsultationId { get; set; }

      
        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        public string? Notes { get; set; }
        public DateTime PrescribedDate { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

      
        public Consultation Consultation { get; set; } = null!;
        public Patient Patient { get; set; } = null!;
        public Doctor Doctor { get; set; } = null!;

      
        public ICollection<PrescriptionItem> Items { get; set; } = new List<PrescriptionItem>();
    }

    

   
}