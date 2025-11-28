using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FitRank_API.Application.CasosDeUso.UsuarioCasosDeUso
{
    public class GenerarTokenCasoDeUso
    {
        private readonly IConfiguration _config;
        private readonly IGimnasioRepositorio _gimnasioRepositorio;

        public GenerarTokenCasoDeUso(IConfiguration config, IGimnasioRepositorio gimnasioRepositorio)
        {
            _config = config;
            _gimnasioRepositorio = gimnasioRepositorio;
        }

        public virtual string Ejecutar(Usuario user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Obtener el gimnasio real del usuario
            long? gimnasioId = _gimnasioRepositorio.ObtenerGimnasioIdPorUsuario(user.Id);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Rol),
            };

            if (gimnasioId.HasValue)
                claims.Add(new Claim(ClaimTypes.GroupSid, gimnasioId.Value.ToString()));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(6),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
