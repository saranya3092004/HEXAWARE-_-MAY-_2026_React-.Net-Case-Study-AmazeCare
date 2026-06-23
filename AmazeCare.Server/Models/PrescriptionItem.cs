using AmazeCare.Server.Models;

namespace AmazeCare.Server.Models
{
    public class PrescriptionItem
    {
        public int PrescriptionItemId { get; set; }
        public int PrescriptionId { get; set; }

        public int MedicineId { get; set; }

        // Dosing schedule — one bool per time of day
        public bool Morning { get; set; }
        public bool Afternoon { get; set; }
        public bool Evening { get; set; }
        public bool Night { get; set; }

        public FoodInstruction FoodInstruction { get; set; } = FoodInstruction.AfterFood;

        public string? Dosage { get; set; }          // e.g. "500mg"
        public int DurationDays { get; set; }         // e.g. 5
        public string? Instructions { get; set; }     // e.g. "Take with warm water"

        // Navigation properties
        public Prescription Prescription { get; set; } = null!;
        public Medicine Medicine { get; set; } = null!;
    }
}