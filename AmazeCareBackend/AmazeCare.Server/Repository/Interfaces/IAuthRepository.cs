using AmazeCare.Server.Models;

namespace AmazeCare.Server.Modules.Auth.Repository.Interface
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByPhoneAsync(string phoneNumber);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByUserIdAsync(int userId);
        Task<bool> PhoneExistsAsync(string phoneNumber);
        Task<bool> EmailExistsAsync(string email);
        Task<User> CreateUserAsync(User user);
        Task UpdateUserAsync(User user);



        Task<Patient?> GetPatientByUserIdAsync(int userId);
        Task<int> GetPatientCountAsync();
        Task<Patient> CreatePatientAsync(Patient patient);
        Task UpdatePatientAsync(Patient patient);



    }
}
