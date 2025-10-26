using FitRank_API.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FitRank_API.Application.CasosDeUso.UsuarioCasosDeUso
{
    public class GenerarTokenCasoDeUso
    {
        private readonly IConfiguration _config;

        public GenerarTokenCasoDeUso(IConfiguration config)
        {
            _config = config;
        }

        public string Ejecutar(FitRank_API.Domain.Entities.Usuario user )
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _config["Jwt:Key"] ?? "tu_secreto_super_seguro_32_chars_minimo"
            ));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Rol ?? "User"),
                
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"] ?? "FitRankAPI",
                audience: _config["Jwt:Audience"] ?? "FitRankApp",
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}


    

