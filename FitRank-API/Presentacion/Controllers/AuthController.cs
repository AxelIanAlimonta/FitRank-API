using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using FitRank_API.Application.CasosDeUso.UsuarioCasosDeUso;
using FitRank_API.Application.DTOs.Invitacion;
using FitRank_API.Application.DTOs.UsuarioDTOs;
using FitRank_API.Application.DTOs.UsuarioDTOs.ValidarAuth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly LoginUsuarioCasoDeUso _loginCasoDeUso;
    private readonly RegistrarUsuarioCasoDeUso _registerCasoDeUso;

    private readonly ValidarTokenActivacionCasoDeUso _validarTokenActivacionCasoDeUso;
    private readonly ActivarCuentaCasoDeUso _activarCuentaCasoDeUso;
    private readonly AgregarUsuarioConInvitacionCasoDeUso _agregarUsuarioConInvitacionCasoDeUso;
    private readonly IConfiguration _config;

    public AuthController(
        LoginUsuarioCasoDeUso loginCasoDeUso,
        RegistrarUsuarioCasoDeUso registerCasoDeUso,
       
        ValidarTokenActivacionCasoDeUso validarTokenActivacionCasoDeUso,
        ActivarCuentaCasoDeUso activarCuentaCasoDeUso , AgregarUsuarioConInvitacionCasoDeUso agregarUsuarioConInvitacionCasoDeUso   , IConfiguration configuration )
    {
        _loginCasoDeUso = loginCasoDeUso;
        _registerCasoDeUso = registerCasoDeUso;
      
        _validarTokenActivacionCasoDeUso = validarTokenActivacionCasoDeUso;
        _activarCuentaCasoDeUso = activarCuentaCasoDeUso;
        _agregarUsuarioConInvitacionCasoDeUso = agregarUsuarioConInvitacionCasoDeUso;
        _config = configuration;
    }



    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // 🔹 Autenticar usuario desde el caso de uso
        var result = await _loginCasoDeUso.Ejecutar(dto);
        if (result == null)
            return Unauthorized(new { Mensaje = "Email o password inválido" });

        // 🔹 Recuperar el usuario autenticado
        var usuario = result.User; // suponiendo que devuelve UsuarioAuthDTO o similar

        // 🔹 Generar token JWT
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()), // ✅ agregado
    new Claim(JwtRegisteredClaimNames.Sub, usuario.Email),
    new Claim(ClaimTypes.Role, usuario.Rol),
    new Claim("UserId", usuario.Id.ToString())
};


        if (usuario.GimnasioId.HasValue)
        {
            claims.Add(new Claim(ClaimTypes.GroupSid, usuario.GimnasioId.Value.ToString()));
        }

        var token = new JwtSecurityToken(
            issuer: "FitRankAPI",
            audience: "FitRankApp",
            claims: claims,
            expires: DateTime.Now.AddHours(5),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        // 🔹 Devolver respuesta estándar con el gimnasio incluido
        return Ok(new AuthResponseDTO
        {
            Token = tokenString,
            User = new UsuarioAuthDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellidos = usuario.Apellidos,
                Email = usuario.Email,
                NombreUsuario = usuario.NombreUsuario,
                Rol = usuario.Rol,
                GimnasioId = usuario.GimnasioId, // 💥 este es el campo nuevo
                TieneCuotaPagada = usuario.TieneCuotaPagada
            }
        });
    }

    [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDTO>> Register([FromBody] RegisterDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _registerCasoDeUso.Ejecutar(dto);
            if (result == null)
                return BadRequest(new { Mensaje = "Email ya existe" });

            return Ok(result);
        }


        [HttpPost("register-invitacion")]
        public async Task<ActionResult<AuthResponseDTO>> AgregarUsuarioConInvitacion([FromBody] RegisterInvitacionDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _agregarUsuarioConInvitacionCasoDeUso.Ejecutar(dto);
            if (result == null)
                return BadRequest(new { Mensaje = "Token de invitación inválido o ya usado" });

            return Ok(result);
        }



        [HttpPost("validar-activacion")]
        public async Task<ActionResult> ValidarActivacion([FromBody] ValidarActivacionDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var esValido = await _validarTokenActivacionCasoDeUso.Ejecutar(dto.Token);
            if (!esValido)
                return BadRequest(new { valido = false, Mensaje = "Token inválido o expirado" });

            return Ok(new { valido = true });
        }

        [HttpPost("activar-cuenta")]
        public async Task<ActionResult<ActivarResponseDTO>> ActivarCuenta([FromBody] ActivarCuentaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var email = await _activarCuentaCasoDeUso.Ejecutar(dto.Token, dto.Password);
            if (email == null)
                return BadRequest(new { Mensaje = "Token inválido o ya usado" });



            return Ok(new ActivarResponseDTO { Email = email, Mensaje = "Cuenta activada. Ahora inicia sesión." });
        }
    }


