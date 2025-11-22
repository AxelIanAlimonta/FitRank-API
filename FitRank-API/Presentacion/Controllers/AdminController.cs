
using System.Security.Claims;
using FitRank_API.Application.CasosDeUso.AdministradorCasosDeUso;
using FitRank_API.Application.CasosDeUso.AsistenciaCasosDeUso;
using FitRank_API.Application.CasosDeUso.Invitacion;
using FitRank_API.Application.CasosDeUso.Invitacion.RegistrarInvitacionCasoDeUso;
using FitRank_API.Application.CasosDeUso.UsuarioCasosDeUso;
using FitRank_API.Application.DTOs.AdministradorDTOs;
using FitRank_API.Application.DTOs.Asistencia;
using FitRank_API.Application.DTOs.Invitacion;
using FitRank_API.Application.DTOs.QR;
using FitRank_API.Application.DTOs.UsuarioDTOs;
using FitRank_API.Application.Interfaces;
using FitRank_API.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AdminController : ControllerBase
    {
        private readonly AgregarInvitacionCasoDeUso _agregarInvitacionCasoDeUso;
        private readonly FallbackEfectivoCasoDeUso _fallbackEfectivoCasoDeUso;
        private readonly EnviarEmailQrCasoDeUso _enviarEmailQrCasoDeUso;
        private readonly AgregarAdministradorCasoDeUso _agregarAdministradorCasoDeUso;
        private readonly EliminarAdministradorCasoDeUso _eliminarAdministradorCasoDeUso;
        private readonly ValidarQrCasoDeUso _validarQrCasoDeUso;
        private readonly ObtenerAdministradorCasoDeUso obtenerAdministradorCasoDeUso;
 
        public AdminController(AgregarInvitacionCasoDeUso agregarInvitacionCasoDeUso,
            FallbackEfectivoCasoDeUso fallbackEfectivoCasoDeUso,
            EnviarEmailQrCasoDeUso enviarEmailQrCasoDeUso,
            AgregarAdministradorCasoDeUso agregarAdministradorCasoDeUso,
            EliminarAdministradorCasoDeUso eliminarAdministradorCasoDeUso,
            ValidarQrCasoDeUso validarQrCasoDeUso,
            ObtenerAdministradorCasoDeUso obtenerAdministradorCasoDeUso)
        {
            _agregarInvitacionCasoDeUso = agregarInvitacionCasoDeUso;
            _fallbackEfectivoCasoDeUso = fallbackEfectivoCasoDeUso;
            _enviarEmailQrCasoDeUso = enviarEmailQrCasoDeUso;
            _agregarAdministradorCasoDeUso = agregarAdministradorCasoDeUso;
            _eliminarAdministradorCasoDeUso = eliminarAdministradorCasoDeUso;
            _validarQrCasoDeUso = validarQrCasoDeUso;
            this.obtenerAdministradorCasoDeUso = obtenerAdministradorCasoDeUso;
        }


        [HttpPost("generar-invitacion")]

        public async Task<ActionResult<InvitacionResponseDTO>> GenerarInvitacion([FromBody] GenerarInvitacionDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // ✅ Obtener el ID del admin desde el token JWT
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(adminIdClaim))
                return Unauthorized(new { Mensaje = "No se pudo identificar al administrador autenticado." });

            var adminId = int.Parse(adminIdClaim);

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


        [HttpPost("crear-admin")]
        public async Task<ActionResult<Administrador>> Agregar([FromBody] AgregarAdministradorDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var admin = await _agregarAdministradorCasoDeUso.Ejecutar(dto);
            return Ok(admin);
        }

        [HttpDelete("eliminar-admin/{id}")]
        public async Task<IActionResult> Eliminar(long id)
        {
            var result = await _eliminarAdministradorCasoDeUso.Ejecutar(id);

            if (!result)
                return NotFound(new { Mensaje = "Administrador no encontrado" });

            return Ok(new { Mensaje = "Administrador eliminado correctamente" });
        }


        [HttpGet]

        public async Task<IActionResult> ObtenerTodosLosAdministradores()
        {
            var result = await obtenerAdministradorCasoDeUso.Ejecutar();
            return Ok(result);
        }

    }
}
