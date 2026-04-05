using Microsoft.IdentityModel.Tokens;
using QuotationManagement.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QuotationManagement.API.Services
{
    public class AuthService
    {
        private const string DevFallbackJwtKey = "THIS_IS_SUPER_SECRET_KEY_1234567890";
        private readonly IConfiguration _config;

        public AuthService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateJwtToken(User user)
        {
            var keyText = _config["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(keyText))
            {
                keyText = DevFallbackJwtKey;
            }

            var issuer = _config["Jwt:Issuer"] ?? "QuotationManagement.API";
            var audience = _config["Jwt:Audience"] ?? "QuotationManagement.Client";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyText));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role ?? string.Empty)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
