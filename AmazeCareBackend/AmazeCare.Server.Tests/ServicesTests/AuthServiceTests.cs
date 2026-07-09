using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using AmazeCare.Server.Models;
using AmazeCare.Server.Modules.Auth.DTOs;
using AmazeCare.Server.Modules.Auth.Repository.Interface;
using AmazeCare.Server.Modules.Auth.Services.Implementation;
using AmazeCare.Server.Modules.Auth.Services.Interface;
using AmazeCare.Server.Modules.Middlewares;
using AmazeCare.Server.DTOs;

namespace AmazeCare.Server.Tests.Services
{
    [TestFixture]
    public class AuthServiceTests
    {
        private Mock<IAuthRepository> _repoMock = null!;
        private Mock<IJwtService> _jwtMock = null!;
        private Mock<ILogger<AuthService>> _loggerMock = null!;
        private AuthService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IAuthRepository>();
            _jwtMock = new Mock<IJwtService>();
            _loggerMock = new Mock<ILogger<AuthService>>();
            _service = new AuthService(_repoMock.Object, _jwtMock.Object, _loggerMock.Object);
        }

        [Test]
        public void EmailLoginAsync_UserNotFound_ThrowsUnauthorized()
        {
            _repoMock.Setup(r => r.GetUserByEmailAsync("nobody@x.com")).ReturnsAsync((User?)null);

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _service.EmailLoginAsync(new EmailLoginRequest { Email = "nobody@x.com", Password = "x" }));
        }

        [Test]
        public void EmailLoginAsync_WrongPassword_ThrowsUnauthorized()
        {
            var user = new User
            {
                UserId = 1,
                Email = "user@x.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
                IsActive = true,
                Role = UserRole.User
            };
            _repoMock.Setup(r => r.GetUserByEmailAsync("user@x.com")).ReturnsAsync(user);

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _service.EmailLoginAsync(new EmailLoginRequest { Email = "user@x.com", Password = "wrongpassword" }));
        }

        [Test]
        public void EmailLoginAsync_DeactivatedAccount_ThrowsUnauthorized()
        {
            var user = new User
            {
                UserId = 1,
                Email = "user@x.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                IsActive = false,
                Role = UserRole.User
            };
            _repoMock.Setup(r => r.GetUserByEmailAsync("user@x.com")).ReturnsAsync(user);

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _service.EmailLoginAsync(new EmailLoginRequest { Email = "user@x.com", Password = "password123" }));
        }

        [Test]
        public async Task EmailLoginAsync_ValidPatientCredentials_ReturnsLoginResponseWithPatientId()
        {
            var user = new User
            {
                UserId = 1,
                Email = "patient@x.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                IsActive = true,
                Role = UserRole.User
            };
            _repoMock.Setup(r => r.GetUserByEmailAsync("patient@x.com")).ReturnsAsync(user);
            _repoMock.Setup(r => r.GetPatientByUserIdAsync(1)).ReturnsAsync(new Patient { PatientId = 55, UserId = 1, FullName = "P", PhoneNumber = "9876543210" });
            _jwtMock.Setup(j => j.BuildToken(user, 55)).Returns(new LoginResponse { Token = "fake-token", RoleSpecificId = 55 });

            var result = await _service.EmailLoginAsync(new EmailLoginRequest { Email = "patient@x.com", Password = "password123" });

            Assert.That(result.Token, Is.EqualTo("fake-token"));
            _jwtMock.Verify(j => j.BuildToken(user, 55), Times.Once);
        }

        [Test]
        public void RegisterPatientAsync_FutureDateOfBirth_ThrowsBadRequestException()
        {
            var request = new RegisterPatientRequest
            {
                FullName = "Test",
                PhoneNumber = "9876543210",
                DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddDays(5)),
                Gender = Gender.Male,
                Password = "password123"
            };

            Assert.ThrowsAsync<BadRequestException>(async () =>
                await _service.RegisterPatientAsync(request));
        }

        [Test]
        public void RegisterPatientAsync_PhoneAlreadyExists_ThrowsConflictException()
        {
            _repoMock.Setup(r => r.PhoneExistsAsync("9876543210")).ReturnsAsync(true);
            var request = new RegisterPatientRequest
            {
                FullName = "Test",
                PhoneNumber = "9876543210",
                DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-25)),
                Gender = Gender.Male,
                Password = "password123"
            };

            Assert.ThrowsAsync<ConflictException>(async () =>
                await _service.RegisterPatientAsync(request));
        }

        [Test]
        public async Task RegisterPatientAsync_ValidRequest_CreatesUserAndPatient()
        {
            var request = new RegisterPatientRequest
            {
                FullName = "New Patient",
                Email = "new@x.com",
                PhoneNumber = "9876543210",
                DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-25)),
                Gender = Gender.Female,
                Password = "password123"
            };

            _repoMock.Setup(r => r.PhoneExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _repoMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _repoMock.Setup(r => r.CreateUserAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => { u.UserId = 1; return u; });
            _repoMock.Setup(r => r.CreatePatientAsync(It.IsAny<Patient>()))
                .ReturnsAsync((Patient p) => { p.PatientId = 100; return p; });
            _jwtMock.Setup(j => j.BuildToken(It.IsAny<User>(), 100))
                .Returns(new LoginResponse { Token = "new-token" });

            var result = await _service.RegisterPatientAsync(request);

            Assert.That(result.Token, Is.EqualTo("new-token"));
            _repoMock.Verify(r => r.CreateUserAsync(It.Is<User>(u => u.Role == UserRole.User)), Times.Once);
        }
    }
}