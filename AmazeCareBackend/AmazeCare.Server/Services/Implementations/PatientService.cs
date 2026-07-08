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
            try
            {
                _logger.LogInformation("GetPatients: RequestingUserId={UserId}, IsAdmin={IsAdmin}, SearchTerm='{Search}', SortBy='{Sort}'.",
                    requestingUserId, isAdmin, searchTerm, sortBy);

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

                _logger.LogInformation("GetPatients succeeded: {Count} records returned.", items.Count);

                return items.Select(MapToResponse).ToList();
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetPatientsAsync for UserId {UserId}.", requestingUserId);
                throw;
            }
        }

        public async Task<PatientResponse> GetByIdAsync(int patientId, int requestingUserId, bool isAdmin)
        {
            try
            {
                _logger.LogInformation("GetPatientById: PatientId={PatientId}, RequestingUserId={UserId}.", patientId, requestingUserId);

                var patient = await _patientRepository.GetByIdAsync(patientId);
                if (patient == null)
                {
                    _logger.LogWarning("GetPatientById failed: PatientId {PatientId} not found.", patientId);
                    throw new NotFoundException("Patient not found.");
                }

                if (!isAdmin && patient.UserId != requestingUserId)
                {
                    _logger.LogWarning("Security violation! UserId {UserId} blocked from viewing PatientId {PatientId}.",
                        requestingUserId, patientId);
                    throw new ForbiddenException("You are not authorized to view this patient record.");
                }

                return MapToResponse(patient);
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetByIdAsync for PatientId {PatientId}.", patientId);
                throw;
            }
        }

        public async Task<PatientHistoryResponse> GetHistoryAsync(int patientId, int requestingUserId, bool isAdmin, bool isDoctor)
        {
            try
            {
                _logger.LogInformation("GetHistory: PatientId={PatientId}, RequestingUserId={UserId}, IsDoctor={IsDoctor}.",
                    patientId, requestingUserId, isDoctor);

                var patient = await _patientRepository.GetByIdWithConsultationsAsync(patientId);
                if (patient == null)
                {
                    _logger.LogWarning("GetHistory failed: PatientId {PatientId} not found.", patientId);
                    throw new NotFoundException("Patient not found.");
                }

                if (!isAdmin && !isDoctor && patient.UserId != requestingUserId)
                {
                    _logger.LogWarning("Security violation! UserId {UserId} blocked from viewing history for PatientId {PatientId}.",
                        requestingUserId, patientId);
                    throw new ForbiddenException("You are not authorized to view this patient's history.");
                }

                _logger.LogInformation("GetHistory succeeded: {Count} consultations found for PatientId {PatientId}.",
                    patient.Consultations?.Count ?? 0, patientId);

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
                            TreatmentPlan=c.TreatmentPlan,
                            CurrentSymptoms = c.CurrentSymptoms
                        })
                        .ToList()
                };
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetHistoryAsync for PatientId {PatientId}.", patientId);
                throw;
            }
        }

        public async Task<PatientResponse> RegisterWalkInPatientAsync(RegisterWalkInPatientRequest request)
        {
            try
            {
                _logger.LogInformation("RegisterWalkInPatient: Name={Name}, Phone={Phone}.", request.FullName, request.PhoneNumber);

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

                _logger.LogInformation("RegisterWalkInPatient succeeded: PatientId={PatientId}.", patient.PatientId);

                return MapToResponse(patient);
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in RegisterWalkInPatientAsync for Name {Name}.", request.FullName);
                throw;
            }
        }

        public async Task<PatientResponse> UpdateAsync(int patientId, int requestingUserId, bool isAdmin, UpdatePatientRequest request)
        {
            try
            {
                _logger.LogInformation("UpdatePatient: PatientId={PatientId}, RequestingUserId={UserId}.", patientId, requestingUserId);

                var patient = await _patientRepository.GetByIdAsync(patientId);
                if (patient == null)
                {
                    _logger.LogWarning("UpdatePatient failed: PatientId {PatientId} not found.", patientId);
                    throw new NotFoundException("Patient not found.");
                }

                if (!isAdmin && patient.UserId != requestingUserId)
                {
                    _logger.LogWarning("Security violation! UserId {UserId} blocked from updating PatientId {PatientId}.",
                        requestingUserId, patientId);
                    throw new ForbiddenException("You are not authorized to update this patient record.");
                }

                patient.FullName = request.FullName;
                patient.PhoneNumber = request.PhoneNumber;

                await _patientRepository.UpdateAsync(patient);

                _logger.LogInformation("UpdatePatient core fields saved for PatientId {PatientId}.", patientId);

                if (patient.UserId.HasValue && !string.IsNullOrEmpty(request.Email))
                {
                    var user = await _patientRepository.GetUserByIdAsync(patient.UserId.Value);
                    if (user != null)
                    {
                        if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase)
                            && await _patientRepository.EmailExistsAsync(request.Email))
                        {
                            _logger.LogWarning("UpdatePatient failed: email {Email} already in use.", request.Email);
                            throw new ConflictException("An account with this email already exists.");
                        }

                        user.Email = request.Email;
                        await _patientRepository.UpdateUserAsync(user);

                        _logger.LogInformation("UpdatePatient email updated for UserId {UserId}.", patient.UserId.Value);
                    }
                }

                return MapToResponse(patient);
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in UpdateAsync for PatientId {PatientId}.", patientId);
                throw;
            }
        }

        public async Task DeactivateAsync(int patientId)
        {
            try
            {
                _logger.LogInformation("DeactivatePatient: PatientId={PatientId}.", patientId);

                var patient = await _patientRepository.GetByIdAsync(patientId);
                if (patient == null)
                {
                    _logger.LogWarning("DeactivatePatient failed: PatientId {PatientId} not found.", patientId);
                    throw new NotFoundException("Patient not found.");
                }

                if (!patient.IsActive)
                {
                    _logger.LogWarning("DeactivatePatient skipped: PatientId {PatientId} already inactive.", patientId);
                    throw new ConflictException("This patient is already deactivated.");
                }

                patient.IsActive = false;
                await _patientRepository.UpdateAsync(patient);

                _logger.LogInformation("DeactivatePatient succeeded: PatientId {PatientId}.", patientId);
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in DeactivateAsync for PatientId {PatientId}.", patientId);
                throw;
            }
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