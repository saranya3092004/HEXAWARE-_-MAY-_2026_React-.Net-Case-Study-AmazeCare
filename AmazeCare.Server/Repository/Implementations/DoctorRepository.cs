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

        // ================= DOCTOR ================
        public async Task<List<Doctor>> SearchAsync(string? name, int? specializationId)
        {
            var doctorsList = await _context.Doctors
                .Where(d => d.IsActive)
                .Include(d => d.Specialization)
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(name))
            {
                var term = name.Trim().ToLower();
                doctorsList = doctorsList.Where(d => d.Name.ToLower().Contains(term)).ToList();
            }

            if (specializationId.HasValue)
            {
                doctorsList = doctorsList.Where(d => d.SpecializationId == specializationId.Value).ToList();
            }

            return doctorsList.OrderBy(d => d.Name).ToList();
        }

        public async Task<Doctor?> GetByIdAsync(int doctorId)
        {
            return await _context.Doctors.FirstOrDefaultAsync(d => d.DoctorId == doctorId);
        }

        public async Task<Doctor?> GetByIdWithProfileAsync(int doctorId)
        {
            return await _context.Doctors
                .Include(d => d.Specialization)
                .FirstOrDefaultAsync(d => d.DoctorId == doctorId);
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

        // ================= SPECIALIZATION =================

        public async Task<Specialization?> GetSpecializationByIdAsync(int specializationId)
        {
            return await _context.Specializations.FirstOrDefaultAsync(s => s.SpecializationId == specializationId);
        }


        // ================= APPOINTMENT =================

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

 
        public async Task<List<Appointment>> GetBookedAppointmentsAsync(int doctorId, DateTime date)
        {
            return await _context.Appointments
                .Where(a => a.DoctorId == doctorId
                    && a.AppointmentDate.Date == date.Date
                    && a.Status != AppointmentStatus.Cancelled
                    && a.Status != AppointmentStatus.Rejected
                    && a.Status != AppointmentStatus.NoShow)
                .ToListAsync();
        }
        public async Task UpdateAppointmentAsync(Appointment appointment)
        {
            appointment.UpdatedAt = DateTime.UtcNow;
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }
    }
}