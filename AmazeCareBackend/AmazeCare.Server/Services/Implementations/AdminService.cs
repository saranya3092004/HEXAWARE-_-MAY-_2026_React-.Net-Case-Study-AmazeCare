using AmazeCare.Server.DTOs.AdminDtos;
using AmazeCare.Server.Repository.Interfaces;
using AmazeCare.Server.Services.Interfaces;

namespace AmazeCare.Server.Modules.AdminModule.Service
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ILogger<AdminService> _logger;

        public AdminService(IAdminRepository adminRepository, ILogger<AdminService> logger)
        {
            _adminRepository = adminRepository;
            _logger = logger;
        }

        public async Task<AdminDashboardResponse> GetDashboardAsync()
        {
            try
            {
                var response = new AdminDashboardResponse
                {
                    TotalPatients = await _adminRepository.GetTotalPatientsAsync(),
                    TotalDoctors = await _adminRepository.GetTotalDoctorsAsync(),
                    TodaysAppointments = await _adminRepository.GetTodaysAppointmentsCountAsync(),
                    PendingAppointments = await _adminRepository.GetPendingAppointmentsCountAsync()
                };

                _logger.LogInformation(
                    "AdminDashboard fetched: Patients={Patients}, Doctors={Doctors}, TodaysAppointments={Today}, Pending={Pending}.",
                    response.TotalPatients, response.TotalDoctors, response.TodaysAppointments, response.PendingAppointments);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while building the admin dashboard.");
                throw;
            }
        }

        public async Task<List<AppointmentReportItem>> GetAppointmentReportAsync(AppointmentReportRequest request)
        {
            try
            {
                var appointments = await _adminRepository.GetAppointmentReportAsync(
                    request.FromDate, request.ToDate, request.DoctorId, request.Status);

                _logger.LogInformation(
                    "AppointmentReport generated: FromDate={FromDate}, ToDate={ToDate}, DoctorId={DoctorId}, Status={Status}, ResultCount={Count}.",
                    request.FromDate, request.ToDate, request.DoctorId, request.Status, appointments.Count);

                return appointments.Select(a => new AppointmentReportItem
                {
                    AppointmentId = a.AppointmentId,
                    PatientName = a.Patient?.FullName ?? string.Empty,
                    DoctorName = a.Doctor?.Name ?? string.Empty,
                    AppointmentDate = a.AppointmentDate,
                    TimeSlot = a.TimeSlot,
                    Status = a.Status.ToString(),
                    CreatedAt = a.CreatedAt
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while generating the appointment report.");
                throw;
            }
        }

        public async Task<List<PatientReportItem>> GetPatientReportAsync(PatientReportRequest request)
        {
            try
            {
                var patients = await _adminRepository.GetPatientReportAsync(request.FromDate, request.ToDate);

                _logger.LogInformation(
                    "PatientReport generated: FromDate={FromDate}, ToDate={ToDate}, ResultCount={Count}.",
                    request.FromDate, request.ToDate, patients.Count);

                return patients.Select(p => new PatientReportItem
                {
                    PatientId = p.PatientId,
                    FullName = p.FullName,
                    Gender = p.Gender.ToString(),
                    PhoneNumber = p.PhoneNumber,
                    CreatedAt = p.CreatedAt
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while generating the patient report.");
                throw;
            }
        }
    }
}