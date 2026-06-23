namespace AmazeCare.Server.Models
{
    public enum MedicineCategory
    {
        Antibiotic,
        Analgesic,
        Antipyretic,
        Antacid,
        Antihistamine,
        Antidiabetic,
        Antihypertensive,
        Antifungal,
        Antiviral,
        Vitamin,
        Supplement,
        Steroid,
        Bronchodilator,
        Diuretic,
        Other
    }

    public class Medicine
    {
        public int MedicineId { get; set; }
        public string Name { get; set; } = string.Empty;           // e.g. Paracetamol 500mg
        public string GenericName { get; set; } = string.Empty;    // e.g. Acetaminophen
        public MedicineCategory Category { get; set; }
        public string? DefaultDosage { get; set; }                 // e.g. "500mg"
        public bool IsActive { get; set; } = true;

        // Navigation property
        public ICollection<PrescriptionItem> PrescriptionItems { get; set; } = new List<PrescriptionItem>();
    }
}