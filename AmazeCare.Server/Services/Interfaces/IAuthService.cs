using AmazeCare.Server.DTOs;
using AmazeCare.Server.Modules.Auth.DTOs;

namespace AmazeCare.Server.Modules.Auth.Services.Interface
{
    public interface IAuthService
    {
        Task<LoginResponse> EmailLoginAsync(EmailLoginRequest request);
        Task<LoginResponse> RegisterPatientAsync(RegisterPatientRequest request);
        Task ChangePasswordAsync(int userId, ChangePasswordRequest request);
    }
}
