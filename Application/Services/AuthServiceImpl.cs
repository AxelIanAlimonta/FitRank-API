using FitRank_API.Application.DTOs.Auth;
using FitRank_API.Application.Interfaces;
using BCrypt.Net;  
using FitRank_API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FitRank_API.Infrastructure.Persistence;
using System.Text.Json;
using System.Collections.Generic;
using FitRank_API.Application.DTOs.Auth.invitacion;
using FitRank_API.Application.DTOs.Usuario;
using System;

namespace FitRank_API.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly FitRankDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(FitRankDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

 

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
        {
            // Chequea si email existe
            if (await _context.Usuarios.AnyAsync(u => u.correo == dto.Correo))
                return null;  // BadRequest "Email existe"

            var user = new Usuario
            {
                nombre = dto.Nombre,
                apellidos = dto.Apellidos,
                dni = dto.Dni,
                fechaNacimiento = dto.FechaNacimiento,
                telefono = dto.Telefono,
                correo = dto.Correo,
                username = dto.Username,
                email = dto.Correo,  // Asume igual
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Rol = dto.Rol ?? "User",
                estado = "Activo",
                alturaCm = dto.AlturaCm,
                pesoKg = dto.PesoKg,
                nivel = dto.Nivel
            };

            _context.Usuarios.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            var userDto = MapToUsuarioDto(user);

            return new AuthResponseDto { Token = token, User = userDto };
        }

        public async Task<AuthResponseDto?> RegisterWithInvitacionAsync(RegisterInvitacionDto dto)
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
                nombre = datosPre.GetValueOrDefault("nombre", "").ToString() ?? dto.Username,  // Pre o nuevo
                apellidos = datosPre.GetValueOrDefault("apellidos", "").ToString() ?? "",
                dni = int.Parse(datosPre.GetValueOrDefault("dni", "0").ToString() ?? "0"),
                telefono = datosPre.GetValueOrDefault("telefono", "").ToString() ?? "",
                correo = invitacion.Email,
                email = invitacion.Email,
                username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Rol = "User",
                estado = "Activo",
                fechaNacimiento = dto.FechaNacimiento,
                alturaCm = dto.AlturaCm,
                pesoKg = dto.PesoKg,
                nivel = dto.Nivel,
                CuotaPagadaHasta = invitacion.CuotaPagadaHasta  // De invitación pagada
            };

            _context.Usuarios.Add(user);
            await _context.SaveChangesAsync();

            // Link a invitación
            invitacion.UsuarioId = user.id;
            invitacion.Estado = "Usada";
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            var userDto = MapToUsuarioDto(user);

            return new AuthResponseDto { Token = token, User = userDto };
        }

        public string GenerateJwtToken(Usuario user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "tu_secreto_super_seguro_32_chars_minimo"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim(ClaimTypes.Email, user.correo),
                new Claim(ClaimTypes.Role, user.Rol ?? "User"),
                new Claim("Username", user.username)
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

        public UsuarioAuthDto MapToUsuarioDto(Usuario user)
        {
            return new UsuarioAuthDto
            {
                Id = user.id,
                Nombre = user.nombre,
                Apellidos = user.apellidos,
                Correo = user.correo,
                Username = user.username,
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
            user.username = user.username ?? user.correo.Split('@')[0];  // Username final si temporal

            await _context.SaveChangesAsync();

            return user.correo;  // Retorna email para login auto en frontend
        }

        // Modifica LoginAsync para chequear activación (opcional: retorna mensaje si no activado)
        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.correo == dto.Email || u.email == dto.Email);

            if (user == null || string.IsNullOrEmpty(user.PasswordHash) || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return null;

            if (!user.EsActivado)
                return null;  // O retorna { Mensaje = "Activa tu cuenta primero" } si quieres response especial

            var token = GenerateJwtToken(user);
            var userDto = MapToUsuarioDto(user);

            return new AuthResponseDto { Token = token, User = userDto };
        }
    }
}