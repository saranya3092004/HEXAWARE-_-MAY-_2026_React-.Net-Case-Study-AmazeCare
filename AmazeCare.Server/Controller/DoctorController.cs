using AmazeCare.Server.Modules.Auth.DTOs;
using AmazeCare.Server.Modules.DoctorModule.DTOs;
using AmazeCare.Server.Modules.DoctorModule.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AmazeCare.Server.Controller
{
    [ApiController]
    [Route("api/v1/doctors")]
    [Authorize]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        // ================= PUBLIC =================

        // GET /api/v1/doctors — search by name, specialization, available day
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Search([FromQuery] DoctorSearchRequest request)
        {
            var result = await _doctorService.SearchAsync(request.Name, request.SpecializationId);
            return Ok(ApiResponse<List<DoctorResponse>>.OK(result));
        }

        // GET /api/v1/doctors/{id} — profile with specializations + availability
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProfile(int id)
        {
            var result = await _doctorService.GetProfileAsync(id);
            return Ok(ApiResponse<DoctorResponse>.OK(result));
        }

        // ================= DOCTOR / ADMIN =================

        // GET /api/v1/doctors/{id}/appointments — doctor's own appointments (upcoming + completed)
        [HttpGet("{id}/appointments")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> GetDoctorAppointments(int id, [FromQuery] bool upcomingOnly = false)
        {
            var ownershipCheck = EnsureDoctorOwnership(id);
            if (ownershipCheck != null) return ownershipCheck;

            var result = await _doctorService.GetDoctorAppointmentsAsync(id, upcomingOnly);
            return Ok(ApiResponse<List<AppointmentSummary>>.OK(result));
        }

        // ================= ADMIN — DOCTOR CRUD =================

        // POST /api/v1/doctors — Admin creates doctor (User account + Doctor profile)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorRequest request)
        {
            var result = await _doctorService.CreateDoctorAsync(request);
            return Ok(ApiResponse<DoctorResponse>.Created(result, "Doctor created successfully."));
        }

        // PUT /api/v1/doctors/{id} — Admin updates doctor profile
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] UpdateDoctorRequest request)
        {
            var result = await _doctorService.UpdateDoctorAsync(id, request);
            return Ok(ApiResponse<DoctorResponse>.OK(result, "Doctor updated successfully."));
        }

        // DELETE /api/v1/doctors/{id} — Admin soft-deactivates doctor
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateDoctor(int id)
        {
            await _doctorService.DeactivateDoctorAsync(id);
            return Ok(ApiResponse.OK("Doctor deactivated successfully."));
        }

                   // ================= HELPERS =================
        private (int CallerId, bool IsAdmin, bool IsDoctor) GetCallerContext()
        {
            var isAdmin = User.IsInRole("Admin");
            var isDoctor = User.IsInRole("Doctor");

            int callerId = 0;
            if (isDoctor)
                int.TryParse(User.FindFirstValue("DoctorId"), out callerId);
            else if (isAdmin)
                int.TryParse(User.FindFirstValue("AdminId"), out callerId);

            return (callerId, isAdmin, isDoctor);
        }

        private IActionResult? EnsureDoctorOwnership(int routeDoctorId)
        {
            var (callerId, isAdmin, isDoctor) = GetCallerContext();

            if (isDoctor && !isAdmin && callerId != routeDoctorId)
                return Forbid();

            return null;
        }
    }
}