using AmazeCare.Server.DTOs.ConsultationDtos;

namespace AmazeCare.Server.Services.Interfaces
{
    public interface IConsultationService
    {
        Task<List<ConsultationResponse>> GetConsultationsAsync(int callerId, bool isAdmin, bool isDoctor);
        Task<ConsultationResponse> GetByIdAsync(int consultationId, int callerId, bool isAdmin, bool isDoctor);
        Task<ConsultationResponse> CreateAsync(int doctorId, CreateConsultationRequest request);
        Task<ConsultationResponse> UpdateAsync(int consultationId, int doctorId, UpdateConsultationRequest request);
    }
}
