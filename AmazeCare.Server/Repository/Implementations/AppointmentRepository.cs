using AmazeCare.Server.Data;
using AmazeCare.Server.Models;
using AmazeCare.Server.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AmazeCare.Server.Repository.Implementations
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AmazeCareDBContext _context;

        public AppointmentRepository(AmazeCareDBContext context)
        {
            _context = context;
        }

        public IQueryable<Appointment> GetQueryable()
        {
            return _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Consultation)
                .AsQueryable();
        }

        public async Task<Appointment?> GetByIdAsync(int appointmentId)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Consultation)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
        }

        public async Task<bool> PatientExistsAsync(int patientId)
        {
            return await _context.Patients.AnyAsync(p => p.PatientId == patientId && p.IsActive);
        }

        public async Task<bool> DoctorExistsAsync(int doctorId)
        {
            return await _context.Doctors.AnyAsync(d => d.DoctorId == doctorId && d.IsActive);
        }

        public async Task<bool> HasConflictingAppointmentAsync(int doctorId, DateTime date, string timeSlot, int? excludeAppointmentId = null)
        {
            return await _context.Appointments.AnyAsync(a =>
                a.DoctorId == doctorId
                && a.AppointmentDate.Date == date.Date
                && a.TimeSlot == timeSlot
                && a.Status != AppointmentStatus.Cancelled
                && a.Status != AppointmentStatus.Rejected
                && (!excludeAppointmentId.HasValue || a.AppointmentId != excludeAppointmentId.Value));
        }

        public async Task<Appointment> AddAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        public async Task UpdateAsync(Appointment appointment)
        {
            appointment.UpdatedAt = DateTime.UtcNow;
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }
        public async Task<List<string>> GetBookedSlotsAsync(int doctorId, DateTime date)
        {
            return await _context.Appointments
                .Where(a => a.DoctorId == doctorId
                         && a.AppointmentDate.Date == date.Date
                         && a.Status != AppointmentStatus.Cancelled
                         && a.Status != AppointmentStatus.Rejected)
                .Select(a => a.TimeSlot)
                .ToListAsync();
        }
    }
}

