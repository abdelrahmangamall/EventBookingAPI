using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EventBookingAPI.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

namespace EventBookingAPI.Helper
{
    public class JwtHelper
    {
        private readonly JwtSettings _jwtSettings;

        public JwtHelper(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateToken(ApplicationUser user)
        {
            if (_jwtSettings == null)
                throw new ArgumentNullException(nameof(_jwtSettings));

            if (string.IsNullOrWhiteSpace(_jwtSettings.Key))
                throw new ArgumentException("JWT Key is not configured");

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}