using AmazeCare.Server.DTOs.ConsultationDtos;
using AmazeCare.Server.Models;
using AmazeCare.Server.Modules.Middlewares;
using AmazeCare.Server.Repository.Interfaces;
using AmazeCare.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AmazeCare.Server.Services.Implementations
{
    public class ConsultationService : IConsultationService
    {
        private readonly IConsultationRepository _consultationRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ILogger<ConsultationService> _logger;

        public ConsultationService(
            IConsultationRepository consultationRepository,
            IAppointmentRepository appointmentRepository,
            ILogger<ConsultationService> logger)
        {
            _consultationRepository = consultationRepository;
            _appointmentRepository = appointmentRepository;
            _logger = logger;
        }

        public async Task<List<ConsultationResponse>> GetConsultationsAsync(int callerId, bool isAdmin, bool isDoctor)
        {
            try
            {
                var query = _consultationRepository.GetQueryable();

                if (!isAdmin)
                {
                    query = isDoctor
                        ? query.Where(c => c.DoctorId == callerId)
                        : query.Where(c => c.PatientId == callerId);
                }

                var consultations = await query.OrderByDescending(c => c.ConsultationDate).ToListAsync();

                _logger.LogInformation("GetConsultations: CallerId={CallerId}, IsAdmin={IsAdmin}, IsDoctor={IsDoctor}, ResultCount={Count}.",
                    callerId, isAdmin, isDoctor, consultations.Count);

                return consultations.Select(MapToResponse).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching consultations for CallerId {CallerId}.", callerId);
                throw;
            }
        }

        public async Task<ConsultationResponse> GetByIdAsync(int consultationId, int callerId, bool isAdmin, bool isDoctor)
        {
            try
            {
                var consultation = await _consultationRepository.GetByIdAsync(consultationId);
                if (consultation == null)
                {
                    _logger.LogWarning("GetConsultationById failed: ConsultationId {ConsultationId} not found.", consultationId);
                    throw new NotFoundException("Consultation not found.");
                }

                if (!isAdmin)
                {
                    var owns = isDoctor ? consultation.DoctorId == callerId : consultation.PatientId == callerId;
                    if (!owns)
                    {
                        _logger.LogWarning("Security violation! CallerId {CallerId} blocked from viewing ConsultationId {ConsultationId}.",
                            callerId, consultationId);
                        throw new ForbiddenException("You are not authorized to view this consultation.");
                    }
                }

                return MapToResponse(consultation);
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching ConsultationId {ConsultationId}.", consultationId);
                throw;
            }
        }

        public async Task<ConsultationResponse> CreateAsync(int doctorId, CreateConsultationRequest request)
        {
            try
            {
                var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId);
                if (appointment == null)
                {
                    _logger.LogWarning("CreateConsultation failed: AppointmentId {AppointmentId} not found.", request.AppointmentId);
                    throw new NotFoundException("Appointment not found.");
                }

                if (appointment.DoctorId != doctorId)
                {
                    _logger.LogWarning("Security violation! DoctorId {DoctorId} blocked from creating a consultation for AppointmentId {AppointmentId} (belongs to DoctorId {OwnerId}).",
                        doctorId, request.AppointmentId, appointment.DoctorId);
                    throw new ForbiddenException("You are not authorized to record a consultation for this appointment.");
                }

                if (appointment.Status != AppointmentStatus.Confirmed)
                {
                    _logger.LogWarning("CreateConsultation failed: AppointmentId {AppointmentId} is {Status}, expected Confirmed.",
                        request.AppointmentId, appointment.Status);
                    throw new ConflictException($"A consultation can only be recorded for a confirmed appointment. Current status: {appointment.Status}.");
                }

                var existing = await _consultationRepository.GetByAppointmentIdAsync(request.AppointmentId);
                if (existing != null)
                {
                    _logger.LogWarning("CreateConsultation failed: AppointmentId {AppointmentId} already has ConsultationId {ConsultationId}.",
                        request.AppointmentId, existing.ConsultationId);
                    throw new ConflictException("A consultation has already been recorded for this appointment.");
                }

                var consultation = new Consultation
                {
                    AppointmentId = request.AppointmentId,
                    PatientId = appointment.PatientId,
                    DoctorId = doctorId,
                    CurrentSymptoms = request.CurrentSymptoms,
                    PhysicalExamination = request.PhysicalExamination,
                    TreatmentPlan = request.TreatmentPlan,
                    Diagnosis = request.Diagnosis,
                    ConsultationDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                consultation = await _consultationRepository.AddAsync(consultation);
                consultation = await _consultationRepository.GetByIdAsync(consultation.ConsultationId) ?? consultation;

                _logger.LogInformation("CreateConsultation succeeded: ConsultationId={ConsultationId}, AppointmentId={AppointmentId}, DoctorId={DoctorId}.",
                    consultation.ConsultationId, request.AppointmentId, doctorId);

                return MapToResponse(consultation);
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating a consultation for AppointmentId {AppointmentId}.", request.AppointmentId);
                throw;
            }
        }

        public async Task<ConsultationResponse> UpdateAsync(int consultationId, int doctorId, UpdateConsultationRequest request)
        {
            try
            {
                var consultation = await _consultationRepository.GetByIdAsync(consultationId);
                if (consultation == null)
                {
                    _logger.LogWarning("UpdateConsultation failed: ConsultationId {ConsultationId} not found.", consultationId);
                    throw new NotFoundException("Consultation not found.");
                }

                if (consultation.DoctorId != doctorId)
                {
                    _logger.LogWarning("Security violation! DoctorId {DoctorId} blocked from updating ConsultationId {ConsultationId} (belongs to DoctorId {OwnerId}).",
                        doctorId, consultationId, consultation.DoctorId);
                    throw new ForbiddenException("You are not authorized to update this consultation.");
                }

                consultation.CurrentSymptoms = request.CurrentSymptoms;
                consultation.PhysicalExamination = request.PhysicalExamination;
                consultation.TreatmentPlan = request.TreatmentPlan;
                consultation.Diagnosis = request.Diagnosis;

                await _consultationRepository.UpdateAsync(consultation);

                _logger.LogInformation("UpdateConsultation succeeded: ConsultationId {ConsultationId}.", consultationId);

                return MapToResponse(consultation);
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating ConsultationId {ConsultationId}.", consultationId);
                throw;
            }
        }

        private static ConsultationResponse MapToResponse(Consultation consultation)
        {
            return new ConsultationResponse
            {
                ConsultationId = consultation.ConsultationId,
                AppointmentId = consultation.AppointmentId,
                PatientId = consultation.PatientId,
                PatientName = consultation.Patient?.FullName ?? string.Empty,
                DoctorId = consultation.DoctorId,
                DoctorName = consultation.Doctor?.Name ?? string.Empty,
                CurrentSymptoms = consultation.CurrentSymptoms,
                PhysicalExamination = consultation.PhysicalExamination,
                TreatmentPlan = consultation.TreatmentPlan,
                Diagnosis = consultation.Diagnosis,
                ConsultationDate = consultation.ConsultationDate,
                CreatedAt = consultation.CreatedAt
            };
        }
    }
}
