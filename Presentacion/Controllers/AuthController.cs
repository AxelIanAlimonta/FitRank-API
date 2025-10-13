using Microsoft.AspNetCore.Mvc;
using FitRank_API.Application.DTOs.Auth;
using FitRank_API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using FitRank_API.Application.DTOs.Auth.invitacion;
using FitRank_API.Application.DTOs.Auth.ValidarAuth;

namespace FitRank_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

       
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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