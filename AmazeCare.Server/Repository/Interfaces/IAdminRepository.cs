using AmazeCare.Server.Models;

namespace AmazeCare.Server.Repository.Interfaces
{
    public interface IAdminRepository
    {
        Task<int> GetTotalPatientsAsync();
        Task<int> GetTotalDoctorsAsync();
        Task<int> GetTodaysAppointmentsCountAsync();
        Task<int> GetPendingAppointmentsCountAsync();
        Task<List<Appointment>> GetAppointmentReportAsync(DateTime? fromDate, DateTime? toDate, int? doctorId, AppointmentStatus? status);
        Task<List<Patient>> GetPatientReportAsync(DateTime? fromDate, DateTime? toDate);
    }
}
