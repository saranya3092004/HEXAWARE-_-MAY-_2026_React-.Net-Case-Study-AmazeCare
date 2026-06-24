using AmazeCare.Server.Models;

namespace AmazeCare.Server.DTOs.Appointment_Dtos
{
    public class AppointmentFilterRequest
    {
        public AppointmentStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
