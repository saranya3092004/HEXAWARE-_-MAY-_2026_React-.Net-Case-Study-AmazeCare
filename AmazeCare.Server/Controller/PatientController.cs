using AmazeCare.Server.Modules.Auth.DTOs;
using AmazeCare.Server.Modules.PatientModule.DTOs;
using AmazeCare.Server.Modules.PatientModule.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AmazeCare.Server.Modules.PatientModule.Controllers
{
    [ApiController]
    [Route("api/patients")]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        // GET /api/v1/patients — User sees own profile, Admin sees all
        [HttpGet]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetPatients([FromQuery] string? searchTerm = null, [FromQuery] string? sortBy = null, [FromQuery] bool sortDescending = false)
        {
            var (userId, isAdmin) = GetCallerContext();
            var result = await _patientService.GetPatientsAsync(userId, isAdmin, searchTerm, sortBy, sortDescending);
            return Ok(ApiResponse<List<PatientResponse>>.OK(result));
        }

        // GET /api/v1/patients/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "User,Doctor,Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var (userId, isAdmin) = GetCallerContext();
            var result = await _patientService.GetByIdAsync(id, userId, isAdmin);
            return Ok(ApiResponse<PatientResponse>.OK(result));
        }

        // GET /api/v1/patients/{id}/history
        [HttpGet("{id}/history")]
        [Authorize(Roles = "User,Doctor,Admin")]
        public async Task<IActionResult> GetHistory(int id)
        {
            var (userId, isAdmin) = GetCallerContext();
            var isDoctor = User.IsInRole("Doctor");
            var result = await _patientService.GetHistoryAsync(id, userId, isAdmin, isDoctor);
            return Ok(ApiResponse<PatientHistoryResponse>.OK(result));
        }

        // POST /api/v1/patients — Admin registers walk-in patient
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterWalkInPatient([FromBody] RegisterWalkInPatientRequest request)
        {
            var result = await _patientService.RegisterWalkInPatientAsync(request);
            return Ok(ApiResponse<PatientResponse>.Created(result, "Patient registered successfully."));
        }



        // PUT /api/v1/patients/{id} — User (own family) or Admin (any)
        [HttpPut("{id}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePatientRequest request)
        {
            var (userId, isAdmin) = GetCallerContext();
            var result = await _patientService.UpdateAsync(id, userId, isAdmin, request);
            return Ok(ApiResponse<PatientResponse>.OK(result, "Patient updated successfully."));
        }

        // DELETE /api/v1/patients/{id} — Admin only, soft delete
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Deactivate(int id)
        {
            await _patientService.DeactivateAsync(id);
            return Ok(ApiResponse.OK("Patient deactivated successfully."));
        }

        // Reads UserId + Admin status from the JWT claims set by JwtService.BuildToken().
        private (int UserId, bool IsAdmin) GetCallerContext()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = int.TryParse(userIdClaim, out var id) ? id : 0;
            var isAdmin = User.IsInRole("Admin");
            return (userId, isAdmin);
        }
    }
}