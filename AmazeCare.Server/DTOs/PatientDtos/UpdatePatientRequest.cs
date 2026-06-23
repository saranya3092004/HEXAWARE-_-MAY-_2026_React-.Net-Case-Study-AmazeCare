namespace AmazeCare.Server.Modules.PatientModule.DTOs
{
   
    public class UpdatePatientRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Email { get; set; }
    }
}