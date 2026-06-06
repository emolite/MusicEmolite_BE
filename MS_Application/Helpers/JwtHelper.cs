using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MS_Application.Helpers
{
    public class JwtHelper
    {
        private readonly string _secretKey;

        public JwtHelper(IConfiguration configuration)
        {
            _secretKey = configuration["JwtSettings:Secret"];
            if (string.IsNullOrEmpty(_secretKey) || Encoding.UTF8.GetBytes(_secretKey).Length < 32)
                throw new Exception("JWT Secret key must be at least 32 bytes long.");
        }

        public string GenerateToken(long userId, string refCode, string username, string roleCode, string email, short userType, int expireMinutes = 60)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim("UserId", userId.ToString()),
            new Claim("RefCode", refCode),
            new Claim("RoleCode", roleCode),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Email, email),
            new Claim("user_type", userType.ToString())
        }),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}