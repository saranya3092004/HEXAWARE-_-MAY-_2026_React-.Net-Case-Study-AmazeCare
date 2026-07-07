using AmazeCare.Server.Data;
using AmazeCare.Server.Models;
using AmazeCare.Server.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AmazeCare.Server.Repository.Implementations
{
    public class ConsultationRepository : IConsultationRepository
    {
        private readonly AmazeCareDBContext _context;

        public ConsultationRepository(AmazeCareDBContext context)
        {
            _context = context;
        }

        public IQueryable<Consultation> GetQueryable()
        {
            return _context.Consultations
                .Include(c => c.Patient)
                .Include(c => c.Doctor)
                .AsQueryable();
        }

        public async Task<Consultation?> GetByIdAsync(int consultationId)
        {
            return await _context.Consultations
                .Include(c => c.Patient)
                .Include(c => c.Doctor)
                .FirstOrDefaultAsync(c => c.ConsultationId == consultationId);
        }

        public async Task<Consultation?> GetByAppointmentIdAsync(int appointmentId)
        {
            return await _context.Consultations
                .FirstOrDefaultAsync(c => c.AppointmentId == appointmentId);
        }

        public async Task<Consultation> AddAsync(Consultation consultation)
        {
            await _context.Consultations.AddAsync(consultation);
            await _context.SaveChangesAsync();
            return consultation;
        }

        public async Task UpdateAsync(Consultation consultation)
        {
            consultation.UpdatedAt = DateTime.UtcNow;
            _context.Consultations.Update(consultation);
            await _context.SaveChangesAsync();
        }
    }
}
