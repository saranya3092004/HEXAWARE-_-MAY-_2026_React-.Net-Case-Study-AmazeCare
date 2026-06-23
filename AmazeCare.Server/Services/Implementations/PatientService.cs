using AmazeCare.Server.Models;
using AmazeCare.Server.Modules.Middlewares;
using AmazeCare.Server.Modules.PatientModule.DTOs;
using AmazeCare.Server.Modules.PatientModule.Repository;
using Microsoft.EntityFrameworkCore;

namespace AmazeCare.Server.Modules.PatientModule.Service
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly ILogger<PatientService> _logger;

        public PatientService(IPatientRepository patientRepository, ILogger<PatientService> logger)
        {
            _patientRepository = patientRepository;
            _logger = logger;
        }

        public async Task<List<PatientResponse>> GetPatientsAsync(int requestingUserId, bool isAdmin, string? searchTerm, string? sortBy, bool sortDescending)
        {
            _logger.LogInformation("PatientService: Fetching complete patient list. SearchTerm='{Search}', SortBy='{Sort}'", searchTerm, sortBy);

            IQueryable<Models.Patient> query = isAdmin
                ? _patientRepository.GetAllQueryable()
                : _patientRepository.GetByUserIdQueryable(requestingUserId);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(p =>
                    p.FullName.ToLower().Contains(term) ||
                    p.PhoneNumber.Contains(term));
            }

            query = sortBy?.ToLower() switch
            {
                "name" => sortDescending ? query.OrderByDescending(p => p.FullName) : query.OrderBy(p => p.FullName),
                "createdat" => sortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
                _ => query.OrderBy(p => p.PatientId)
            };

            var items = await query.ToListAsync();

            _logger.LogInformation("GetPatients processed. RequestingUserId={UserId}, TotalItemsReturned={Count}.", requestingUserId, items.Count);

            return items.Select(MapToResponse).ToList();
        }

        public async Task<PatientResponse> GetByIdAsync(int patientId, int requestingUserId, bool isAdmin)
        {
            _logger.LogInformation("Searching details for PatientId: {PatientId} by User: {UserId}", patientId, requestingUserId);
            var patient = await _patientRepository.GetByIdAsync(patientId);
            if (patient == null)
            {
                _logger.LogWarning("Patient retrieval failed. PatientId: {PatientId} was not found.", patientId);
                throw new NotFoundException("Patient not found.");
            }

            if (!isAdmin && patient.UserId != requestingUserId)
            {
                _logger.LogWarning("Security violation! User: {UserId} blocked from viewing PatientId: {PatientId} (Belongs to User: {OwnerId})",
                    requestingUserId, patientId, patient.UserId);
                throw new ForbiddenException("You are not authorized to view this patient record.");
            }

            return MapToResponse(patient);
        }

        public async Task<PatientHistoryResponse> GetHistoryAsync(int patientId, int requestingUserId, bool isAdmin, bool isDoctor)
        {
            _logger.LogInformation("Medical history pull triggered for PatientId: {PatientId}. Requested by User: {UserId}, IsDoctor: {IsDoctor}",
                patientId, requestingUserId, isDoctor);
            var patient = await _patientRepository.GetByIdWithConsultationsAsync(patientId);
            if (patient == null)
            {
                _logger.LogWarning("History retrieval abort. PatientId: {PatientId} does not exist.", patientId);
                throw new NotFoundException("Patient not found.");
            }

            if (!isAdmin && !isDoctor && patient.UserId != requestingUserId)
            {
                _logger.LogWarning("Security violation! Unauthorized history inspection block for PatientId: {PatientId} by User: {UserId}",
                    patientId, requestingUserId);
                throw new ForbiddenException("You are not authorized to view this patient's history.");
            }

            var consultationCount = patient.Consultations?.Count ?? 0;
            _logger.LogInformation("Mapping history context package. Found {Count} consultation records.", consultationCount);

            return new PatientHistoryResponse
            {
                PatientId = patient.PatientId,
                PatientName = patient.FullName,
                Consultations = patient.Consultations
                    .OrderByDescending(c => c.ConsultationDate)
                    .Select(c => new ConsultationSummary
                    {
                        ConsultationId = c.ConsultationId,
                        AppointmentId = c.AppointmentId,
                        ConsultationDate = c.ConsultationDate,
                        DoctorName = c.Doctor?.Name ?? "Unknown",
                        Diagnosis = c.Diagnosis,
                        CurrentSymptoms = c.CurrentSymptoms,
                        HasPrescription = c.Prescription != null,
                        LabTestCount = c.LabTests?.Count ?? 0
                    })
                    .ToList()
            };
        }

        public async Task<PatientResponse> RegisterWalkInPatientAsync(RegisterWalkInPatientRequest request)
        {
            _logger.LogInformation("Registering a new walk-in patient. Name: {Name}, Phone: {Phone}", request.FullName, request.PhoneNumber);

            var patient = new Models.Patient
            {
                UserId = null,
                FullName = request.FullName,
                DateOfBirth = request.DateOfBirth.ToDateTime(TimeOnly.MinValue),
                Gender = request.Gender,
                PhoneNumber = request.PhoneNumber,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            patient = await _patientRepository.AddAsync(patient);
            _logger.LogInformation("Walk-in patient created successfully. PatientId: {Id}", patient.PatientId);
            return MapToResponse(patient);
        }

        public async Task<PatientResponse> UpdateAsync(int patientId, int requestingUserId, bool isAdmin, UpdatePatientRequest request)
        {
            _logger.LogInformation("Profile modification query received for PatientId: {PatientId} by User: {UserId}", patientId, requestingUserId);
            var patient = await _patientRepository.GetByIdAsync(patientId);
            if (patient == null)
            {
                _logger.LogWarning("Update cancelled. Target PatientId: {PatientId} doesn't exist.", patientId);
                throw new NotFoundException("Patient not found.");
            }

            if (!isAdmin && patient.UserId != requestingUserId)
            {
                _logger.LogWarning("Access Denied on Update request. User {UserId} is not authorized to edit records of Patient {PatientId}", requestingUserId, patientId);
                throw new ForbiddenException("You are not authorized to update this patient record.");
            }

            patient.FullName = request.FullName;
            patient.PhoneNumber = request.PhoneNumber;

            await _patientRepository.UpdateAsync(patient);
            _logger.LogInformation("Core patient profile table details updated for PatientId: {PatientId}", patientId);

            if (patient.UserId.HasValue && !string.IsNullOrEmpty(request.Email))
            {
                var user = await _patientRepository.GetUserByIdAsync(patient.UserId.Value);
                if (user != null)
                {
                    if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase)
                        && await _patientRepository.EmailExistsAsync(request.Email))
                    {
                        _logger.LogWarning("Update rejected. The email identity target '{Email}' is already assigned to a different user profile.", request.Email);
                        throw new ConflictException("An account with this email already exists.");
                    }

                    user.Email = request.Email;
                    await _patientRepository.UpdateUserAsync(user);
                    _logger.LogInformation("Linked Identity login email adjusted successfully for Profile User ID: {UserId}", patient.UserId.Value);
                }
            }

            return MapToResponse(patient);
        }

        public async Task DeactivateAsync(int patientId)
        {
            _logger.LogInformation("Deactivation sequence triggered for PatientId: {PatientId}", patientId);
            var patient = await _patientRepository.GetByIdAsync(patientId);
            if (patient == null)
            {
                _logger.LogWarning("Deactivation failed. Target PatientId: {PatientId} does not exist.", patientId);
                throw new NotFoundException("Patient not found.");
            }

            if (!patient.IsActive)
            {
                _logger.LogWarning("Idempotency notice. PatientId: {PatientId} is already marked inactive.", patientId);
                throw new ConflictException("This patient is already deactivated.");
            }

            patient.IsActive = false;
            await _patientRepository.UpdateAsync(patient);
            _logger.LogInformation("Patient record successfully deactivated. PatientId: {PatientId} is now soft-deleted.", patientId);
        }

        private static PatientResponse MapToResponse(Models.Patient patient)
        {
            return new PatientResponse
            {
                PatientId = patient.PatientId,
                UserId = patient.UserId,
                FullName = patient.FullName,
                DateOfBirth = patient.DateOfBirth,
                Age = patient.Age,
                Gender = patient.Gender,
                PhoneNumber = patient.PhoneNumber,
                Email = patient.User?.Email,
                IsActive = patient.IsActive,
                CreatedAt = patient.CreatedAt
            };
        }
    }
}