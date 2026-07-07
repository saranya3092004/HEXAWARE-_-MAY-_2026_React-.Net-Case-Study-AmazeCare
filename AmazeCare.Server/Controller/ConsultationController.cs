using AmazeCare.Server.DTOs.ConsultationDtos;
using AmazeCare.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AmazeCare.Server.Modules.Auth.DTOs;

namespace AmazeCare.Server.Controller
{
    [Route("api/consultations")]
    [ApiController]
    public class ConsultationController : ControllerBase
    {
        private readonly IConsultationService _consultationService;

        public ConsultationController(IConsultationService consultationService)
        {
            _consultationService = consultationService;
        }

        // GET /api/v1/consultations — Doctor sees own, Patient sees own, Admin sees all
        [HttpGet]
        [Authorize(Roles = "User,Doctor,Admin")]
        public async Task<IActionResult> GetConsultations()
        {
            var (callerId, isAdmin, isDoctor) = GetCallerContext();
            var result = await _consultationService.GetConsultationsAsync(callerId, isAdmin, isDoctor);
            return Ok(ApiResponse<List<ConsultationResponse>>.OK(result));
        }

        // GET /api/v1/consultations/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "User,Doctor,Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var (callerId, isAdmin, isDoctor) = GetCallerContext();
            var result = await _consultationService.GetByIdAsync(id, callerId, isAdmin, isDoctor);
            return Ok(ApiResponse<ConsultationResponse>.OK(result));
        }

        // POST /api/v1/consultations — Doctor creates consultation for a confirmed appointment
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Create([FromBody] CreateConsultationRequest request)
        {
            var (callerId, _, _) = GetCallerContext();
            var result = await _consultationService.CreateAsync(callerId, request);
            return Ok(ApiResponse<ConsultationResponse>.Created(result, "Consultation recorded successfully."));
        }

        // PUT /api/v1/consultations/{id} — Doctor updates symptoms, examination, diagnosis, treatment plan
        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateConsultationRequest request)
        {
            var (callerId, _, _) = GetCallerContext();
            var result = await _consultationService.UpdateAsync(id, callerId, request);
            return Ok(ApiResponse<ConsultationResponse>.OK(result, "Consultation updated successfully."));
        }

        // Reads role-specific profile id (PatientId/DoctorId/AdminId) from the JWT claims,
        // matching the pattern used in AppointmentController and DoctorController.
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
                int.TryParse(User.FindFirstValue("PatientId"), out callerId);

            return (callerId, isAdmin, isDoctor);
        }
    }
}
