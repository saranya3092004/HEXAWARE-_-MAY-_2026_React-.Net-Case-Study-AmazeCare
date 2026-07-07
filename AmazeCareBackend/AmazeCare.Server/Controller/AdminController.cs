using AmazeCare.Server.DTOs.AdminDtos;
using AmazeCare.Server.Modules.Auth.DTOs;
using AmazeCare.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AmazeCare.Server.Modules.AdminModule.Controllers
{
    [ApiController]
    [Route("api/v1/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // GET /api/v1/admin/dashboard — total patients, doctors, today's appointments, pending appointments
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _adminService.GetDashboardAsync();
            return Ok(ApiResponse<AdminDashboardResponse>.OK(result));
        }

        // GET /api/v1/admin/reports/appointments — filter by date range, doctor, status (list view)
        [HttpGet("reports/appointments")]
        public async Task<IActionResult> GetAppointmentReport([FromQuery] AppointmentReportRequest request)
        {
            var result = await _adminService.GetAppointmentReportAsync(request);
            return Ok(ApiResponse<List<AppointmentReportItem>>.OK(result));
        }

        // GET /api/v1/admin/reports/patients — new patients per period
        [HttpGet("reports/patients")]
        public async Task<IActionResult> GetPatientReport([FromQuery] PatientReportRequest request)
        {
            var result = await _adminService.GetPatientReportAsync(request);
            return Ok(ApiResponse<List<PatientReportItem>>.OK(result));
        }
    }
}