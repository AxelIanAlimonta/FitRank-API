using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using FitRank_API.Application.CasosDeUso.UsuarioCasosDeUso;
using FitRank_API.Application.DTOs.Invitacion;
using FitRank_API.Application.DTOs.UsuarioDTOs;
using FitRank_API.Application.DTOs.UsuarioDTOs.ValidarAuth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FitRank_API.Presentacion.Controllers;


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
    private readonly GenerarTokenCasoDeUso _generarTokenLogin;


    public AuthController(
        LoginUsuarioCasoDeUso loginCasoDeUso,
        RegistrarUsuarioCasoDeUso registerCasoDeUso,
        ValidarTokenActivacionCasoDeUso validarTokenActivacionCasoDeUso,
        ActivarCuentaCasoDeUso activarCuentaCasoDeUso,
        AgregarUsuarioConInvitacionCasoDeUso agregarUsuarioConInvitacionCasoDeUso,
        GenerarTokenCasoDeUso generarTokenLogin,
        IConfiguration config)
    {
        _loginCasoDeUso = loginCasoDeUso;
        _registerCasoDeUso = registerCasoDeUso;
        _validarTokenActivacionCasoDeUso = validarTokenActivacionCasoDeUso;
        _activarCuentaCasoDeUso = activarCuentaCasoDeUso;
        _agregarUsuarioConInvitacionCasoDeUso = agregarUsuarioConInvitacionCasoDeUso;
        _generarTokenLogin = generarTokenLogin;
        _config = config;
    }


    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var resultado = await _loginCasoDeUso.Ejecutar(dto);

        if (resultado == null)
            return Unauthorized(new { Mensaje = "Email o password inválido" });

        var (entidad, usuarioDto) = resultado.Value;


        var token = _generarTokenLogin.Ejecutar(entidad);

        return Ok(new AuthResponseDTO
        {
            Token = token,
            User = usuarioDto
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


