using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using AmazeCare.Server.Models;
using AmazeCare.Server.Modules.Auth.Services.Implementation;

namespace AmazeCare.Server.Tests.Services
{
    [TestFixture]
    public class JwtServiceTests
    {
        private JwtService _service = null!;

        [SetUp]
        public void Setup()
        {
            var configValues = new Dictionary<string, string?>
            {
                { "Jwt:Key", "this-is-a-super-secret-test-key-with-enough-length-1234" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" },
                { "Jwt:ExpiryMinutes", "60" }
            };

            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues)
                .Build();

            _service = new JwtService(config);
        }

        [Test]
        public void BuildToken_WithRoleSpecificId_IncludesRoleIdClaimAndCorrectFieldName()
        {
            var user = new User { UserId = 1, FullName = "Dr Test", Role = UserRole.Doctor, Email = "doc@x.com" };

            var result = _service.BuildToken(user, roleSpecificId: 42);

            Assert.That(result.Token, Is.Not.Null.And.Not.Empty);
            Assert.That(result.RoleSpecificId, Is.EqualTo(42));
            Assert.That(result.Role, Is.EqualTo("Doctor"));

            // Decode and verify the claim name actually matches "{Role}Id" convention
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(result.Token);
            var doctorIdClaim = jwt.Claims.FirstOrDefault(c => c.Type == "DoctorId");

            Assert.That(doctorIdClaim, Is.Not.Null);
            Assert.That(doctorIdClaim!.Value, Is.EqualTo("42"));
        }

        [Test]
        public void BuildToken_PatientRole_ClaimIsNamedUserId()
        {
            // Regression test for the PatientId/UserId bug fixed earlier in this project:
            // Role.User produces a claim literally named "UserId", not "PatientId".
            var user = new User { UserId = 1, FullName = "Patient Test", Role = UserRole.User };

            var result = _service.BuildToken(user, roleSpecificId: 7);

            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(result.Token);

            Assert.That(jwt.Claims.Any(c => c.Type == "UserId" && c.Value == "7"), Is.True);
            Assert.That(jwt.Claims.Any(c => c.Type == "PatientId"), Is.False);
        }

        [Test]
        public void BuildToken_NoRoleSpecificId_OmitsRoleIdClaim()
        {
            var user = new User { UserId = 1, FullName = "No Role Id", Role = UserRole.Admin };

            var result = _service.BuildToken(user, roleSpecificId: null);

            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(result.Token);

            Assert.That(jwt.Claims.Any(c => c.Type == "AdminId"), Is.False);
            Assert.That(result.RoleSpecificId, Is.Null);
        }
    }
}