using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SnakeHub.Services
{
    public class JwtService(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;
        public string GenerateJwtToken(ClaimsPrincipal claims)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? string.Empty);
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(claims.Claims.GroupBy(claim => new { claim.Type, claim.Value }).Select(group => group.Last())),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public ClaimsPrincipal GetClaimsFromJwtToken(string token)
        {
            TokenValidationParameters validationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? string.Empty))
            };
            return new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out _);
        }
    }
}
