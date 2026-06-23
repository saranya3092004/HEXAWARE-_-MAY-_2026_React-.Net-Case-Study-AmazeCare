using AmazeCare.Server.Models;

namespace AmazeCare.Server.Modules.PatientModule.DTOs
{
    public class PatientResponse
    {
        public int PatientId { get; set; }
        public int? UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; } 
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
