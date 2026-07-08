using AmazeCare.Server.Models;
using AmazeCare.Server.Modules.Auth.Repository.Interface;
using AmazeCare.Server.Modules.DoctorModule.DTOs;
using AmazeCare.Server.Modules.DoctorModule.Repository;
using AmazeCare.Server.Modules.Middlewares;

namespace AmazeCare.Server.Modules.DoctorModule.Service
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IAuthRepository _authRepository;
        private readonly ILogger<DoctorService> _logger;

        public DoctorService(IDoctorRepository doctorRepository, IAuthRepository authRepository, ILogger<DoctorService> logger)
        {
            _doctorRepository = doctorRepository;
            _authRepository = authRepository;
            _logger = logger;
        }

        public async Task<List<DoctorResponse>> SearchAsync(string? name, string? specialization)
        {
            try
            {
                var doctors = await _doctorRepository.SearchAsync(name, specialization);

                _logger.LogInformation("DoctorSearch: Name={Name}, Specialization={Specialization}, ResultCount={Count}.",
                    name, specialization, doctors.Count);

                return doctors.Select(MapToResponse).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in SearchAsync: Name={Name}, Specialization={Specialization}.", name, specialization);
                throw;
            }
        }

        public async Task<DoctorResponse> GetProfileAsync(int doctorId)
        {
            try
            {
                var doctor = await _doctorRepository.GetByIdWithProfileAsync(doctorId);
                if (doctor == null)
                {
                    _logger.LogWarning("GetProfile failed: DoctorId {DoctorId} not found.", doctorId);
                    throw new NotFoundException("Doctor not found.");
                }

                return new DoctorResponse
                {
                    DoctorId = doctor.DoctorId,
                    Name = doctor.Name,
                    Specialization = doctor.Specialization,
                    Qualification = doctor.Qualification,
                    Designation = doctor.Designation,
                    ExperienceYears = doctor.ExperienceYears,
                    IsActive = doctor.IsActive
                };
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetProfileAsync for DoctorId {DoctorId}.", doctorId);
                throw;
            }
        }

        public async Task<List<AppointmentSummary>> GetDoctorAppointmentsAsync(int doctorId, bool upcomingOnly)
        {
            try
            {
                var doctor = await _doctorRepository.GetByIdAsync(doctorId);
                if (doctor == null)
                {
                    _logger.LogWarning("GetDoctorAppointments failed: DoctorId {DoctorId} not found.", doctorId);
                    throw new NotFoundException("Doctor not found.");
                }

                var appointments = await _doctorRepository.GetAppointmentsForDoctorAsync(doctorId, upcomingOnly);

                _logger.LogInformation("GetDoctorAppointments: DoctorId={DoctorId}, UpcomingOnly={UpcomingOnly}, Count={Count}.",
                    doctorId, upcomingOnly, appointments.Count);

                return appointments.Select(a => new AppointmentSummary
                {
                    AppointmentId = a.AppointmentId,
                    DoctorId = a.DoctorId,
                    PatientName = a.Patient.FullName,
                    AppointmentDate = a.AppointmentDate,
                    TimeSlot = a.TimeSlot,
                    Reason=a.Reason,
                    Status = a.Status.ToString()
                }).ToList();
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetDoctorAppointmentsAsync for DoctorId {DoctorId}.", doctorId);
                throw;
            }
        }

        public async Task<DoctorResponse> CreateDoctorAsync(CreateDoctorRequest request)
        {
            try
            {
                if (await _doctorRepository.UserEmailExistsAsync(request.Email))
                {
                    _logger.LogWarning("CreateDoctor failed: email {Email} already in use.", request.Email);
                    throw new ConflictException("A user with this email already exists.");
                }

                if (await _doctorRepository.UserPhoneExistsAsync(request.PhoneNumber))
                {
                    _logger.LogWarning("CreateDoctor failed: phone {Phone} already in use.", request.PhoneNumber);
                    throw new ConflictException("A user with this phone number already exists.");
                }

                var user = new User
                {
                    FullName = request.Name,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Role = UserRole.Doctor,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                user = await _doctorRepository.AddUserAsync(user);

                var doctor = new Doctor
                {
                    UserId = user.UserId,
                    Name = request.Name,
                    Specialization = request.Specialization,
                    Qualification = request.Qualification,
                    Designation = request.Designation,
                    ExperienceYears = request.ExperienceYears,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                doctor = await _doctorRepository.AddAsync(doctor);

                _logger.LogInformation("CreateDoctor succeeded: DoctorId={DoctorId}, UserId={UserId}.",
                    doctor.DoctorId, user.UserId);

                return MapToResponse(doctor);
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in CreateDoctorAsync for Name {Name}.", request.Name);
                throw;
            }
        }

        public async Task<DoctorResponse> UpdateDoctorAsync(int doctorId, UpdateDoctorRequest request)
        {
            try
            {
                var doctor = await _doctorRepository.GetByIdAsync(doctorId);
                if (doctor == null)
                {
                    _logger.LogWarning("UpdateDoctor failed: DoctorId {DoctorId} not found.", doctorId);
                    throw new NotFoundException("Doctor not found.");
                }

                doctor.Name = request.Name;
                doctor.Specialization = request.Specialization;
                doctor.Qualification = request.Qualification;
                doctor.Designation = request.Designation;
                doctor.ExperienceYears = request.ExperienceYears;

                await _doctorRepository.UpdateAsync(doctor);

                _logger.LogInformation("UpdateDoctor succeeded: DoctorId {DoctorId}.", doctorId);

                return MapToResponse(doctor);
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in UpdateDoctorAsync for DoctorId {DoctorId}.", doctorId);
                throw;
            }
        }

        public async Task DeactivateDoctorAsync(int doctorId)
        {
            try
            {
                var doctor = await _doctorRepository.GetByIdAsync(doctorId);
                if (doctor == null)
                {
                    _logger.LogWarning("DeactivateDoctor failed: DoctorId {DoctorId} not found.", doctorId);
                    throw new NotFoundException("Doctor not found.");
                }

                if (!doctor.IsActive)
                {
                    _logger.LogWarning("DeactivateDoctor skipped: DoctorId {DoctorId} already inactive.", doctorId);
                    throw new ConflictException("This doctor is already deactivated.");
                }

                doctor.IsActive = false;
                await _doctorRepository.UpdateAsync(doctor);

                // Also deactivate the linked User account so login is blocked too
                var user = await _authRepository.GetUserByUserIdAsync(doctor.UserId);
                if (user != null)
                {
                    user.IsActive = false;
                    await _authRepository.UpdateUserAsync(user);
                }

                _logger.LogInformation("DeactivateDoctor succeeded: DoctorId {DoctorId}, UserId {UserId}.", doctorId, doctor.UserId);
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in DeactivateDoctorAsync for DoctorId {DoctorId}.", doctorId);
                throw;
            }
        }

        public async Task<List<string>> GetSpecializationsAsync()
        {
            return await _doctorRepository.GetDistinctSpecializationsAsync();
        }

        internal static DoctorResponse MapToResponse(Doctor doctor)
        {
            return new DoctorResponse
            {
                DoctorId = doctor.DoctorId,
                Name = doctor.Name,
                Specialization = doctor.Specialization,
                Qualification = doctor.Qualification,
                Designation = doctor.Designation,
                ExperienceYears = doctor.ExperienceYears,
                IsActive = doctor.IsActive
            };
        }
    }
}