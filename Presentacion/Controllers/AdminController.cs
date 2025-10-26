
using FitRank_API.Application.CasosDeUso.AsistenciaCasosDeUso;
using FitRank_API.Application.CasosDeUso.Invitacion;
using FitRank_API.Application.CasosDeUso.Invitacion.RegistrarInvitacionCasoDeUso;
using FitRank_API.Application.CasosDeUso.UsuarioCasosDeUso;
using FitRank_API.Application.DTOs.Asistencia;
using FitRank_API.Application.DTOs.Invitacion;
using FitRank_API.Application.DTOs.QR;
using FitRank_API.Application.DTOs.UsuarioDTOs;
using FitRank_API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] 
    public class AdminController : ControllerBase
    {
        private readonly AgregarInvitacionCasoDeUso _agregarInvitacionCasoDeUso;
        private readonly FallbackEfectivoCasoDeUso _fallbackEfectivoCasoDeUso;
        private readonly EnviarEmailQrCasoDeUso _enviarEmailQrCasoDeUso;
     
        private readonly ValidarQrCasoDeUso _validarQrCasoDeUso;
        public AdminController(
            AgregarInvitacionCasoDeUso agregarInvitacionCasoDeUso,
            FallbackEfectivoCasoDeUso fallbackEfectivoCasoDeUso,
            EnviarEmailQrCasoDeUso enviarEmailQrCasoDeUso,
            ValidarQrCasoDeUso validarQrCasoDeUso)
        {
            _agregarInvitacionCasoDeUso = agregarInvitacionCasoDeUso;
            _fallbackEfectivoCasoDeUso = fallbackEfectivoCasoDeUso;
            _enviarEmailQrCasoDeUso = enviarEmailQrCasoDeUso;
            _validarQrCasoDeUso = validarQrCasoDeUso;
        }

        [HttpPost("generar-invitacion")]
        public async Task<ActionResult<InvitacionResponseDTO>> GenerarInvitacion([FromBody] GenerarInvitacionDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int adminId = 1; 
            var result = await _agregarInvitacionCasoDeUso.Ejecutar(dto, adminId);

            if (!result.Success)
                return BadRequest(new { Mensaje = result.Mensaje });

            return Ok(result);
        }


        [HttpPost("fallback-efectivo")]
        public async Task<ActionResult<InvitacionResponseDTO>> FallbackEfectivo([FromBody] FallbackEfectivoDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var adminIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminIdClaim) || !int.TryParse(adminIdClaim, out int adminId))
                return Unauthorized(new { Mensaje = "Admin ID no válido" });

            var result = await _fallbackEfectivoCasoDeUso.Ejecutar(dto, adminId);
            if (!result.Success)
                return BadRequest(new { Mensaje = result.Mensaje });

            return Ok(result);
        }


        [HttpPost("enviar-email-qr")]
        public async Task<ActionResult<EmailResponseDTO>> EnviarEmailQr([FromBody] EmailDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _enviarEmailQrCasoDeUso.Ejecutar(dto);
            if (!result.Success)
                return BadRequest(new { Mensaje = result.Mensaje });

            return Ok(result);
        }


        [HttpPost("validar-qr")]
        public async Task<ActionResult<QrValidationResponseDTO>> ValidarQr([FromBody] QrValidationDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var adminIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int? adminId = string.IsNullOrEmpty(adminIdClaim) ? null : int.Parse(adminIdClaim);

            var result = await _validarQrCasoDeUso.Ejecutar(dto, adminId);
            if (!result.Valido)
                return BadRequest(new { Mensaje = result.Mensaje });

            return Ok(result);
        }
    }
}
