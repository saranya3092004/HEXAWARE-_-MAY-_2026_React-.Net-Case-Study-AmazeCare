namespace AmazeCare.Server.Models
{
        public enum Gender
        {
            Male,
            Female,
            Other,
            PreferNotToSay
        }

        //public enum BloodGroup
        //{
        //    APositive,
        //    ANegative,
        //    BPositive,
        //    BNegative,
        //    ABPositive,
        //    ABNegative,
        //    OPositive,
        //    ONegative,
        //    Unknown
        //}

        public enum RelationshipType
        {
            Self,
            Spouse,
            Child,
            Parent,
            Sibling,
            Other
        }

        public class Patient
        {
            public int PatientId { get; set; }
            //public string PatientCode { get; set; } = string.Empty;   // e.g. PAT-2024-0001
            public int? UserId { get; set; }                           // nullable — admin can add without account
            public string FullName { get; set; } = string.Empty;
            public DateTime DateOfBirth { get; set; }                  // ← replaces Age (calculate on the fly)
            public Gender Gender { get; set; }
            //public string? Address { get; set; }
            //public BloodGroup BloodGroup { get; set; } = BloodGroup.Unknown;
            //public RelationshipType Relationship { get; set; } = RelationshipType.Self;
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
            public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
            public ICollection<LabTest> LabTests { get; set; } = new List<LabTest>();
            //public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        }
 }

