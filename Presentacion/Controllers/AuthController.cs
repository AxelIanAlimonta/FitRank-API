using Microsoft.AspNetCore.Mvc;
using FitRank_API.Application.DTOs.Auth;
using FitRank_API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using FitRank_API.Application.DTOs.Auth.invitacion;
using FitRank_API.Application.DTOs.Auth.ValidarAuth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FitRank_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;

        public AuthController(IAuthService authService, IConfiguration config)
        {
            _authService = authService;
            _config = config;
        }


        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // HARDCORE ADMIN EN CONTROLADOR
            if (dto.Email == "fitrank2025@gmail.com" && dto.Password == "Admin1234!")
            {
                // Generamos JWT con rol Admin
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                // ✅ Aquí se crean los claims incluyendo el rol Admin
                var claims = new[]
                {
      new Claim(JwtRegisteredClaimNames.Sub, dto.Email),
      new Claim(ClaimTypes.Role, "Admin") // <- Rol Admin
  };

                var token = new JwtSecurityToken(
                    issuer: "FitRankAPI",
                    audience: "FitRankApp",
                    claims: claims,
                    expires: DateTime.Now.AddHours(5),
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                var adminUser = new UsuarioAuthDto
                {
                    Id = 1,
                    Nombre = "Admin",
                    Apellidos = "Dev",
                    Correo = "fitrank2025@gmail.com",
                    Username = "admin",
                    Rol = "Admin",
                    TieneCuotaPagada = true
                };

                return Ok(new AuthResponseDto
                {
                    Token = tokenString,
                    User = adminUser
                });
            }

            // Login normal con DB
            var result = await _authService.LoginAsync(dto);
            if (result == null)
                return Unauthorized(new { Mensaje = "Email o password inválido" });

            return Ok(result);
        }


        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(dto);
            if (result == null)
                return BadRequest(new { Mensaje = "Email ya existe" });

            return Ok(result);
        }

      
        [HttpPost("register-invitacion")]
        public async Task<ActionResult<AuthResponseDto>> RegisterWithInvitacion([FromBody] RegisterInvitacionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterWithInvitacionAsync(dto);
            if (result == null)
                return BadRequest(new { Mensaje = "Token de invitación inválido o ya usado" });

            return Ok(result);
        }



        [HttpPost("validar-activacion")]
        public async Task<ActionResult> ValidarActivacion([FromBody] ValidarActivacionDto dto)  
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var esValido = await _authService.ValidarTokenActivacionAsync(dto.Token);
            if (!esValido)
                return BadRequest(new { valido = false, Mensaje = "Token inválido o expirado" });

            return Ok(new { valido = true });
        }

        [HttpPost("activar-cuenta")]
        public async Task<ActionResult<ActivarResponseDto>> ActivarCuenta([FromBody] ActivarCuentaDto dto)  
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var email = await _authService.ActivarCuentaAsync(dto.Token, dto.Password);
            if (email == null)
                return BadRequest(new { Mensaje = "Token inválido o ya usado" });

        

            return Ok(new ActivarResponseDto { Email = email, Mensaje = "Cuenta activada. Ahora inicia sesión." });
        }
    }
}