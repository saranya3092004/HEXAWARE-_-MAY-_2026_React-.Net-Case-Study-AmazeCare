using AmazeCare.Server.DTOs;
using AmazeCare.Server.Modules.Auth.DTOs;

namespace AmazeCare.Server.Modules.Auth.Services.Interface
{
    public interface IAuthService
    {
        //Task<SendOtpResponse> SendOtpAsync(string phoneNumber);       
        //Task<LoginResponse> VerifyOtpAsync(VerifyOtpRequest request);
        Task<LoginResponse> EmailLoginAsync(EmailLoginRequest request);
        //Task<LoginResponse> StaffLoginAsync(StaffLoginRequest request); 
        Task<LoginResponse> RegisterPatientAsync(RegisterPatientRequest request);
        //Task<LoginResponse> LinkPatientAsync(LinkPatientRequest request);
        Task ChangePasswordAsync(int userId, ChangePasswordRequest request);
    }
}
