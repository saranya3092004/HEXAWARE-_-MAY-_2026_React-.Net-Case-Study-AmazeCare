using AmazeCare.Server.DTOs.AdminDtos;

namespace AmazeCare.Server.Services.Interfaces
{
    public interface IAdminService
    {
        Task<AdminDashboardResponse> GetDashboardAsync();
        Task<List<AppointmentReportItem>> GetAppointmentReportAsync(AppointmentReportRequest request);
        Task<List<PatientReportItem>> GetPatientReportAsync(PatientReportRequest request);
    }
}
