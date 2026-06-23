using AmazeCare.Server.Modules.DoctorModule.DTOs;

namespace AmazeCare.Server.Modules.DoctorModule.Service
{
    public interface IDoctorService
    {
        Task<List<DoctorResponse>> SearchAsync(string? name, int? specializationId);
        Task<DoctorResponse> GetProfileAsync(int doctorId);
        Task<List<AppointmentSummary>> GetDoctorAppointmentsAsync(int doctorId, bool upcomingOnly);
        Task<DoctorResponse> CreateDoctorAsync(CreateDoctorRequest request);
        Task<DoctorResponse> UpdateDoctorAsync(int doctorId, UpdateDoctorRequest request);
        Task DeactivateDoctorAsync(int doctorId);
    }

    // Used by GetDoctorAppointmentsAsync — kept here rather than a separate file
    // since it's a small, Doctor-module-local shape, not used by the Appointment module.
    public class AppointmentSummary
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public string TimeSlot { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}