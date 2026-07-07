using AmazeCare.Server.DTOs;
using AmazeCare.Server.Models;
using AmazeCare.Server.Modules.Auth.Services.Interface;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AmazeCare.Server.Modules.Auth.Services.Implementation
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public LoginResponse BuildToken(User user, int? roleSpecificId)
        {
            var jwtSection = _config.GetSection("Jwt");

            var secret = jwtSection.GetValue<string>("Key")
                ?? throw new InvalidOperationException("JWT Secret is not configured in your settings.");

            var issuer = jwtSection.GetValue<string>("Issuer");
            var audience = jwtSection.GetValue<string>("Audience");
            var expiryMinutes = jwtSection.GetValue<int?>("ExpiryMinutes") ?? 60;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Name, user.FullName),
                new(ClaimTypes.Role, user.Role.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, user.Email));
            }

            if (roleSpecificId.HasValue)
            {
                claims.Add(new Claim($"{user.Role}Id", roleSpecificId.Value.ToString()));
            }

           

            var expirationTime = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expirationTime,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponse
            {
                Token = tokenString,
                Expiry = expirationTime,
                Role = user.Role.ToString(),
                UserId = user.UserId,
                FullName = user.FullName,
                RoleSpecificId = roleSpecificId,
            };
        }
    }
}