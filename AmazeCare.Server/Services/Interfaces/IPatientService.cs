using AmazeCare.Server.Modules.PatientModule.DTOs;

namespace AmazeCare.Server.Modules.PatientModule.Service
{
    public interface IPatientService
    {
        Task<List<PatientResponse>> GetPatientsAsync(int requestingUserId, bool isAdmin, string? searchTerm, string? sortBy, bool sortDescending);
        Task<PatientResponse> GetByIdAsync(int patientId, int requestingUserId, bool isAdmin);
        Task<PatientHistoryResponse> GetHistoryAsync(int patientId, int requestingUserId, bool isAdmin, bool isDoctor);
        Task<PatientResponse> RegisterWalkInPatientAsync(RegisterWalkInPatientRequest request);
        Task<PatientResponse> UpdateAsync(int patientId, int requestingUserId, bool isAdmin, UpdatePatientRequest request);
        Task DeactivateAsync(int patientId);
    }
}
