using AmazeCare.Server.Models;

namespace AmazeCare.Server.Modules.Auth.DTOs
{

    public class RegisterPatientRequest 
    {
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public string? Email  { get; set; }
        public string? Password { get; set; }

        public DateOnly DateOfBirth { get; set; }
        public Gender Gender { get; set; }
    }
}
