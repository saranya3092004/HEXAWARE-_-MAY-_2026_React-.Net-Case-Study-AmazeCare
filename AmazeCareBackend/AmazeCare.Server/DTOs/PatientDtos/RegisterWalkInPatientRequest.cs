using AmazeCare.Server.Models;

namespace AmazeCare.Server.Modules.PatientModule.DTOs
{
    public class RegisterWalkInPatientRequest
    {
        public string FullName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string? Address { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
    }
}