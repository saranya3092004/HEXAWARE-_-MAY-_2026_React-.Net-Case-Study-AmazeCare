using AmazeCare.Server.Models;

namespace AmazeCare.Server.DTOs.AdminDtos
{
    public class AppointmentReportRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? DoctorId { get; set; }
        public AppointmentStatus? Status { get; set; }
    }
}
