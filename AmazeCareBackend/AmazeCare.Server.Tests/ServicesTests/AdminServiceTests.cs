using Moq;
using Microsoft.Extensions.Logging;
using AmazeCare.Server.Models;
using AmazeCare.Server.DTOs.AdminDtos;
using AmazeCare.Server.Modules.AdminModule.Service;
using AmazeCare.Server.Repository.Interfaces;

namespace AmazeCare.Server.Tests.Services
{
    [TestFixture]
    public class AdminServiceTests
    {
        private Mock<IAdminRepository> _repoMock = null!;
        private Mock<ILogger<AdminService>> _loggerMock = null!;
        private AdminService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IAdminRepository>();
            _loggerMock = new Mock<ILogger<AdminService>>();
            _service = new AdminService(_repoMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetDashboardAsync_AggregatesAllCounts()
        {
            _repoMock.Setup(r => r.GetTotalPatientsAsync()).ReturnsAsync(50);
            _repoMock.Setup(r => r.GetTotalDoctorsAsync()).ReturnsAsync(8);
            _repoMock.Setup(r => r.GetTodaysAppointmentsCountAsync()).ReturnsAsync(12);
            _repoMock.Setup(r => r.GetPendingAppointmentsCountAsync()).ReturnsAsync(3);

            var result = await _service.GetDashboardAsync();

            Assert.That(result.TotalPatients, Is.EqualTo(50));
            Assert.That(result.TotalDoctors, Is.EqualTo(8));
            Assert.That(result.TodaysAppointments, Is.EqualTo(12));
            Assert.That(result.PendingAppointments, Is.EqualTo(3));
        }

        [Test]
        public async Task GetAppointmentReportAsync_MapsFieldsCorrectly()
        {
            var appointments = new List<Appointment>
            {
                new()
                {
                    AppointmentId = 1,
                    Patient = new Patient { FullName = "Patient A", PhoneNumber = "9876543210" },
                    Doctor = new Doctor { Name = "Dr B" },
                    AppointmentDate = DateTime.Today,
                    TimeSlot = "10:00-10:30",
                    Status = AppointmentStatus.Confirmed
                }
            };

            _repoMock.Setup(r => r.GetAppointmentReportAsync(null, null, null, null))
                .ReturnsAsync(appointments);

            var result = await _service.GetAppointmentReportAsync(new AppointmentReportRequest());

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].PatientName, Is.EqualTo("Patient A"));
            Assert.That(result[0].DoctorName, Is.EqualTo("Dr B"));
            Assert.That(result[0].Status, Is.EqualTo("Confirmed"));
        }

        [Test]
        public async Task GetPatientReportAsync_MapsFieldsCorrectly()
        {
            var patients = new List<Patient>
            {
                new() { PatientId = 1, FullName = "Test Patient", PhoneNumber = "9876543210", Gender = Gender.Female }
            };

            _repoMock.Setup(r => r.GetPatientReportAsync(null, null)).ReturnsAsync(patients);

            var result = await _service.GetPatientReportAsync(new PatientReportRequest());

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Gender, Is.EqualTo("Female"));
        }
    }
}