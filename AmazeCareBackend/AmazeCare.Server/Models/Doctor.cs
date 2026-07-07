namespace AmazeCare.Server.Models
{
        public class Doctor
        {
            public int DoctorId { get; set; }
            public int UserId { get; set; }                               // FK → User (for login)
            public string Name { get; set; } = string.Empty;
            public string Specialization { get; set; } = string.Empty;
            public string Qualification { get; set; } = string.Empty;    // e.g. MS (OG), MBBS
            public string Designation { get; set; } = string.Empty;      // e.g. Associate Professor/Consultant
            public int ExperienceYears { get; set; }
            public bool IsActive { get; set; } = true;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

            // Navigation properties
            public User User { get; set; } = null!;
     

            public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
            public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
        
        }
}
