using AmazeCare.Server.Data;
using AmazeCare.Server.DTOs;
using AmazeCare.Server.Models;
using AmazeCare.Server.Modules.Auth.DTOs;
using AmazeCare.Server.Modules.Auth.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

//Admin@AmazeCare@123 admin password

namespace AmazeCare.Server.Modules.Auth.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] EmailLoginRequest request)
        {
           
                var result = await _authService.EmailLoginAsync(request);
                return Ok(ApiResponse<LoginResponse>.OK(result, "Login successful."));
           
        }


        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterPatientRequest request)
        {
            
            
                var result = await _authService.RegisterPatientAsync(request);
                return Ok(ApiResponse<LoginResponse>.Created(result, "Registration successful."));
           
        }


        [HttpPut("change-password")]
        [Authorize(Roles = "User,Doctor,Admin,LabTechnician")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(ApiResponse.Fail("Invalid token."));
            }

           
                await _authService.ChangePasswordAsync(userId, request);
                return Ok(ApiResponse.OK("Password changed successfully."));
          
        }
    }
}
