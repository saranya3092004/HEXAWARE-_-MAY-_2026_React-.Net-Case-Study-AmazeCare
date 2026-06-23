using AmazeCare.Server.Models;

namespace AmazeCare.Server.Modules.Auth.DTOs
{

    public class RegisterPatientRequest // to Regsiter the User the Role is Assigned Later whether User OR Patient
    {
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public string? Email  { get; set; }
        public string? Password { get; set; }

        public DateOnly DateOfBirth { get; set; }
        public Gender Gender { get; set; }
    }
}
