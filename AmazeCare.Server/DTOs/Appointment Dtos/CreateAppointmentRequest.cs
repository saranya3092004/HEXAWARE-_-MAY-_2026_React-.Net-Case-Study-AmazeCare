using AmazeCare.Server.Models;

namespace AmazeCare.Server.DTOs.Appointment_Dtos
{
    public class CreateAppointmentRequest
    {
        public int? PatientId { get; set; }

        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string TimeSlot { get; set; } = string.Empty;     // "10:00-10:30"
        public string? Reason { get; set; }
        public VisitType VisitType { get; set; } = VisitType.GeneralCheckup;
    }
}
