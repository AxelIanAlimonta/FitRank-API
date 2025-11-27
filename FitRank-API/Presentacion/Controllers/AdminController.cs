using System.Security.Claims;
using FitRank_API.Application.CasosDeUso.AdministradorCasosDeUso;
using FitRank_API.Application.CasosDeUso.AsistenciaCasosDeUso;
using FitRank_API.Application.CasosDeUso.Invitacion;
using FitRank_API.Application.CasosDeUso.Invitacion.RegistrarInvitacionCasoDeUso;
using FitRank_API.Application.CasosDeUso.SocioCasosDeUso;
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
        private readonly ObtenerAdministradorCasoDeUso _obtenerAdministradorCasoDeUso;
        private readonly BorrarSocioCompletoCasoDeUso _borrarSocioCompletoCasoDeUso;

        public AdminController(
            AgregarInvitacionCasoDeUso agregarInvitacionCasoDeUso,
            FallbackEfectivoCasoDeUso fallbackEfectivoCasoDeUso,
            EnviarEmailQrCasoDeUso enviarEmailQrCasoDeUso,
            AgregarAdministradorCasoDeUso agregarAdministradorCasoDeUso,
            EliminarAdministradorCasoDeUso eliminarAdministradorCasoDeUso,
            ValidarQrCasoDeUso validarQrCasoDeUso,
            ObtenerAdministradorCasoDeUso obtenerAdministradorCasoDeUso,
            BorrarSocioCompletoCasoDeUso borrarSocioCompletoCasoDeUso
            )
        {
            _agregarInvitacionCasoDeUso = agregarInvitacionCasoDeUso;
            _fallbackEfectivoCasoDeUso = fallbackEfectivoCasoDeUso;
            _enviarEmailQrCasoDeUso = enviarEmailQrCasoDeUso;
            _agregarAdministradorCasoDeUso = agregarAdministradorCasoDeUso;
            _eliminarAdministradorCasoDeUso = eliminarAdministradorCasoDeUso;
            _validarQrCasoDeUso = validarQrCasoDeUso;
            _obtenerAdministradorCasoDeUso = obtenerAdministradorCasoDeUso;
            _borrarSocioCompletoCasoDeUso = borrarSocioCompletoCasoDeUso;
        }

        [HttpPost("generar-invitacion")]
        public async Task<IActionResult> GenerarInvitacion([FromBody] GenerarInvitacionDTO dto)
        {
            if (dto == null)
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminIdClaim))
                return Unauthorized(new { Mensaje = "No se pudo identificar al administrador autenticado." });

            var adminId = int.Parse(adminIdClaim);

            try
            {
                var result = await _agregarInvitacionCasoDeUso.Ejecutar(dto, adminId);

                if (!result.Success)
                    return BadRequest(new { Mensaje = result.Mensaje });

                return Ok(result);
            }
            catch (Exception ex)
            {
                if (ex.Message == "EMAIL_DUPLICADO")
                    return BadRequest(new { mensaje = "EMAIL_DUPLICADO", socioId = ex.Data["socioId"] });

                if (ex.Message == "DNI_DUPLICADO")
                    return BadRequest(new { mensaje = "DNI_DUPLICADO", socioId = ex.Data["socioId"] });

                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPost("fallback-efectivo")]
        public async Task<IActionResult> FallbackEfectivo([FromBody] FallbackEfectivoDTO dto)
        {
            if (dto == null)
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminIdClaim) || !int.TryParse(adminIdClaim, out int adminId))
                return Unauthorized(new { Mensaje = "Admin ID no válido" });

            try
            {
                var result = await _fallbackEfectivoCasoDeUso.Ejecutar(dto, adminId);
                if (!result.Success)
                    return BadRequest(new { Mensaje = result.Mensaje });

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPost("enviar-email-qr")]
        public async Task<IActionResult> EnviarEmailQr([FromBody] EmailDTO dto)
        {
            if (dto == null)
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _enviarEmailQrCasoDeUso.Ejecutar(dto);
                if (!result.Success)
                    return BadRequest(new { Mensaje = result.Mensaje });

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPost("validar-qr")]
        public async Task<IActionResult> ValidarQr([FromBody] QrValidationDTO dto)
        {
            if (dto == null)
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int? adminId = string.IsNullOrEmpty(adminIdClaim) ? null : int.Parse(adminIdClaim);

                var result = await _validarQrCasoDeUso.Ejecutar(dto, adminId);
                if (!result.Valido)
                    return BadRequest(new { Mensaje = result.Mensaje });

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPost("crear-admin")]
        public async Task<IActionResult> Agregar([FromBody] AgregarAdministradorDTO dto)
        {
            if (dto == null)
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var admin = await _agregarAdministradorCasoDeUso.Ejecutar(dto);
                return CreatedAtAction(nameof(Agregar), new { id = admin.Id }, admin);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpDelete("eliminar-admin/{id}")]
        public async Task<IActionResult> Eliminar(long id)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            try
            {
                var result = await _eliminarAdministradorCasoDeUso.Ejecutar(id);

                if (!result)
                    return NotFound(new { Mensaje = "Administrador no encontrado" });

                return Ok(new { Mensaje = "Administrador eliminado correctamente" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodosLosAdministradores()
        {
            try
            {
                var result = await _obtenerAdministradorCasoDeUso.Ejecutar();
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpDelete("borrar-completo/{usuarioId}")]
        public async Task<IActionResult> BorrarCompleto(long usuarioId)
        {
            if (usuarioId <= 0)
                return BadRequest(new { Mensaje = "El ID del usuario debe ser mayor a cero." });

            try
            {
                var resultado = await _borrarSocioCompletoCasoDeUso.Ejecutar(usuarioId);
                return Ok(resultado);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }
    }
}
