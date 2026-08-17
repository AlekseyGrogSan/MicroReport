using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace UserService.Tests.Integration
{
    /// <summary>
    /// Builds real, validly-signed JWTs using the same secret/issuer/audience that
    /// CustomWebApplicationFactory injects into the test host configuration.
    /// This lets integration tests exercise the real JwtBearer + [Authorize] pipeline
    /// instead of mocking authentication away.
    /// </summary>
    public static class TestJwtTokenGenerator
    {
        public const string SecretKey = "SUPER_SECRET_KEY_THAT_MUST_BE_AT_LEAST_32_BYTES_LONG_12345!";
        public const string Issuer = "MicroReport";
        public const string Audience = "MicroReportClients";

        public static string GenerateToken(Guid userId, TimeSpan? lifetime = null)
        {
            // Program.cs clears JwtSecurityTokenHandler.DefaultInboundClaimTypeMap,
            // so the controller reads the raw "sub" claim directly.
            var claims = new[]
            {
                new Claim("sub", userId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(lifetime ?? TimeSpan.FromMinutes(30)),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
