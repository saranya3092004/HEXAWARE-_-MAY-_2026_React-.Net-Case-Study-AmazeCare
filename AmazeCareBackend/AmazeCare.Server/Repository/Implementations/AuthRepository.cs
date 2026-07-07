using AmazeCare.Server.Data;
using AmazeCare.Server.Models;
using AmazeCare.Server.Modules.Auth.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace AmazeCare.Server.Modules.Auth.Repository.Implementation
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AmazeCareDBContext _db;

        public AuthRepository(AmazeCareDBContext db)
        {
            _db = db;
        }

        public async Task<User?> GetUserByPhoneAsync(string phoneNumber) // without lambda
        {
            return await _db.Users.Include(u => u.Patients).FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<User?> GetUserByEmailAsync(string email)=>  //with lambda function
          await _db.Users
            .Include(u=>u.Patients)
            .Include(u => u.Doctor)
            .Include(u => u.Admin)
            .FirstOrDefaultAsync(u=> u.Email == email);
        public async Task<User?> GetUserByUserIdAsync(int userId)
        {
            return await _db.Users.FindAsync(userId);
        }

        public async Task<bool> PhoneExistsAsync(string phoneNumber)
        {
            return await _db.Users.AnyAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _db.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _db.Users.Add(user); // tracks this object
            await _db.SaveChangesAsync(); //inserts to db
            return user;
        }

        public async Task UpdateUserAsync(User user)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }

        public async Task<Patient?> GetPatientByUserIdAsync(int userId)
        {
            return await _db.Patients
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<int> GetPatientCountAsync()
        {
            return await _db.Patients.CountAsync();
        }

        public async Task<Patient> CreatePatientAsync(Patient patient)
        {
            _db.Patients.Add(patient);
            await _db.SaveChangesAsync();
            return patient;
        }

        public async Task UpdatePatientAsync(Patient patient)
        {
            _db.Patients.Update(patient);
            await _db.SaveChangesAsync();
        }
    }
}
