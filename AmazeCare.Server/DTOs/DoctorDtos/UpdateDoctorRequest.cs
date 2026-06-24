namespace AmazeCare.Server.Modules.DoctorModule.DTOs
{
    public class UpdateDoctorRequest
    {
        public string Name { get; set; } = string.Empty;

        public string Qualification { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }
        public string Specialization { get; set; } = string.Empty;

    }
}
