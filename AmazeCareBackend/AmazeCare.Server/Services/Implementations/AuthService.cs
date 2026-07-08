using AmazeCare.Server.DTOs;
using AmazeCare.Server.Models;
using AmazeCare.Server.Modules.Auth.DTOs;
using AmazeCare.Server.Modules.Auth.Repository.Interface;
using AmazeCare.Server.Modules.Auth.Services.Interface;
using AmazeCare.Server.Modules.Middlewares;
using Microsoft.Extensions.Caching.Memory;

namespace AmazeCare.Server.Modules.Auth.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IAuthRepository authRepository, IJwtService jwtService, ILogger<AuthService> logger)
        {
            _authRepository = authRepository;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<LoginResponse> EmailLoginAsync(EmailLoginRequest request)
        {
            try
            {
                var user = await _authRepository.GetUserByEmailAsync(request.Email);
                if (user == null || string.IsNullOrEmpty(user.PasswordHash))
                {
                    _logger.LogWarning("EmailLogin failed: no account or no password set for email {Email}.", request.Email);
                    throw new UnauthorizedAccessException("Invalid email or password.");
                }

                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    _logger.LogWarning("EmailLogin failed: incorrect password for UserId {UserId}, email {Email}.", user.UserId, request.Email);
                    throw new UnauthorizedAccessException("Invalid email or password.");
                }

                if (!user.IsActive)
                {
                    _logger.LogWarning("EmailLogin failed: account deactivated for UserId {UserId}.", user.UserId);
                    throw new UnauthorizedAccessException("This account has been deactivated.");
                }

                _logger.LogInformation("EmailLogin succeeded for UserId {UserId}, email {Email}.", user.UserId, request.Email);
                return await BuildLoginResponseAsync(user);
            }
            catch (AppException)
            {
                throw;
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during EmailLogin for email {Email}.", request.Email);
                throw;
            }
        }

        public async Task<LoginResponse> RegisterPatientAsync(RegisterPatientRequest request)
        {
            try
            {
                if (request.DateOfBirth > DateOnly.FromDateTime(DateTime.Today))
                {
                    throw new BadRequestException("Date of birth cannot be in the future.");
                }

                if (await _authRepository.PhoneExistsAsync(request.PhoneNumber))
                {
                    _logger.LogWarning("RegisterPatient failed: phone {PhoneNumber} already registered.", request.PhoneNumber);
                    throw new ConflictException("An account with this phone number already exists.");
                }

                if (!string.IsNullOrEmpty(request.Email) && await _authRepository.EmailExistsAsync(request.Email))
                {
                    _logger.LogWarning("RegisterPatient failed: email {Email} already registered.", request.Email);
                    throw new ConflictException("An account with this email already exists.");
                }

                string? passwordHash = !string.IsNullOrEmpty(request.Password)
                    ? BCrypt.Net.BCrypt.HashPassword(request.Password)
                    : null;

                var user = new User
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    PasswordHash = passwordHash,
                    Role = UserRole.User,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                user = await _authRepository.CreateUserAsync(user);

                var patient = new Patient
                {
                    UserId = user.UserId,
                    FullName = request.FullName,
                    DateOfBirth = request.DateOfBirth.ToDateTime(TimeOnly.MinValue),
                    Gender = request.Gender,
                    PhoneNumber = request.PhoneNumber,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                patient = await _authRepository.CreatePatientAsync(patient);

                _logger.LogInformation("RegisterPatient succeeded: UserId {UserId}, PatientId {PatientId}.",
                    user.UserId, patient.PatientId);

                return _jwtService.BuildToken(user, patient.PatientId);
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during RegisterPatient for phone {PhoneNumber}.", request.PhoneNumber);
                throw;
            }
        }


        private async Task<LoginResponse> BuildLoginResponseAsync(User user)
        {
            try
            {
                int? roleSpecificId = null;

                switch (user.Role)
                {
                    case UserRole.User:
                        var patient = await _authRepository.GetPatientByUserIdAsync(user.UserId);
                        roleSpecificId = patient?.PatientId;
                        break;
                    case UserRole.Doctor:
                        roleSpecificId = user.Doctor?.DoctorId;
                        break;
                    case UserRole.Admin:
                        roleSpecificId = user.Admin?.AdminId;
                        break;
                }

                return _jwtService.BuildToken(user, roleSpecificId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error building login response for UserId {UserId}.", user.UserId);
                throw;
            }
        }
    }
}