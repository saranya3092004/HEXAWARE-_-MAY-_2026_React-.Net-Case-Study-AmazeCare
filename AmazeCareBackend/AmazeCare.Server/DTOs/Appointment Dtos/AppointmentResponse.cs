namespace AmazeCare.Server.DTOs.Appointment_Dtos
{
    public class AppointmentResponse
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public string TimeSlot { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string VisitType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? CancellationReason { get; set; }
        public bool HasConsultation { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
