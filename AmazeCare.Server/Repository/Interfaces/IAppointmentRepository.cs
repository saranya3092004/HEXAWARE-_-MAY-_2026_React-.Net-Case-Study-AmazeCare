using AmazeCare.Server.Models;

namespace AmazeCare.Server.Repository.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<List<Appointment>> GetFilteredAsync(
            int? patientUserId, int? doctorId, bool isAdmin,
            AppointmentStatus? status, DateTime? fromDate, DateTime? toDate);

        Task<Appointment?> GetByIdAsync(int appointmentId);
        Task<Appointment?> GetByIdWithConsultationAsync(int appointmentId);

        Task<int> GetAppointmentCountAsync();
        Task<Appointment> AddAsync(Appointment appointment);
        Task UpdateAsync(Appointment appointment);
        Task<Models.Patient?> GetPatientByIdAsync(int patientId);
        Task<Doctor?> GetDoctorByIdAsync(int doctorId);

    }
}
