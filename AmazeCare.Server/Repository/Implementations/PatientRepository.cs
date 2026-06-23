using AmazeCare.Server.Data;
using AmazeCare.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace AmazeCare.Server.Modules.PatientModule.Repository
{
    public class PatientRepository : IPatientRepository
    {
        private readonly AmazeCareDBContext _context;

        public PatientRepository(AmazeCareDBContext context)
        {
            _context = context;
        }

        public IQueryable<Patient> GetByUserIdQueryable(int userId)
        {
            return _context.Patients
                .Include(p => p.User)
                .Where(p => p.UserId == userId && p.IsActive);
        }

        public IQueryable<Patient> GetAllQueryable()
        {
            return _context.Patients
                .Include(p => p.User);
        }


        public async Task<Patient?> GetByIdAsync(int patientId)
        {
            return await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PatientId == patientId);
        }

        public async Task<Patient?> GetByIdWithConsultationsAsync(int patientId)
        {
            return await _context.Patients
                .Include(p => p.Consultations).ThenInclude(c => c.Doctor)
                .Include(p => p.Consultations).ThenInclude(c => c.Prescription)
                .Include(p => p.Consultations).ThenInclude(c => c.LabTests)
                .FirstOrDefaultAsync(p => p.PatientId == patientId);
        }

        public async Task<bool> PhoneExistsAsync(string phoneNumber)
        {
            return await _context.Patients.AnyAsync(p => p.PhoneNumber == phoneNumber);
        }

        public async Task<int> GetPatientCountAsync()
        {
            return await _context.Patients.CountAsync();
        }

        public async Task<Patient> AddAsync(Patient patient)
        {
            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();
            return patient;
        }

        public async Task UpdateAsync(Patient patient)
        {
            patient.UpdatedAt = DateTime.UtcNow;
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task UpdateUserAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email != null && u.Email.ToLower() == email.ToLower());
        }

       
    }
}