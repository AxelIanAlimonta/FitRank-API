using FitRank_API.Application.DTOs.Auth;
using FitRank_API.Application.DTOs.Auth.Invitacion;
using FitRank_API.Application.Interfaces;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace FitRank_API.Application.Servicio
{
    public class AuthServiceImpl : IAuthService
    {
        private readonly FitRankDbContext _context;
        private readonly IConfiguration _config;

        public AuthServiceImpl(FitRankDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }



        public async Task<AuthResponseDTO?> RegisterAsync(RegisterDTO dto)
        {
            // Chequea si email existe
            if (await _context.Usuarios.AnyAsync(u => u.Correo == dto.Correo))
                return null;  // BadRequest "Email existe"

            var user = new Usuario
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellidos,
                Dni = dto.Dni,
                FechaNacimiento = dto.FechaNacimiento,
                telefono = dto.Telefono,
                Correo = dto.Correo,
                UserName = dto.Username,
                Email = dto.Correo,  // Asume igual
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Rol = dto.Rol ?? "User",
                Estado = "Activo",
                alturaCm = dto.AlturaCm,
                pesoKg = dto.PesoKg,
                nivel = dto.Nivel
            };

            _context.Usuarios.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            var userDto = MapToUsuarioDto(user);

            return new AuthResponseDTO { Token = token, User = userDto };
        }

        public async Task<AuthResponseDTO?> RegisterWithInvitacionAsync(RegisterInvitacionDTO dto)
        {
            // Valida token invitación (lógica en AdminService o aquí)
            // Por ahora, asume válido – integra validación JWT después
            var invitacion = await _context.Invitaciones
                .Include(i => i.UsuarioId)
                .FirstOrDefaultAsync(i => i.Id == ParseInvitacionIdFromToken(dto.TokenInvitacion) && i.Estado == "Pagado");

            if (invitacion == null || invitacion.UsuarioId != null)  // Ya usada
                return null;  // BadRequest

            // Parsea datos pre-llenados del token/invitación
            var datosPre = JsonSerializer.Deserialize<Dictionary<string, object>>(invitacion.DatosPrellenados) ?? new Dictionary<string, object>();

            var user = new Usuario
            {
                Nombre = datosPre.GetValueOrDefault("nombre", "").ToString() ?? dto.Username,  // Pre o nuevo
                Apellido = datosPre.GetValueOrDefault("apellidos", "").ToString() ?? "",
                Dni = int.Parse(datosPre.GetValueOrDefault("dni", "0").ToString() ?? "0"),
                telefono = datosPre.GetValueOrDefault("telefono", "").ToString() ?? "",
                Correo = invitacion.Email,
                Email = invitacion.Email,
                UserName = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Rol = "User",
                Estado = "Activo",
                FechaNacimiento = dto.FechaNacimiento,
                alturaCm = dto.AlturaCm,
                pesoKg = dto.PesoKg,
                nivel = dto.Nivel,
                CuotaPagadaHasta = invitacion.CuotaPagadaHasta  // De invitación pagada
            };

            _context.Usuarios.Add(user);
            await _context.SaveChangesAsync();

            // Link a invitación
            invitacion.UsuarioId = (int?)user.Id;
            invitacion.Estado = "Usada";
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            var userDto = MapToUsuarioDto(user);

            return new AuthResponseDTO { Token = token, User = userDto };
        }

        public string GenerateJwtToken(Usuario user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "tu_secreto_super_seguro_32_chars_minimo"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Correo),
                new Claim(ClaimTypes.Role, user.Rol ?? "User"),
                new Claim("Username", user.UserName)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"] ?? "FitRankAPI",
                audience: _config["Jwt:Audience"] ?? "FitRankApp",
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),  // 24h
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public UsuarioAuthDTO MapToUsuarioDto(Usuario user)
        {
            return new UsuarioAuthDTO
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Apellidos = user.Apellido,
                Correo = user.Correo,
                Username = user.UserName,
                Rol = user.Rol ?? "User",
                CuotaPagadaHasta = user.CuotaPagadaHasta,
                TieneCuotaPagada = user.CuotaPagadaHasta.HasValue && user.CuotaPagadaHasta > DateTime.UtcNow,
                QrToken = user.QrToken  // Solo si pagado
            };
        }

        // Helper para parse token invitación (integra JWT decode después)
        private int ParseInvitacionIdFromToken(string token)
        {
            // TODO: Valida y parsea JWT para extraer invitacionId
            // Por ahora, dummy – usa JwtSecurityTokenHandler para real
            return 1;  // Ejemplo; implementa después
        }



        public async Task<bool> ValidarTokenActivacionAsync(string token)
        {
            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.TokenRecuperacion == token && u.TokenExpira > DateTime.UtcNow && !u.EsActivado);

            return user != null;
        }

        public async Task<string?> ActivarCuentaAsync(string token, string nuevaPassword)
        {
            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.TokenRecuperacion == token && u.TokenExpira > DateTime.UtcNow && !u.EsActivado);

            if (user == null)
                return null;  // Inválido

            // Cambia password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(nuevaPassword);
            user.EsActivado = true;
            user.TokenRecuperacion = null;  // Limpia token
            user.TokenExpira = null;
            user.UserName = user.UserName ?? user.Correo.Split('@')[0];  // Username final si temporal

            await _context.SaveChangesAsync();

            return user.Correo;  // Retorna email para login auto en frontend
        }

        // Modifica LoginAsync para chequear activación (opcional: retorna mensaje si no activado)
        public async Task<AuthResponseDTO?> LoginAsync(LoginDTO dto)
        {
            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == dto.Email || u.Email == dto.Email);

            if (user == null || string.IsNullOrEmpty(user.PasswordHash) || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return null;

            if (!user.EsActivado)
                return null;  // O retorna { Mensaje = "Activa tu cuenta primero" } si quieres response especial

            var token = GenerateJwtToken(user);
            var userDto = MapToUsuarioDto(user);

            return new AuthResponseDTO { Token = token, User = userDto };
        }
    }
}
