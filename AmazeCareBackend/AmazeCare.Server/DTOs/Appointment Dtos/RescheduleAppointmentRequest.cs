namespace AmazeCare.Server.DTOs.Appointment_Dtos
{
    public class RescheduleAppointmentRequest
    {
        public DateTime NewAppointmentDate { get; set; }
        public string NewTimeSlot { get; set; } = string.Empty;
    }
}
