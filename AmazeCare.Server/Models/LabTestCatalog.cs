namespace AmazeCare.Server.Models
{
    public enum TestCategory
    {
        BloodTest,
        UrineTest,
        Imaging,        // X-Ray, MRI, CT
        Biopsy,
        MicrobiologyTest,
        HormoneTest,
        CardiologyTest,
        Other
    }

    public class LabTestCatalog
    {
        public int TestCatalogId { get; set; }
        public string TestName { get; set; } = string.Empty;       // e.g. Complete Blood Count
        public string? ShortName { get; set; }                     // e.g. CBC
        public TestCategory Category { get; set; }
        public string? NormalRange { get; set; }                   // e.g. "Hb: 13-17 g/dL"
        public bool IsActive { get; set; } = true;

        // Navigation property
        public ICollection<LabTest> LabTests { get; set; } = new List<LabTest>();
    }
}
