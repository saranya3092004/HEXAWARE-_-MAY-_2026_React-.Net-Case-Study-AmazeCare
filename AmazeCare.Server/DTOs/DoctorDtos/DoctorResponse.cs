namespace AmazeCare.Server.Modules.DoctorModule.DTOs
{
    public class DoctorResponse
    {
        public int DoctorId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }
        public bool IsActive { get; set; }
        public int SpecializationId { get; set; }
        public string SpecializationName { get; set;  } = string.Empty;
    }
}
