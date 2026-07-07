using AmazeCare.Server.DTOs;
using AmazeCare.Server.Models;
using AmazeCare.Server.Modules.Auth.DTOs;

namespace AmazeCare.Server.Modules.Auth.Services.Interface
{
    public interface IJwtService
    {
        LoginResponse BuildToken(User user, int? roleSpecificId);
    }
}
