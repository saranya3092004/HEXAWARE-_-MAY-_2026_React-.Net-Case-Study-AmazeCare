using AmazeCare.Server.Models;

namespace AmazeCare.Server.Modules.DoctorModule.Repository
{
    public interface IDoctorRepository
    {
        Task<List<Doctor>> SearchAsync(string? name, string? specialization);
        Task<Doctor?> GetByIdAsync(int doctorId);
        Task<Doctor?> GetByIdWithProfileAsync(int doctorId); // includes Specializations + Availabilities
        Task<Doctor> AddAsync(Doctor doctor);
        Task UpdateAsync(Doctor doctor);

        Task<User> AddUserAsync(User user);
        Task<bool> UserEmailExistsAsync(string email);
        Task<bool> UserPhoneExistsAsync(string phoneNumber);

        // ---- Appointment (read/update needed for slot calc + cascade reassignment) ----
        Task<List<Appointment>> GetAppointmentsForDoctorAsync(int doctorId, bool upcomingOnly);
    }
}
