namespace AmazeCare.Server.Models
{
        public enum Gender
        {
            Male,
            Female,
            Other,
            PreferNotToSay
        }



        public class Patient
        {
            public int PatientId { get; set; }
            public int? UserId { get; set; }                           // nullable — admin can add without account
            public string FullName { get; set; } = string.Empty;
            public DateTime DateOfBirth { get; set; }                  // ← replaces Age (calculate on the fly)
            public Gender Gender { get; set; }
            public string PhoneNumber { get; set; } = string.Empty;
            public bool IsActive { get; set; } = true;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

            // Computed — never stored in DB
            public int Age => DateTime.Today.Year - DateOfBirth.Year -
                              (DateTime.Today.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);

            // Navigation properties
            public User? User { get; set; }
            public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
            public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();

        }
 }

