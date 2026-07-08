using AmazeCare.Server.Models;

namespace AmazeCare.Server.Repository.Interfaces
{
    public interface IAppointmentRepository
    {
        IQueryable<Appointment> GetQueryable();
        Task<Appointment?> GetByIdAsync(int appointmentId);
        Task<bool> PatientExistsAsync(int patientId);
        Task<bool> DoctorExistsAsync(int doctorId);
        Task<bool> HasConflictingAppointmentAsync(int doctorId, DateTime date, string timeSlot, int? excludeAppointmentId = null);
        Task<Appointment> AddAsync(Appointment appointment);
        Task UpdateAsync(Appointment appointment);
        Task<HashSet<string>> GetBookedSlotsAsync(int doctorId, DateTime date, int? excludeAppointmentId = null);
    }
}
