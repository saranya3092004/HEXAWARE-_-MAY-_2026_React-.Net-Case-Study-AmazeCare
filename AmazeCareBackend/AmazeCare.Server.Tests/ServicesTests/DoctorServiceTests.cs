using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using AmazeCare.Server.Models;
using AmazeCare.Server.Modules.Auth.Repository.Interface;
using AmazeCare.Server.Modules.DoctorModule.DTOs;
using AmazeCare.Server.Modules.DoctorModule.Repository;
using AmazeCare.Server.Modules.DoctorModule.Service;
using AmazeCare.Server.Modules.Middlewares;

namespace AmazeCare.Server.Tests.Services
{
    [TestFixture]
    public class DoctorServiceTests
    {
        private Mock<IDoctorRepository> _doctorRepoMock = null!;
        private Mock<IAuthRepository> _authRepoMock = null!;
        private Mock<ILogger<DoctorService>> _loggerMock = null!;
        private DoctorService _service = null!;

        [SetUp]
        public void Setup()
        {
            _doctorRepoMock = new Mock<IDoctorRepository>();
            _authRepoMock = new Mock<IAuthRepository>();
            _loggerMock = new Mock<ILogger<DoctorService>>();
            _service = new DoctorService(_doctorRepoMock.Object, _authRepoMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task DeactivateDoctorAsync_ActiveDoctor_DeactivatesDoctorAndLinkedUser()
        {
            var doctor = new Doctor { DoctorId = 1, UserId = 42, IsActive = true };
            var user = new User { UserId = 42, IsActive = true };

            _doctorRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(doctor);
            _authRepoMock.Setup(r => r.GetUserByUserIdAsync(42)).ReturnsAsync(user);

            await _service.DeactivateDoctorAsync(1);

            Assert.That(doctor.IsActive, Is.False);
            Assert.That(user.IsActive, Is.False);
            _doctorRepoMock.Verify(r => r.UpdateAsync(doctor), Times.Once);
            _authRepoMock.Verify(r => r.UpdateUserAsync(user), Times.Once);
        }

        [Test]
        public void DeactivateDoctorAsync_AlreadyInactive_ThrowsConflictException()
        {
            var doctor = new Doctor { DoctorId = 1, IsActive = false };
            _doctorRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(doctor);

            Assert.ThrowsAsync<ConflictException>(async () =>
                await _service.DeactivateDoctorAsync(1));
        }

        [Test]
        public void DeactivateDoctorAsync_NotFound_ThrowsNotFoundException()
        {
            _doctorRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Doctor?)null);

            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _service.DeactivateDoctorAsync(99));
        }

        [Test]
        public async Task DeactivateDoctorAsync_LinkedUserMissing_StillDeactivatesDoctor()
        {
            var doctor = new Doctor { DoctorId = 1, UserId = 42, IsActive = true };
            _doctorRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(doctor);
            _authRepoMock.Setup(r => r.GetUserByUserIdAsync(42)).ReturnsAsync((User?)null);

            await _service.DeactivateDoctorAsync(1);

            Assert.That(doctor.IsActive, Is.False);
            _authRepoMock.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public async Task CreateDoctorAsync_EmailAlreadyExists_ThrowsConflictException()
        {
            _doctorRepoMock.Setup(r => r.UserEmailExistsAsync("doc@x.com")).ReturnsAsync(true);
            var request = new CreateDoctorRequest { Email = "doc@x.com", PhoneNumber = "9876543210", Name = "Dr X", Password = "pass1234" };

            Assert.ThrowsAsync<ConflictException>(async () =>
                await _service.CreateDoctorAsync(request));
        }

        [Test]
        public async Task CreateDoctorAsync_PhoneAlreadyExists_ThrowsConflictException()
        {
            _doctorRepoMock.Setup(r => r.UserEmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _doctorRepoMock.Setup(r => r.UserPhoneExistsAsync("9876543210")).ReturnsAsync(true);
            var request = new CreateDoctorRequest { Email = "doc@x.com", PhoneNumber = "9876543210", Name = "Dr X", Password = "pass1234" };

            Assert.ThrowsAsync<ConflictException>(async () =>
                await _service.CreateDoctorAsync(request));
        }

        [Test]
        public async Task GetProfileAsync_NotFound_ThrowsNotFoundException()
        {
            _doctorRepoMock.Setup(r => r.GetByIdWithProfileAsync(1)).ReturnsAsync((Doctor?)null);

            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _service.GetProfileAsync(1));
        }

        [Test]
        public async Task GetSpecializationsAsync_ReturnsDistinctList()
        {
            _doctorRepoMock.Setup(r => r.GetDistinctSpecializationsAsync())
                .ReturnsAsync(new List<string> { "Cardiology", "ENT" });

            var result = await _service.GetSpecializationsAsync();

            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result, Contains.Item("Cardiology"));
        }
    }
}