using AmazeCare.Server.Modules.DoctorModule.DTOs;

namespace AmazeCare.Server.Modules.DoctorModule.Service
{
    public interface IDoctorService
    {
        Task<List<DoctorResponse>> SearchAsync(string? name, string? specialization);
        Task<DoctorResponse> GetProfileAsync(int doctorId);
        Task<List<AppointmentSummary>> GetDoctorAppointmentsAsync(int doctorId, bool upcomingOnly);
        Task<DoctorResponse> CreateDoctorAsync(CreateDoctorRequest request);
        Task<DoctorResponse> UpdateDoctorAsync(int doctorId, UpdateDoctorRequest request);
        Task DeactivateDoctorAsync(int doctorId);
        Task<List<string>> GetSpecializationsAsync();
    }


    public class AppointmentSummary
    {
        public int AppointmentId { get; set; }
        public int DoctorId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public string TimeSlot { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}