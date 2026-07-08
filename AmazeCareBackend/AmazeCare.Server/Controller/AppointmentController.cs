using AmazeCare.Server.DTOs.Appointment_Dtos;
using AmazeCare.Server.Modules.Auth.DTOs;
using AmazeCare.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AmazeCare.Server.Controller
{
    [Route("api/appointments")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        // GET /api/appointments — filtered by role, status, date range
        [HttpGet]
        [Authorize(Roles = "User,Doctor,Admin")]
        public async Task<IActionResult> GetAppointments([FromQuery] AppointmentFilterRequest filter)
        {
            var (callerId, isAdmin, isDoctor) = GetCallerContext();
            var result = await _appointmentService.GetAppointmentsAsync(callerId, isAdmin, isDoctor, filter);
            return Ok(ApiResponse<List<AppointmentResponse>>.OK(result));
        }

        // GET /api/appointments/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "User,Doctor,Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var (callerId, isAdmin, isDoctor) = GetCallerContext();
            var result = await _appointmentService.GetByIdAsync(id, callerId, isAdmin, isDoctor);
            return Ok(ApiResponse<AppointmentResponse>.OK(result));
        }

        // POST /api/appointments — books for self (User) or on behalf of a patient (Admin)
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Book([FromBody] CreateAppointmentRequest request)
        {
            var (callerId, isAdmin, _) = GetCallerContext();
            var result = await _appointmentService.BookAppointmentAsync(callerId, isAdmin, request);
            return Ok(ApiResponse<AppointmentResponse>.Created(result, "Appointment booked successfully."));
        }

        // PUT /api/appointments/{id}/confirm
        [HttpPut("{id}/confirm")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> Confirm(int id)
        {
            var (callerId, isAdmin, _) = GetCallerContext();
            var result = await _appointmentService.ConfirmAsync(id, callerId, isAdmin);
            return Ok(ApiResponse<AppointmentResponse>.OK(result, "Appointment confirmed successfully."));
        }

        // PUT /api/appointments/{id}/reject
        [HttpPut("{id}/reject")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> Reject(int id, [FromBody] RejectAppointmentRequest request)
        {
            var (callerId, isAdmin, _) = GetCallerContext();
            var result = await _appointmentService.RejectAsync(id, callerId, isAdmin, request);
            return Ok(ApiResponse<AppointmentResponse>.OK(result, "Appointment rejected successfully."));
        }

        // PUT /api/appointments/{id}/cancel
        [HttpPut("{id}/cancel")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Cancel(int id, [FromBody] CancelAppointmentRequest request)
        {
            var (callerId, isAdmin, _) = GetCallerContext();
            var result = await _appointmentService.CancelAsync(id, callerId, isAdmin, request);
            return Ok(ApiResponse<AppointmentResponse>.OK(result, "Appointment cancelled successfully."));
        }

        // PUT /api/appointments/{id}/reschedule — Patient, Doctor, or Admin
        [HttpPut("{id}/reschedule")]
        [Authorize(Roles = "User,Doctor,Admin")]
        public async Task<IActionResult> Reschedule(int id, [FromBody] RescheduleAppointmentRequest request)
        {
            var (callerId, isAdmin, isDoctor) = GetCallerContext();
            var result = await _appointmentService.RescheduleAsync(id, callerId, isAdmin, isDoctor, request);
            return Ok(ApiResponse<AppointmentResponse>.OK(result, "Appointment rescheduled successfully."));
        }

        // PUT /api/appointments/{id}/complete
        [HttpPut("{id}/complete")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Complete(int id)
        {
            var (callerId, _, _) = GetCallerContext();
            var result = await _appointmentService.CompleteAsync(id, callerId);
            return Ok(ApiResponse<AppointmentResponse>.OK(result, "Appointment marked as completed."));
        }

        // GET /api/appointments/slots?doctorId=1&date=2026-07-10&excludeAppointmentId=5
        [HttpGet("slots")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableSlots(
            [FromQuery] int doctorId,
            [FromQuery] DateTime date,
            [FromQuery] int? excludeAppointmentId = null)
        {
            var slots = await _appointmentService.GetAvailableSlotsAsync(doctorId, date, excludeAppointmentId);
            return Ok(ApiResponse<List<string>>.OK(slots));
        }

        // Reads role-specific profile id (PatientId/DoctorId/AdminId) from the JWT claims
        // set by JwtService.BuildToken(), matching the pattern used in DoctorController.
        private (int CallerId, bool IsAdmin, bool IsDoctor) GetCallerContext()
        {
            var isAdmin = User.IsInRole("Admin");
            var isDoctor = User.IsInRole("Doctor");

            int callerId = 0;
            if (isDoctor)
                int.TryParse(User.FindFirstValue("DoctorId"), out callerId);
            else if (isAdmin)
                int.TryParse(User.FindFirstValue("AdminId"), out callerId);
            else
                int.TryParse(User.FindFirstValue("UserId"), out callerId);   // it is "PatientId"

            return (callerId, isAdmin, isDoctor);
        }
    }
}
