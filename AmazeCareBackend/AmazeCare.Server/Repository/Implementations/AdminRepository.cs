using AmazeCare.Server.Data;
using AmazeCare.Server.Models;
using AmazeCare.Server.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AmazeCare.Server.Modules.AdminModule.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AmazeCareDBContext _context;

        public AdminRepository(AmazeCareDBContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalPatientsAsync()
        {
            return await _context.Patients.CountAsync(p => p.IsActive);
        }

        public async Task<int> GetTotalDoctorsAsync()
        {
            return await _context.Doctors.CountAsync(d => d.IsActive);
        }

        public async Task<int> GetTodaysAppointmentsCountAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _context.Appointments.CountAsync(a => a.AppointmentDate.Date == today);
        }

        public async Task<int> GetPendingAppointmentsCountAsync()
        {
            return await _context.Appointments.CountAsync(a => a.Status == AppointmentStatus.Pending);
        }

        public async Task<List<Appointment>> GetAppointmentReportAsync(DateTime? fromDate, DateTime? toDate, int? doctorId, AppointmentStatus? status)
        {
            var query = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(a => a.AppointmentDate.Date >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(a => a.AppointmentDate.Date <= toDate.Value.Date);

            if (doctorId.HasValue)
                query = query.Where(a => a.DoctorId == doctorId.Value);

            if (status.HasValue)
                query = query.Where(a => a.Status == status.Value);

            return await query.OrderByDescending(a => a.AppointmentDate).ToListAsync();
        }

        public async Task<List<Patient>> GetPatientReportAsync(DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Patients.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(p => p.CreatedAt.Date >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(p => p.CreatedAt.Date <= toDate.Value.Date);

            return await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
        }
    }
}