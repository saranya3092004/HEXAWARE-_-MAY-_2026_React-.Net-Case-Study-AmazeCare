using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using AmazeCare.Server.Models;
using AmazeCare.Server.Repository.Interfaces;
using AmazeCare.Server.Services.Implementations;
using AmazeCare.Server.Modules.Middlewares;
using AmazeCare.Server.DTOs.Appointment_Dtos;

namespace AmazeCare.Server.Tests.Services
{
    [TestFixture]
    public class AppointmentServiceTests
    {
        private Mock<IAppointmentRepository> _repoMock = null!;
        private Mock<ILogger<AppointmentService>> _loggerMock = null!;
        private AppointmentService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IAppointmentRepository>();
            _loggerMock = new Mock<ILogger<AppointmentService>>();
            _service = new AppointmentService(_repoMock.Object, _loggerMock.Object);
        }

        // ── ConfirmAsync ──
        [Test]
        public async Task ConfirmAsync_PendingAppointment_SetsStatusToConfirmed()
        {
            var appointment = new Appointment { AppointmentId = 1, DoctorId = 5, Status = AppointmentStatus.Pending };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(appointment);

            await _service.ConfirmAsync(1, callerId: 5, isAdmin: false);

            Assert.That(appointment.Status, Is.EqualTo(AppointmentStatus.Confirmed));
            _repoMock.Verify(r => r.UpdateAsync(appointment), Times.Once);
        }

        [Test]
        public async Task ConfirmAsync_RescheduledAppointment_SetsStatusToConfirmed()
        {
            var appointment = new Appointment { AppointmentId = 1, DoctorId = 5, Status = AppointmentStatus.Rescheduled };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(appointment);

            await _service.ConfirmAsync(1, callerId: 5, isAdmin: false);

            Assert.That(appointment.Status, Is.EqualTo(AppointmentStatus.Confirmed));
        }

        [Test]
        public void ConfirmAsync_WrongDoctor_ThrowsForbiddenException()
        {
            var appointment = new Appointment { AppointmentId = 1, DoctorId = 5, Status = AppointmentStatus.Pending };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(appointment);

            Assert.ThrowsAsync<ForbiddenException>(async () =>
                await _service.ConfirmAsync(1, callerId: 999, isAdmin: false));
        }

        [Test]
        public void ConfirmAsync_AlreadyConfirmed_ThrowsConflictException()
        {
            var appointment = new Appointment { AppointmentId = 1, DoctorId = 5, Status = AppointmentStatus.Confirmed };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(appointment);

            Assert.ThrowsAsync<ConflictException>(async () =>
                await _service.ConfirmAsync(1, callerId: 5, isAdmin: false));
        }

        [Test]
        public void ConfirmAsync_NotFound_ThrowsNotFoundException()
        {
            _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Appointment?)null);

            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _service.ConfirmAsync(99, callerId: 5, isAdmin: false));
        }

        [Test]
        public async Task ConfirmAsync_Admin_CanConfirmAnyDoctorsAppointment()
        {
            var appointment = new Appointment { AppointmentId = 1, DoctorId = 5, Status = AppointmentStatus.Pending };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(appointment);

            await _service.ConfirmAsync(1, callerId: 999, isAdmin: true);

            Assert.That(appointment.Status, Is.EqualTo(AppointmentStatus.Confirmed));
        }

        // ── RejectAsync ──
        [Test]
        public async Task RejectAsync_PendingAppointment_SetsStatusAndReason()
        {
            var appointment = new Appointment { AppointmentId = 1, DoctorId = 5, Status = AppointmentStatus.Pending };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(appointment);
            var request = new RejectAppointmentRequest { Reason = "Doctor unavailable" };

            await _service.RejectAsync(1, callerId: 5, isAdmin: false, request);

            Assert.That(appointment.Status, Is.EqualTo(AppointmentStatus.Rejected));
            Assert.That(appointment.CancellationReason, Is.EqualTo("Doctor unavailable"));
        }

        [Test]
        public void RejectAsync_AlreadyCompleted_ThrowsConflictException()
        {
            var appointment = new Appointment { AppointmentId = 1, DoctorId = 5, Status = AppointmentStatus.Completed };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(appointment);

            Assert.ThrowsAsync<ConflictException>(async () =>
                await _service.RejectAsync(1, callerId: 5, isAdmin: false, new RejectAppointmentRequest { Reason = "x" }));
        }

        // ── CancelAsync ──
        [Test]
        public async Task CancelAsync_PatientCancelsOwnAppointment_Succeeds()
        {
            var appointment = new Appointment { AppointmentId = 1, PatientId = 10, Status = AppointmentStatus.Confirmed };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(appointment);

            await _service.CancelAsync(1, callerId: 10, isAdmin: false, new CancelAppointmentRequest { Reason = "Can't make it" });

            Assert.That(appointment.Status, Is.EqualTo(AppointmentStatus.Cancelled));
        }

        [Test]
        public void CancelAsync_WrongPatient_ThrowsForbiddenException()
        {
            var appointment = new Appointment { AppointmentId = 1, PatientId = 10, Status = AppointmentStatus.Confirmed };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(appointment);

            Assert.ThrowsAsync<ForbiddenException>(async () =>
                await _service.CancelAsync(1, callerId: 999, isAdmin: false, new CancelAppointmentRequest { Reason = "x" }));
        }

        [TestCase(AppointmentStatus.Completed)]
        [TestCase(AppointmentStatus.Cancelled)]
        [TestCase(AppointmentStatus.Rejected)]
        public void CancelAsync_TerminalStatus_ThrowsConflictException(AppointmentStatus status)
        {
            var appointment = new Appointment { AppointmentId = 1, PatientId = 10, Status = status };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(appointment);

            Assert.ThrowsAsync<ConflictException>(async () =>
                await _service.CancelAsync(1, callerId: 10, isAdmin: false, new CancelAppointmentRequest { Reason = "x" }));
        }

        // ── BookAppointmentAsync ──
        [Test]
        public void BookAppointmentAsync_MisalignedTimeSlot_ThrowsBadRequestException()
        {
            var request = new CreateAppointmentRequest
            {
                DoctorId = 1,
                AppointmentDate = DateTime.Today.AddDays(1),
                TimeSlot = "09:15-09:45",
                Reason = "Checkup"
            };

            Assert.ThrowsAsync<BadRequestException>(async () =>
                await _service.BookAppointmentAsync(callerId: 1, isAdmin: false, request));
        }

        [Test]
        public void BookAppointmentAsync_SlotOutsideBusinessHours_ThrowsBadRequestException()
        {
            var request = new CreateAppointmentRequest
            {
                DoctorId = 1,
                AppointmentDate = DateTime.Today.AddDays(1),
                TimeSlot = "18:00-18:30",
                Reason = "Checkup"
            };

            Assert.ThrowsAsync<BadRequestException>(async () =>
                await _service.BookAppointmentAsync(callerId: 1, isAdmin: false, request));
        }

        [Test]
        public void BookAppointmentAsync_AdminWithoutPatientId_ThrowsBadRequestException()
        {
            var request = new CreateAppointmentRequest
            {
                DoctorId = 1,
                AppointmentDate = DateTime.Today.AddDays(1),
                TimeSlot = "10:00-10:30",
                Reason = "Checkup",
                PatientId = null
            };

            Assert.ThrowsAsync<BadRequestException>(async () =>
                await _service.BookAppointmentAsync(callerId: 1, isAdmin: true, request));
        }

        [Test]
        public void BookAppointmentAsync_DoctorNotFound_ThrowsNotFoundException()
        {
            var request = new CreateAppointmentRequest
            {
                DoctorId = 1,
                AppointmentDate = DateTime.Today.AddDays(1),
                TimeSlot = "10:00-10:30",
                Reason = "Checkup"
            };

            _repoMock.Setup(r => r.PatientExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
            _repoMock.Setup(r => r.DoctorExistsAsync(1)).ReturnsAsync(false);

            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _service.BookAppointmentAsync(callerId: 10, isAdmin: false, request));
        }

        [Test]
        public void BookAppointmentAsync_ConflictingSlot_ThrowsConflictException()
        {
            var date = DateTime.Today.AddDays(1);
            var request = new CreateAppointmentRequest
            {
                DoctorId = 1,
                AppointmentDate = date,
                TimeSlot = "10:00-10:30",
                Reason = "Checkup"
            };

            _repoMock.Setup(r => r.PatientExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
            _repoMock.Setup(r => r.DoctorExistsAsync(1)).ReturnsAsync(true);
            _repoMock.Setup(r => r.HasConflictingAppointmentAsync(1, date, "10:00-10:30", null)).ReturnsAsync(true);

            Assert.ThrowsAsync<ConflictException>(async () =>
                await _service.BookAppointmentAsync(callerId: 10, isAdmin: false, request));
        }

        // ── CompleteAsync ──
        [Test]
        public async Task CompleteAsync_ConfirmedAppointment_MarksCompleted()
        {
            var appointment = new Appointment { AppointmentId = 1, DoctorId = 5, Status = AppointmentStatus.Confirmed };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(appointment);

            await _service.CompleteAsync(1, callerId: 5);

            Assert.That(appointment.Status, Is.EqualTo(AppointmentStatus.Completed));
        }

        [Test]
        public void CompleteAsync_NotConfirmed_ThrowsConflictException()
        {
            var appointment = new Appointment { AppointmentId = 1, DoctorId = 5, Status = AppointmentStatus.Pending };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(appointment);

            Assert.ThrowsAsync<ConflictException>(async () =>
                await _service.CompleteAsync(1, callerId: 5));
        }

        // ── GetAvailableSlotsAsync ──
        [Test]
        public async Task GetAvailableSlotsAsync_ExcludesBookedSlots()
        {
            var date = DateTime.Today.AddDays(3);
            _repoMock.Setup(r => r.GetBookedSlotsAsync(1, date, null))
                .ReturnsAsync(new HashSet<string> { "09:00-09:30", "09:30-10:00" });

            var result = await _service.GetAvailableSlotsAsync(1, date);

            Assert.That(result, Does.Not.Contain("09:00-09:30"));
            Assert.That(result, Does.Not.Contain("09:30-10:00"));
            Assert.That(result, Does.Contain("10:00-10:30"));
        }

        [Test]
        public async Task GetAvailableSlotsAsync_NoBookedSlots_ReturnsFullGrid()
        {
            var date = DateTime.Today.AddDays(3);
            _repoMock.Setup(r => r.GetBookedSlotsAsync(1, date, null)).ReturnsAsync(new HashSet<string>());

            var result = await _service.GetAvailableSlotsAsync(1, date);

            // 9:00 to 17:00 in 30-min slots = 16 slots
            Assert.That(result.Count, Is.EqualTo(16));
        }
    }
}