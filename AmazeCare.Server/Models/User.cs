namespace AmazeCare.Server.Models
{
     public enum UserRole
        {       
            User,
            Doctor,
            Admin,
            LabTechnician
        }

        public class User
        {
            public int UserId { get; set; }
            public string FullName { get; set; } = string.Empty;
            public string? Email { get; set; }               
            public string PhoneNumber { get; set; } = string.Empty;  
            public string? PasswordHash { get; set; }        
            public UserRole Role { get; set; }
            public bool IsActive { get; set; } = true;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

           
            public Doctor? Doctor { get; set; }
            public Admin? Admin { get; set; }
            public ICollection<Patient> Patients { get; set; } = new List<Patient>();
        }
    }

