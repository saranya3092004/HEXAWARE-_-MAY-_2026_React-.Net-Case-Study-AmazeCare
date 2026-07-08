using AmazeCare.Server.Data;
using AmazeCare.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace AmazeCare.Server.Modules.DoctorModule.Repository
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly AmazeCareDBContext _context;

        public DoctorRepository(AmazeCareDBContext context)
        {
            _context = context;
        }

        public async Task<List<Doctor>> SearchAsync(string? name, string? specialization)
        {
            var query = _context.Doctors
                .Where(d => d.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                var term = name.Trim().ToLower();
                query = query.Where(d => d.Name.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(specialization))
            {
                var term = specialization.Trim().ToLower();
                query = query.Where(d => d.Specialization.ToLower()==term);
            }

            return await query.OrderBy(d => d.Name).ToListAsync();
        }

        public async Task<Doctor?> GetByIdAsync(int doctorId)
        {
            return await _context.Doctors.FirstOrDefaultAsync(d => d.DoctorId == doctorId);
        }

        public async Task<Doctor?> GetByIdWithProfileAsync(int doctorId)
        {
            return await _context.Doctors.FirstOrDefaultAsync(d => d.DoctorId == doctorId);
        }

        public async Task<Doctor> AddAsync(Doctor doctor)
        {
            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();
            return doctor;
        }

        public async Task UpdateAsync(Doctor doctor)
        {
            doctor.UpdatedAt = DateTime.UtcNow;
            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync();
        }

        public async Task<User> AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UserEmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email != null && u.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> UserPhoneExistsAsync(string phoneNumber)
        {
            return await _context.Users.AnyAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<List<Appointment>> GetAppointmentsForDoctorAsync(int doctorId, bool upcomingOnly)
        {
            var query = _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId);

            if (upcomingOnly)
            {
                query = query.Where(a => a.AppointmentDate >= DateTime.UtcNow.Date
                    && a.Status != AppointmentStatus.Cancelled
                    && a.Status != AppointmentStatus.Rejected
                    && a.Status != AppointmentStatus.Completed);
            }

            return await query.OrderBy(a => a.AppointmentDate).ThenBy(a => a.TimeSlot).ToListAsync();
        }

        public async Task<List<string>> GetDistinctSpecializationsAsync()
        {
            return await _context.Doctors
                .Where(d => !string.IsNullOrEmpty(d.Specialization))
                .Select(d => d.Specialization)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();
        }
    }
}