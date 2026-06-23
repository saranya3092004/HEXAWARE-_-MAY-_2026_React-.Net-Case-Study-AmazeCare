using AmazeCare.Server.DTOs;
using AmazeCare.Server.Models;
using AmazeCare.Server.Modules.Auth.DTOs;
using AmazeCare.Server.Modules.Auth.Repository.Interface;
using AmazeCare.Server.Modules.Auth.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace AmazeCare.Server.Modules.Auth.Services.Implementation

{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IJwtService _jwtService;
        private readonly IMemoryCache _memoryCache;
        private readonly IHostEnvironment _environment;
        private readonly ILogger<AuthService> _logger;

        //private const int OtpExpirySeconds = 300; //created time span for expiry
        //private const string OtpCachePrefix = "otp_"; // created because we might also stroe they have same format so accessing it would be confusing so if prefix is added we can idenity particular data

        public AuthService(IAuthRepository authRepository,IJwtService jwtService,IMemoryCache memoryCache, IHostEnvironment environment, ILogger<AuthService> logger) 
        { 
            _authRepository = authRepository;
            _jwtService = jwtService;
            _memoryCache = memoryCache;
            _environment = environment;
            _logger = logger;
        }

        public async Task<LoginResponse> EmailLoginAsync(EmailLoginRequest request)
        {
            var user = await _authRepository.GetUserByEmailAsync(request.Email);
            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
            {
                _logger.LogWarning("EmailLogin failed: no account or no password set for email {Email}.", request.Email);
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))// verifies the password and the hashed are same by extracting the salt from hash and applies it to password and verifies whether it generates the same hash 
                                                                              // the hashed password contains password salt -> random value generated and a cost
            {
                _logger.LogWarning("EmailLogin failed: incorrect password for UserId {UserId}, email {Email}.", user.UserId, request.Email);
                throw new UnauthorizedAccessException("Invalid emial or password.");
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("EmailLogin failed: account deactivated for UserId {UserId}.", user.UserId);
                throw new UnauthorizedAccessException("This account has been deactivated.");
            }

            _logger.LogInformation("EmailLogin succeeded for UserId {UserId}, email {Email}.", user.UserId, request.Email);
            return await BuildLoginResponseAsync(user);
        }

        public async Task<LoginResponse> RegisterPatientAsync(RegisterPatientRequest request)
        {
            if (await _authRepository.PhoneExistsAsync(request.PhoneNumber))
            {
                _logger.LogWarning("RegisterPatient failed: phone {PhoneNumber} already registered.", request.PhoneNumber);
                throw new InvalidOperationException("An account with this phone number already exists.");
            }

            if (!string.IsNullOrEmpty(request.Email) && await _authRepository.EmailExistsAsync(request.Email))
            {
                _logger.LogWarning("RegisterPatient failed: email {Email} already registered.", request.Email);
                throw new InvalidOperationException("An account with this email already exists.");
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


            int count = await _authRepository.GetPatientCountAsync();
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

        public async Task ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            if (request.NewPassword != request.ConfirmNewPassword)
            {
                _logger.LogWarning("ChangePassword failed: confirmation mismatch for UserId {UserId}.", userId);
                throw new ArgumentException("New password and confirmation do not match.");
            }

            var user = await _authRepository.GetUserByUserIdAsync(userId);
            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
            {
                _logger.LogError("ChangePassword failed: UserId {UserId} not found.", userId);
                throw new KeyNotFoundException("User not found.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                _logger.LogWarning("ChangePassword failed: incorrect current password for UserId {UserId}.", userId);
                throw new UnauthorizedAccessException("Current password is incorrect.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _authRepository.UpdateUserAsync(user);

            _logger.LogInformation("ChangePassword succeeded for UserId {UserId}.", userId);
        }



        private async Task<LoginResponse> BuildLoginResponseAsync(User user)
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
    }
}
