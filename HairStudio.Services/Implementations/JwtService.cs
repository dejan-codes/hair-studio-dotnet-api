using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HairStudio.Model.Models;
using Microsoft.IdentityModel.Tokens;

namespace HairStudio.Services.Implementations
{
    public class JwtService
    {
        private readonly string _secret;

        public JwtService(string secret)
        {
            _secret = secret;
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler(); 
            
            if (JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Any())
            {
                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                Issuer = "http://localhost:3000",
                Audience = "http://localhost:3000",
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
