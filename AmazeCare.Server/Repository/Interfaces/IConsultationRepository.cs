using AmazeCare.Server.Models;

namespace AmazeCare.Server.Repository.Interfaces
{
    public interface IConsultationRepository
    {
        IQueryable<Consultation> GetQueryable();
        Task<Consultation?> GetByIdAsync(int consultationId);
        Task<Consultation?> GetByAppointmentIdAsync(int appointmentId);
        Task<Consultation> AddAsync(Consultation consultation);
        Task UpdateAsync(Consultation consultation);
    }
}
