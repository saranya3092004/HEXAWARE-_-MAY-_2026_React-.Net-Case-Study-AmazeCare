namespace AmazeCare.Server.Models
{
    public enum AppointmentStatus
    {
        Pending,
        Confirmed,
        Completed,
        Cancelled,
        Rejected,
        Rescheduled
    }

    public enum VisitType
    {
        GeneralCheckup,
        FollowUp,
        SpecificIssue,
        Emergency,
    }

    public class Appointment
    {
        public int AppointmentId { get; set; }

        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        public DateTime AppointmentDate { get; set; }
        public string TimeSlot { get; set; } = string.Empty;     // "10:00-10:30"

        public string? Reason { get; set; }
        public VisitType VisitType { get; set; } = VisitType.GeneralCheckup;
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
        public string? CancellationReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Patient Patient { get; set; } = null!;
        public Doctor Doctor { get; set; } = null!;
        public Consultation? Consultation { get; set; }
    }
}