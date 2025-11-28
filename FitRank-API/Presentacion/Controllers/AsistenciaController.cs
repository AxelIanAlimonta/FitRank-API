using FitRank_API.Application.CasosDeUso.Asistencia;
using FitRank_API.Application.CasosDeUso.AsistenciaCasosDeUso;
using FitRank_API.Application.DTOs.QR;
using FitRank_API.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitRank_API.Presentacion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AsistenciaController : ControllerBase
    {
        private readonly ObtenerAsistenciasPorUsuarioCasoDeUso _obtenerAsistenciasPorUsuarioCasoDeUso;
        private readonly ObtenerAsistenciasPorDiaCasoDeUso _obtenerAsistenciasPorDiaCasoDeUso;
        private readonly ObtenerAsistenciasDetalladasPorUsuarioCasoDeUso _obtenerAsistenciasDetalladasPorUsuarioCasoDeUso;
        private readonly ValidarAsistenciaQrCasoDeUso _validarAsistenciaQrCasoDeUso;
        private readonly ObtenerTodasLasAsistenciasCasoDeUso _obtenerTodasLasAsistenciasCasoDeUso;
        private readonly DetectarSociosInactivosCasoDeUso _detectarSociosInactivosCasoDeUso;
        private readonly ObtenerOcupacionActualCasoDeUso _obtenerOcupacionActualCasoDeUso;



        public AsistenciaController(
            ObtenerAsistenciasPorUsuarioCasoDeUso obtenerAsistenciasPorUsuarioCasoDeUso,
            ObtenerAsistenciasPorDiaCasoDeUso obtenerAsistenciasPorDiaCasoDeUso,
            ObtenerAsistenciasDetalladasPorUsuarioCasoDeUso obtenerAsistenciasDetalladasPorUsuarioCasoDeUso,
            ValidarAsistenciaQrCasoDeUso validarAsistenciaQrCasoDeUso,
            ObtenerTodasLasAsistenciasCasoDeUso obtenerTodasLasAsistenciasCasoDeUso,
            DetectarSociosInactivosCasoDeUso detectarSociosInactivosCasoDeUso,
            ObtenerOcupacionActualCasoDeUso obtenerOcupacionActualCasoDeUso)
        {
            _obtenerAsistenciasPorUsuarioCasoDeUso = obtenerAsistenciasPorUsuarioCasoDeUso;
            _obtenerAsistenciasPorDiaCasoDeUso = obtenerAsistenciasPorDiaCasoDeUso;
            _obtenerAsistenciasDetalladasPorUsuarioCasoDeUso = obtenerAsistenciasDetalladasPorUsuarioCasoDeUso;
            _validarAsistenciaQrCasoDeUso = validarAsistenciaQrCasoDeUso;
            _obtenerTodasLasAsistenciasCasoDeUso = obtenerTodasLasAsistenciasCasoDeUso;
            _detectarSociosInactivosCasoDeUso = detectarSociosInactivosCasoDeUso;
            _obtenerOcupacionActualCasoDeUso = obtenerOcupacionActualCasoDeUso;
        }




        [HttpGet("mias")]
        [Authorize(Roles = "Socio")]
        public async Task<IActionResult> ObtenerMias()
        {
            try
            {
                var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(usuarioIdClaim))
                    return Unauthorized(new { Mensaje = "No se pudo obtener el ID del usuario." });

                var usuarioId = int.Parse(usuarioIdClaim);
                var asistencias = await _obtenerAsistenciasPorUsuarioCasoDeUso.Ejecutar(usuarioId);
                return Ok(asistencias);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("por-dia")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ObtenerAsistenciasPorDia(
            [FromQuery] DateTime? desde = null,
            [FromQuery] DateTime? hasta = null)
        {
            try
            {
                if (desde.HasValue && hasta.HasValue && desde.Value > hasta.Value)
                    return BadRequest(new { Mensaje = "La fecha 'desde' no puede ser mayor que 'hasta'." });

                var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(adminIdClaim) || !int.TryParse(adminIdClaim, out int gimnasioId))
                    return Unauthorized(new { Mensaje = "Admin ID no válido" });

                var resultado = await _obtenerAsistenciasPorDiaCasoDeUso.Ejecutar(gimnasioId, desde, hasta);
                return Ok(resultado);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("todas")]
        [Authorize(Roles = "Admin,Profesor")]
        public async Task<IActionResult> ObtenerTodasLasAsistencias()
        {
            try
            {
                var asistencias = await _obtenerTodasLasAsistenciasCasoDeUso.Ejecutar();
                return Ok(asistencias);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("por-usuario/{usuarioId}")]
        [Authorize(Roles = "Admin,Profesor")]
        public async Task<IActionResult> ObtenerPorUsuario(long usuarioId)
        {
            if (usuarioId <= 0)
                return BadRequest(new { Mensaje = "El ID del usuario debe ser mayor a cero." });

            try
            {
                var asistencias = await _obtenerAsistenciasPorUsuarioCasoDeUso.Ejecutar((int)usuarioId);
                return Ok(asistencias);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("detalle-usuarioAsistencia/{usuarioId}")]
        [Authorize(Roles = "Admin,Profesor,Socio")]
        public async Task<IActionResult> ObtenerAsistenciasDetalladasPorUsuario(long usuarioId)
        {
            if (usuarioId <= 0)
                return BadRequest(new { Mensaje = "El ID del usuario debe ser mayor a cero." });

            try
            {
                var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserIdClaim))
                    return Unauthorized(new { Mensaje = "No se pudo obtener el ID del usuario." });

                var currentUserId = long.Parse(currentUserIdClaim);
                var isSocio = User.IsInRole("Socio");

                if (isSocio && currentUserId != usuarioId)
                    return Forbid("No puedes acceder al historial de otro usuario.");

                var resultado = await _obtenerAsistenciasDetalladasPorUsuarioCasoDeUso.Ejecutar((int)usuarioId);
                return Ok(resultado);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPost("validar-qr")]
        [Authorize(Roles = "Admin,Profesor")]
        public async Task<IActionResult> ValidarQr([FromBody] QrValidationDTO dto)
        {
            if (dto == null)
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(adminIdClaim) || !int.TryParse(adminIdClaim, out int usuarioId))
                    return Unauthorized(new { Mensaje = "Usuario no válido" });

                var result = await _validarAsistenciaQrCasoDeUso.Ejecutar(dto, usuarioId);

                if (!result.Valido)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("socios-inactivos/{dias?}")]
        [Authorize(Roles = "Admin,Profesor")]
        public async Task<IActionResult> ObtenerSociosInactivos(int dias = 5)
        {
            if (dias <= 0)
                return BadRequest(new { Mensaje = "Los días deben ser mayor a cero." });

            try
            {
                var resultado = await _detectarSociosInactivosCasoDeUso.Ejecutar(dias);
                return Ok(resultado);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }


        [HttpGet("ocupacion-actual")]
        [Authorize(Roles = "Admin,Profesor,Socio")]
        public async Task<IActionResult> ObtenerOcupacionActual()
        {
            var gimnasioIdClaim = User.FindFirst(ClaimTypes.GroupSid)?.Value;

            if (string.IsNullOrEmpty(gimnasioIdClaim) || !long.TryParse(gimnasioIdClaim, out long gimnasioId))
                return Unauthorized("No se pudo determinar el gimnasio.");

            var cantidad = await _obtenerOcupacionActualCasoDeUso.Ejecutar(gimnasioId);

            return Ok(new { personasDentro = cantidad });
        }



    }
}
