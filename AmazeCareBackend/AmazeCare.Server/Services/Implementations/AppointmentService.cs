using AmazeCare.Server.DTOs.Appointment_Dtos;
using AmazeCare.Server.Models;
using AmazeCare.Server.Modules.Middlewares;
using AmazeCare.Server.Repository.Interfaces;
using AmazeCare.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AmazeCare.Server.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ILogger<AppointmentService> _logger;

        private const int MinimumBookingLeadTimeMinutes = 30;
        private static readonly TimeSpan WorkStart = TimeSpan.FromHours(9);
        private static readonly TimeSpan WorkEnd = TimeSpan.FromHours(17);
        private static readonly TimeSpan SlotDuration = TimeSpan.FromMinutes(30);

        private static readonly TimeZoneInfo IstZone =
    TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        private static DateTime NowInIst()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IstZone);
        }


        public AppointmentService(IAppointmentRepository appointmentRepository, ILogger<AppointmentService> logger)
        {
            _appointmentRepository = appointmentRepository;
            _logger = logger;
        }

        public async Task<List<AppointmentResponse>> GetAppointmentsAsync(int callerId, bool isAdmin, bool isDoctor, AppointmentFilterRequest filter)
        {
            try
            {
                IQueryable<Appointment> query = _appointmentRepository.GetQueryable();

                if (!isAdmin)
                {
                    query = isDoctor
                        ? query.Where(a => a.DoctorId == callerId)
                        : query.Where(a => a.PatientId == callerId);
                }

                if (filter.Status.HasValue)
                    query = query.Where(a => a.Status == filter.Status.Value);

                if (filter.FromDate.HasValue)
                    query = query.Where(a => a.AppointmentDate.Date >= filter.FromDate.Value.Date);

                if (filter.ToDate.HasValue)
                    query = query.Where(a => a.AppointmentDate.Date <= filter.ToDate.Value.Date);

                var appointments = await query.OrderByDescending(a => a.AppointmentDate).ToListAsync();

                _logger.LogInformation("GetAppointments: CallerId={CallerId}, IsAdmin={IsAdmin}, IsDoctor={IsDoctor}, ResultCount={Count}.",
                    callerId, isAdmin, isDoctor, appointments.Count);

                return appointments.Select(MapToResponse).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching appointments for CallerId {CallerId}.", callerId);
                throw;
            }
        }

        public async Task<AppointmentResponse> GetByIdAsync(int appointmentId, int callerId, bool isAdmin, bool isDoctor)
        {
            try
            {
                var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
                if (appointment == null)
                {
                    _logger.LogWarning("GetAppointmentById failed: AppointmentId {AppointmentId} not found.", appointmentId);
                    throw new NotFoundException("Appointment not found.");
                }

                if (!isAdmin)
                {
                    var owns = isDoctor ? appointment.DoctorId == callerId : appointment.PatientId == callerId;
                    if (!owns)
                    {
                        _logger.LogWarning("Security violation! CallerId {CallerId} blocked from viewing AppointmentId {AppointmentId}.",
                            callerId, appointmentId);
                        throw new ForbiddenException("You are not authorized to view this appointment.");
                    }
                }

                return MapToResponse(appointment);
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching AppointmentId {AppointmentId}.", appointmentId);
                throw;
            }
        }

        public async Task<AppointmentResponse> BookAppointmentAsync(int callerId, bool isAdmin, CreateAppointmentRequest request)
        {
            try
            {
                if (!IsValidSlot(request.TimeSlot))
                {
                    _logger.LogWarning("BookAppointment failed: TimeSlot '{TimeSlot}' is outside business hours ({Start}-{End}) or not aligned to the {Duration}-minute grid.",
                        request.TimeSlot, WorkStart, WorkEnd, SlotDuration.TotalMinutes);
                    throw new BadRequestException(
                        $"'{request.TimeSlot}' is not a valid appointment slot. Slots run from {WorkStart:hh\\:mm} to {WorkEnd:hh\\:mm} in {SlotDuration.TotalMinutes}-minute increments.");
                }

                var slotStart = GetSlotStartDateTime(request.AppointmentDate, request.TimeSlot);
                var earliestBookable = NowInIst().AddMinutes(MinimumBookingLeadTimeMinutes);
                if (slotStart <= earliestBookable)
                {
                    _logger.LogWarning("BookAppointment failed: requested slot {SlotStart} does not meet the {Buffer}-minute minimum lead time (earliest bookable: {Earliest}).",
                        slotStart, MinimumBookingLeadTimeMinutes, earliestBookable);
                    throw new BadRequestException(
                        $"Appointments must be booked at least {MinimumBookingLeadTimeMinutes} minutes in advance. Please choose a later time slot.");
                }

                int patientId;
                if (isAdmin)
                {
                    if (!request.PatientId.HasValue)
                    {
                        _logger.LogWarning("BookAppointment failed: Admin booking did not specify a PatientId.");
                        throw new BadRequestException("PatientId is required when an admin books an appointment on behalf of a patient.");
                    }
                    patientId = request.PatientId.Value;
                }
                else
                {
                    patientId = callerId;
                }

                if (!await _appointmentRepository.PatientExistsAsync(patientId))
                {
                    _logger.LogWarning("BookAppointment failed: PatientId {PatientId} not found or inactive.", patientId);
                    throw new NotFoundException("Patient not found.");
                }

                if (!await _appointmentRepository.DoctorExistsAsync(request.DoctorId))
                {
                    _logger.LogWarning("BookAppointment failed: DoctorId {DoctorId} not found or inactive.", request.DoctorId);
                    throw new NotFoundException("Doctor not found or inactive.");
                }

                if (await _appointmentRepository.HasConflictingAppointmentAsync(request.DoctorId, request.AppointmentDate, request.TimeSlot))
                {
                    _logger.LogWarning("BookAppointment failed: DoctorId {DoctorId} already booked for {Date} {TimeSlot}.",
                        request.DoctorId, request.AppointmentDate.Date, request.TimeSlot);
                    throw new ConflictException("This doctor already has an appointment booked for that date and time slot.");
                }

                var appointment = new Appointment
                {
                    PatientId = patientId,
                    DoctorId = request.DoctorId,
                    AppointmentDate = request.AppointmentDate.Date,
                    TimeSlot = request.TimeSlot,
                    Reason = request.Reason,
                    VisitType = request.VisitType,
                    Status = AppointmentStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                appointment = await _appointmentRepository.AddAsync(appointment);
                appointment = await _appointmentRepository.GetByIdAsync(appointment.AppointmentId) ?? appointment;

                _logger.LogInformation("BookAppointment succeeded: AppointmentId={AppointmentId}, PatientId={PatientId}, DoctorId={DoctorId}.",
                    appointment.AppointmentId, patientId, request.DoctorId);

                return MapToResponse(appointment);
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while booking an appointment.");
                throw;
            }
        }

        public async Task<AppointmentResponse> ConfirmAsync(int appointmentId, int callerId, bool isAdmin)
        {
            try
            {
                var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
                if (appointment == null)
                {
                    _logger.LogWarning("ConfirmAppointment failed: AppointmentId {AppointmentId} not found.", appointmentId);
                    throw new NotFoundException("Appointment not found.");
                }

                if (!isAdmin && appointment.DoctorId != callerId)
                {
                    _logger.LogWarning("Security violation! DoctorId {CallerId} blocked from confirming AppointmentId {AppointmentId} (belongs to DoctorId {OwnerId}).",
                        callerId, appointmentId, appointment.DoctorId);
                    throw new ForbiddenException("You are not authorized to confirm this appointment.");
                }

                if (appointment.Status != AppointmentStatus.Pending && appointment.Status != AppointmentStatus.Rescheduled)
                {
                    _logger.LogWarning("ConfirmAppointment failed: AppointmentId {AppointmentId} is {Status}, expected Pending.",
                        appointmentId, appointment.Status);
                    throw new ConflictException($"Only pending or rescheduled appointments can be confirmed. Current status: {appointment.Status}.");
                }

                appointment.Status = AppointmentStatus.Confirmed;
                await _appointmentRepository.UpdateAsync(appointment);

                _logger.LogInformation("ConfirmAppointment succeeded: AppointmentId {AppointmentId}.", appointmentId);

                return MapToResponse(appointment);
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while confirming AppointmentId {AppointmentId}.", appointmentId);
                throw;
            }
        }

        public async Task<AppointmentResponse> RejectAsync(int appointmentId, int callerId, bool isAdmin, RejectAppointmentRequest request)
        {
            try
            {
                var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
                if (appointment == null)
                {
                    _logger.LogWarning("RejectAppointment failed: AppointmentId {AppointmentId} not found.", appointmentId);
                    throw new NotFoundException("Appointment not found.");
                }

                if (!isAdmin && appointment.DoctorId != callerId)
                {
                    _logger.LogWarning("Security violation! DoctorId {CallerId} blocked from rejecting AppointmentId {AppointmentId}.",
                        callerId, appointmentId);
                    throw new ForbiddenException("You are not authorized to reject this appointment.");
                }

                if (appointment.Status != AppointmentStatus.Pending && appointment.Status != AppointmentStatus.Rescheduled)
                {
                    _logger.LogWarning("RejectAppointment failed: AppointmentId {AppointmentId} is {Status}, expected Pending.",
                        appointmentId, appointment.Status);
                    throw new ConflictException($"Only pending or rescheduled appointments can be rejected. Current status: {appointment.Status}.");
                }

                appointment.Status = AppointmentStatus.Rejected;
                appointment.CancellationReason = request.Reason;
                await _appointmentRepository.UpdateAsync(appointment);

                _logger.LogInformation("RejectAppointment succeeded: AppointmentId {AppointmentId}, Reason='{Reason}'.",
                    appointmentId, request.Reason);

                return MapToResponse(appointment);
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while rejecting AppointmentId {AppointmentId}.", appointmentId);
                throw;
            }
        }

        public async Task<AppointmentResponse> CancelAsync(int appointmentId, int callerId, bool isAdmin, CancelAppointmentRequest request)
        {
            try
            {
                var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
                if (appointment == null)
                {
                    _logger.LogWarning("CancelAppointment failed: AppointmentId {AppointmentId} not found.", appointmentId);
                    throw new NotFoundException("Appointment not found.");
                }

                if (!isAdmin && appointment.PatientId != callerId)
                {
                    _logger.LogWarning("Security violation! CallerId {CallerId} blocked from cancelling AppointmentId {AppointmentId}.",
                        callerId, appointmentId);
                    throw new ForbiddenException("You are not authorized to cancel this appointment.");
                }

                if (appointment.Status is AppointmentStatus.Completed or AppointmentStatus.Cancelled or AppointmentStatus.Rejected)
                {
                    _logger.LogWarning("CancelAppointment failed: AppointmentId {AppointmentId} is already {Status}.",
                        appointmentId, appointment.Status);
                    throw new ConflictException($"This appointment cannot be cancelled. Current status: {appointment.Status}.");
                }

                appointment.Status = AppointmentStatus.Cancelled;
                appointment.CancellationReason = request.Reason;
                await _appointmentRepository.UpdateAsync(appointment);

                _logger.LogInformation("CancelAppointment succeeded: AppointmentId {AppointmentId}, Reason='{Reason}'.",
                    appointmentId, request.Reason);

                return MapToResponse(appointment);
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while cancelling AppointmentId {AppointmentId}.", appointmentId);
                throw;
            }
        }

        public async Task<AppointmentResponse> RescheduleAsync(int appointmentId, int callerId, bool isAdmin, bool isDoctor, RescheduleAppointmentRequest request)
        {
            try
            {
                var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
                if (appointment == null)
                {
                    _logger.LogWarning("RescheduleAppointment failed: AppointmentId {AppointmentId} not found.", appointmentId);
                    throw new NotFoundException("Appointment not found.");
                }

                // Authorization — patient can reschedule own, doctor can reschedule own, admin can reschedule any
                if (!isAdmin)
                {
                    if (isDoctor && appointment.DoctorId != callerId)
                        throw new ForbiddenException("You are not authorized to reschedule this appointment.");

                    if (!isDoctor && appointment.PatientId != callerId)
                        throw new ForbiddenException("You are not authorized to reschedule this appointment.");
                }

                if (appointment.Status is AppointmentStatus.Completed or AppointmentStatus.Cancelled or AppointmentStatus.Rejected)
                {
                    _logger.LogWarning("RescheduleAppointment failed: AppointmentId {AppointmentId} is {Status}.",
                        appointmentId, appointment.Status);
                    throw new ConflictException($"This appointment cannot be rescheduled. Current status: {appointment.Status}.");
                }

                if (!IsValidSlot(request.NewTimeSlot))
                    throw new BadRequestException($"'{request.NewTimeSlot}' is not a valid appointment slot.");

                var newSlotStart = GetSlotStartDateTime(request.NewAppointmentDate, request.NewTimeSlot);
                var earliestBookable = NowInIst().AddMinutes(MinimumBookingLeadTimeMinutes);

                if (newSlotStart <= earliestBookable)
                    throw new BadRequestException($"Appointments must be rescheduled to at least {MinimumBookingLeadTimeMinutes} minutes from now.");

                if (await _appointmentRepository.HasConflictingAppointmentAsync(
                        appointment.DoctorId, request.NewAppointmentDate, request.NewTimeSlot, appointment.AppointmentId))
                    throw new ConflictException("The doctor already has an appointment booked for that new date and time slot.");

                appointment.AppointmentDate = request.NewAppointmentDate.Date;
                appointment.TimeSlot = request.NewTimeSlot;

                // Patient reschedule → back to Pending (doctor must re-confirm the new slot)
                // Doctor/Admin reschedule → Rescheduled (they're deciding on behalf of the patient)
                appointment.Status = (isDoctor || isAdmin)
                    ? AppointmentStatus.Rescheduled
                    : AppointmentStatus.Pending;

                await _appointmentRepository.UpdateAsync(appointment);

                _logger.LogInformation("RescheduleAppointment succeeded: AppointmentId {AppointmentId}, NewDate={Date}, NewSlot={Slot}, NewStatus={Status}.",
                    appointmentId, request.NewAppointmentDate.Date, request.NewTimeSlot, appointment.Status);

                return MapToResponse(appointment);
            }
            catch (AppException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while rescheduling AppointmentId {AppointmentId}.", appointmentId);
                throw;
            }
        }

        public async Task<AppointmentResponse> CompleteAsync(int appointmentId, int callerId)
        {
            try
            {
                var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
                if (appointment == null)
                {
                    _logger.LogWarning("CompleteAppointment failed: AppointmentId {AppointmentId} not found.", appointmentId);
                    throw new NotFoundException("Appointment not found.");
                }

                if (appointment.DoctorId != callerId)
                {
                    _logger.LogWarning("Security violation! DoctorId {CallerId} blocked from completing AppointmentId {AppointmentId} (belongs to DoctorId {OwnerId}).",
                        callerId, appointmentId, appointment.DoctorId);
                    throw new ForbiddenException("You are not authorized to complete this appointment.");
                }

                if (appointment.Status != AppointmentStatus.Confirmed)
                {
                    _logger.LogWarning("CompleteAppointment failed: AppointmentId {AppointmentId} is {Status}, expected Confirmed.",
                        appointmentId, appointment.Status);
                    throw new ConflictException($"Only confirmed appointments can be marked complete. Current status: {appointment.Status}.");
                }

                appointment.Status = AppointmentStatus.Completed;
                await _appointmentRepository.UpdateAsync(appointment);

                _logger.LogInformation("CompleteAppointment succeeded: AppointmentId {AppointmentId}.", appointmentId);

                return MapToResponse(appointment);
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while completing AppointmentId {AppointmentId}.", appointmentId);
                throw;
            }
        }

        private static DateTime GetSlotStartDateTime(DateTime appointmentDate, string timeSlot)
        {
            var startPart = timeSlot.Split('-')[0].Trim();   // "10:00-10:30" → "10:00"
            var startTime = TimeSpan.Parse(startPart);
            return appointmentDate.Date + startTime;
        }

        /// <summary>
        /// Validates that a "HH:mm-HH:mm" slot string:
        ///  - parses cleanly on both ends,
        ///  - falls entirely within business hours (WorkStart..WorkEnd),
        ///  - is exactly one SlotDuration long,
        ///  - and starts on a boundary aligned to the generated slot grid.
        /// This guards BookAppointmentAsync/RescheduleAsync against slot values
        /// (e.g. midnight, or a misaligned/oversized range) that would never
        /// have been offered by GetAvailableSlotsAsync in the first place.
        /// </summary>
        private static bool IsValidSlot(string timeSlot)
        {
            if (string.IsNullOrWhiteSpace(timeSlot))
                return false;

            var parts = timeSlot.Split('-');
            if (parts.Length != 2)
                return false;

            if (!TimeSpan.TryParse(parts[0].Trim(), out var start))
                return false;

            if (!TimeSpan.TryParse(parts[1].Trim(), out var end))
                return false;

            if (start < WorkStart || end > WorkEnd)
                return false;

            if (end - start != SlotDuration)
                return false;

            // Must land exactly on a grid boundary (09:00, 09:30, 10:00, ...)
            var offsetTicks = (start - WorkStart).Ticks;
            if (offsetTicks < 0 || offsetTicks % SlotDuration.Ticks != 0)
                return false;

            return true;
        }

        public async Task<List<string>> GetAvailableSlotsAsync(int doctorId, DateTime date, int? excludeAppointmentId = null)
        {
            var bookedSlots = await _appointmentRepository.GetBookedSlotsAsync(doctorId, date, excludeAppointmentId);
            var allSlots = new List<string>();

            var cursor = WorkStart;
            while (cursor + SlotDuration <= WorkEnd)
            {
                var slotEnd = cursor + SlotDuration;
                allSlots.Add($"{cursor:hh\\:mm}-{slotEnd:hh\\:mm}");
                cursor = slotEnd;
            }

            var earliestBookable = NowInIst().AddMinutes(MinimumBookingLeadTimeMinutes);
            var isToday = date.Date == NowInIst().Date;

            return allSlots
                .Where(s => !bookedSlots.Contains(s))
                .Where(s => !isToday || (date.Date + TimeSpan.Parse(s.Split('-')[0])) > earliestBookable)
                .ToList();
        }
        private static AppointmentResponse MapToResponse(Appointment appointment)
        {
            return new AppointmentResponse
            {
                AppointmentId = appointment.AppointmentId,
                PatientId = appointment.PatientId,
                PatientName = appointment.Patient?.FullName ?? string.Empty,
                DoctorId = appointment.DoctorId,
                DoctorName = appointment.Doctor?.Name ?? string.Empty,
                AppointmentDate = appointment.AppointmentDate,
                TimeSlot = appointment.TimeSlot,
                Reason = appointment.Reason,
                VisitType = appointment.VisitType.ToString(),
                Status = appointment.Status.ToString(),
                CancellationReason = appointment.CancellationReason,
                HasConsultation = appointment.Consultation != null,
                CreatedAt = appointment.CreatedAt
            };
        }
    }
}