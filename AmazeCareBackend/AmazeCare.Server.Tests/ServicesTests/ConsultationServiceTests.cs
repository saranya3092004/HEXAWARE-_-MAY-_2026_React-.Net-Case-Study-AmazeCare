using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using AmazeCare.Server.Models;
using AmazeCare.Server.DTOs.ConsultationDtos;
using AmazeCare.Server.Modules.Middlewares;
using AmazeCare.Server.Repository.Interfaces;
using AmazeCare.Server.Services.Implementations;

namespace AmazeCare.Server.Tests.Services
{
    [TestFixture]
    public class ConsultationServiceTests
    {
        private Mock<IConsultationRepository> _consultRepoMock = null!;
        private Mock<IAppointmentRepository> _apptRepoMock = null!;
        private Mock<ILogger<ConsultationService>> _loggerMock = null!;
        private ConsultationService _service = null!;

        [SetUp]
        public void Setup()
        {
            _consultRepoMock = new Mock<IConsultationRepository>();
            _apptRepoMock = new Mock<IAppointmentRepository>();
            _loggerMock = new Mock<ILogger<ConsultationService>>();
            _service = new ConsultationService(_consultRepoMock.Object, _apptRepoMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task CreateAsync_ConfirmedAppointmentNoExistingConsultation_CreatesAndCompletesAppointment()
        {
            var appointment = new Appointment { AppointmentId = 1, DoctorId = 5, PatientId = 10, Status = AppointmentStatus.Confirmed };
            _apptRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(appointment);
            _consultRepoMock.Setup(r => r.GetByAppointmentIdAsync(1)).ReturnsAsync((Consultation?)null);
            _consultRepoMock.Setup(r => r.AddAsync(It.IsAny<Consultation>()))
                .ReturnsAsync((Consultation c) => { c.ConsultationId = 100; return c; });
            _consultRepoMock.Setup(r => r.GetByIdAsync(100)).ReturnsAsync(
                new Consultation { ConsultationId = 100, AppointmentId = 1, PatientId = 10, DoctorId = 5 });

            var request = new CreateConsultationRequest
            {
                AppointmentId = 1,
                CurrentSymptoms = "Fever",
                Diagnosis = "Flu"
            };

            var result = await _service.CreateAsync(doctorId: 5, request);

            Assert.That(appointment.Status, Is.EqualTo(AppointmentStatus.Completed));
            _apptRepoMock.Verify(r => r.UpdateAsync(appointment), Times.Once);
            _consultRepoMock.Verify(r => r.AddAsync(It.IsAny<Consultation>()), Times.Once);
        }

        [Test]
        public void CreateAsync_WrongDoctor_ThrowsForbiddenException()
        {
            var appointment = new Appointment { AppointmentId = 1, DoctorId = 5, Status = AppointmentStatus.Confirmed };
            _apptRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(appointment);

            var request = new CreateConsultationRequest { AppointmentId = 1, CurrentSymptoms = "x" };

            Assert.ThrowsAsync<ForbiddenException>(async () =>
                await _service.CreateAsync(doctorId: 999, request));
        }

        [Test]
        public void CreateAsync_AppointmentNotConfirmed_ThrowsConflictException()
        {
            var appointment = new Appointment { AppointmentId = 1, DoctorId = 5, Status = AppointmentStatus.Pending };
            _apptRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(appointment);

            var request = new CreateConsultationRequest { AppointmentId = 1, CurrentSymptoms = "x" };

            Assert.ThrowsAsync<ConflictException>(async () =>
                await _service.CreateAsync(doctorId: 5, request));
        }

        [Test]
        public void CreateAsync_ConsultationAlreadyExists_ThrowsConflictException()
        {
            var appointment = new Appointment { AppointmentId = 1, DoctorId = 5, Status = AppointmentStatus.Confirmed };
            _apptRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(appointment);
            _consultRepoMock.Setup(r => r.GetByAppointmentIdAsync(1))
                .ReturnsAsync(new Consultation { ConsultationId = 5, AppointmentId = 1 });

            var request = new CreateConsultationRequest { AppointmentId = 1, CurrentSymptoms = "x" };

            Assert.ThrowsAsync<ConflictException>(async () =>
                await _service.CreateAsync(doctorId: 5, request));
        }

        [Test]
        public void CreateAsync_AppointmentNotFound_ThrowsNotFoundException()
        {
            _apptRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Appointment?)null);
            var request = new CreateConsultationRequest { AppointmentId = 99, CurrentSymptoms = "x" };

            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _service.CreateAsync(doctorId: 5, request));
        }

        [Test]
        public async Task UpdateAsync_OwnerDoctor_UpdatesFields()
        {
            var consultation = new Consultation { ConsultationId = 1, DoctorId = 5, Diagnosis = "Old" };
            _consultRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(consultation);

            var request = new UpdateConsultationRequest { CurrentSymptoms = "Updated", Diagnosis = "New Diagnosis" };
            await _service.UpdateAsync(1, doctorId: 5, request);

            Assert.That(consultation.Diagnosis, Is.EqualTo("New Diagnosis"));
        }

        [Test]
        public void UpdateAsync_WrongDoctor_ThrowsForbiddenException()
        {
            var consultation = new Consultation { ConsultationId = 1, DoctorId = 5 };
            _consultRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(consultation);

            var request = new UpdateConsultationRequest { CurrentSymptoms = "x" };

            Assert.ThrowsAsync<ForbiddenException>(async () =>
                await _service.UpdateAsync(1, doctorId: 999, request));
        }
    }
}