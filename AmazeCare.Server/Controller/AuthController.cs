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

        //[HttpPost("send-otp")]
        //[AllowAnonymous]
        //public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
        //{
            
            
        //        await _authService.SendOtpAsync(request.PhoneNumber);
        //        return Ok(ApiResponse.OK("OTP sent successfully."));
          
        //}

        //// POST /api/v1/auth/verify-otp
        //[HttpPost("verify-otp")]
        //[AllowAnonymous]
        //public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        //{
            
        //        var result = await _authService.VerifyOtpAsync(request);
        //        return Ok(ApiResponse<LoginResponse>.OK(result, "OTP verified successfully."));
          
        //}

        // POST /api/v1/auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] EmailLoginRequest request)
        {
           
                var result = await _authService.EmailLoginAsync(request);
                return Ok(ApiResponse<LoginResponse>.OK(result, "Login successful."));
           
        }

        // POST /api/v1/auth/staff-login
        //[HttpPost("staff-login")]
        //[AllowAnonymous]
        //public async Task<IActionResult> StaffLogin([FromBody] StaffLoginRequest request)
        //{
            
        //        var result = await _authService.StaffLoginAsync(request);
        //        return Ok(ApiResponse<LoginResponse>.OK(result, "Login successful."));
           
        //}

        // POST /api/v1/auth/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterPatientRequest request)
        {
            
            
                var result = await _authService.RegisterPatientAsync(request);
                return Ok(ApiResponse<LoginResponse>.Created(result, "Registration successful."));
           
        }

        // POST /api/v1/auth/link-patient  the patient created by Admin registering themself as user in online 
        //[HttpPost("link-patient")]
        //[AllowAnonymous]
        //public async Task<IActionResult> LinkPatient([FromBody] LinkPatientRequest request)
        //{
           
        //        var result = await _authService.LinkPatientAsync(request);
        //        return Ok(ApiResponse<LoginResponse>.OK(result, "Patient account linked successfully."));

        //}

        // PUT /api/v1/auth/change-password
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
