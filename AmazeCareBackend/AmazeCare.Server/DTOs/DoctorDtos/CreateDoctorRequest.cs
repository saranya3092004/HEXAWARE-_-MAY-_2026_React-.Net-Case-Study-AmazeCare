namespace AmazeCare.Server.Modules.DoctorModule.DTOs
{
    public class CreateDoctorRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;       // becomes both Doctor.Email and User.Email
        public string Password { get; set; } = string.Empty;    // initial login password, hashed before storage
        public string PhoneNumber { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }
    }
}
