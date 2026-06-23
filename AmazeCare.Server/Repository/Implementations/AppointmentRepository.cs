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

        public async Task<List<Appointment>> GetFilteredAsync(int? patientUserId, int? doctorId, bool isAdmin,AppointmentStatus? status, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Specialization)
                .Include(a => a.Consultation)
                .AsQueryable();

            if (!isAdmin)
            {
                if (patientUserId.HasValue)
                {
                    query = query.Where(a => a.Patient.UserId == patientUserId.Value);
                }
                else if (doctorId.HasValue)
                {
                    query = query.Where(a => a.DoctorId == doctorId.Value);
                }
                else
                {
                    return new List<Appointment>();
                }
            }

            if (status.HasValue)
            {
                query = query.Where(a => a.Status == status.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(a => a.AppointmentDate.Date >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                query = query.Where(a => a.AppointmentDate.Date <= toDate.Value.Date);
            }

            return await query.OrderByDescending(a => a.AppointmentDate).ThenBy(a => a.TimeSlot).ToListAsync();
        }

        public async Task<Appointment?> GetByIdAsync(int appointmentId)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Specialization)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
        }

        public async Task<Appointment?> GetByIdWithConsultationAsync(int appointmentId)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Specialization)
                .Include(a => a.Consultation)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
        }

        public async Task<int> GetAppointmentCountAsync()
        {
            return await _context.Appointments.CountAsync();
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

        public async Task<Models.Patient?> GetPatientByIdAsync(int patientId)
        {
            return await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == patientId);
        }

        public async Task<Doctor?> GetDoctorByIdAsync(int doctorId)
        {
            return await _context.Doctors.FirstOrDefaultAsync(d => d.DoctorId == doctorId);
        }
    }
}
