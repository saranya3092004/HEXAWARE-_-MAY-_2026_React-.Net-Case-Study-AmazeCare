using AmazeCare.Server.Models;

namespace AmazeCare.Server.Modules.PatientModule.Repository
{
    public interface IPatientRepository
    {

        IQueryable<Patient> GetByUserIdQueryable(int userId);
        IQueryable<Patient> GetAllQueryable();

        Task<Patient?> GetByIdAsync(int patientId);
        Task<Patient?> GetByIdWithConsultationsAsync(int patientId);

        Task<bool> PhoneExistsAsync(string phoneNumber);

        Task<int> GetPatientCountAsync();
        Task<Patient> AddAsync(Patient patient);
        Task UpdateAsync(Patient patient);

        Task<User?> GetUserByIdAsync(int userId);
        Task UpdateUserAsync(User user);
        Task<bool> EmailExistsAsync(string email);

    }
}
