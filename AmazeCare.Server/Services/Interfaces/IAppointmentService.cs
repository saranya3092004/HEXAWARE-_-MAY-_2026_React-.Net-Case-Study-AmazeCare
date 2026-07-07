using AmazeCare.Server.DTOs.Appointment_Dtos;

namespace AmazeCare.Server.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<List<AppointmentResponse>> GetAppointmentsAsync(int callerId, bool isAdmin, bool isDoctor, AppointmentFilterRequest filter);
        Task<AppointmentResponse> GetByIdAsync(int appointmentId, int callerId, bool isAdmin, bool isDoctor);
        Task<AppointmentResponse> BookAppointmentAsync(int callerId, bool isAdmin, CreateAppointmentRequest request);
        Task<AppointmentResponse> ConfirmAsync(int appointmentId, int callerId, bool isAdmin);
        Task<AppointmentResponse> RejectAsync(int appointmentId, int callerId, bool isAdmin, RejectAppointmentRequest request);
        Task<AppointmentResponse> CancelAsync(int appointmentId, int callerId, bool isAdmin, CancelAppointmentRequest request);
        Task<AppointmentResponse> RescheduleAsync(int appointmentId, int callerId, bool isAdmin, RescheduleAppointmentRequest request);
        Task<AppointmentResponse> CompleteAsync(int appointmentId, int callerId);
        Task<List<string>> GetAvailableSlotsAsync(int doctorId, DateTime date);
    }
}
