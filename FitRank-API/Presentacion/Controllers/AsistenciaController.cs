using FitRank_API.Application.CasosDeUso.Asistencia;
using FitRank_API.Application.CasosDeUso.AsistenciaCasosDeUso;
using FitRank_API.Application.DTOs.QR;
using FitRank_API.Infrastructure.Interfaces;
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

        public AsistenciaController(
            ObtenerAsistenciasPorUsuarioCasoDeUso obtenerAsistenciasPorUsuarioCasoDeUso,
            ObtenerAsistenciasPorDiaCasoDeUso obtenerAsistenciasPorDiaCasoDeUso,
            ObtenerAsistenciasDetalladasPorUsuarioCasoDeUso obtenerAsistenciasDetalladasPorUsuarioCasoDeUso,
            ValidarAsistenciaQrCasoDeUso validarAsistenciaQrCasoDeUso,
            ObtenerTodasLasAsistenciasCasoDeUso obtenerTodasLasAsistenciasCasoDeUso,
            DetectarSociosInactivosCasoDeUso detectarSociosInactivosCasoDeUso)
        {
            _obtenerAsistenciasPorUsuarioCasoDeUso = obtenerAsistenciasPorUsuarioCasoDeUso;
            _obtenerAsistenciasPorDiaCasoDeUso = obtenerAsistenciasPorDiaCasoDeUso;
            _obtenerAsistenciasDetalladasPorUsuarioCasoDeUso = obtenerAsistenciasDetalladasPorUsuarioCasoDeUso;
            _validarAsistenciaQrCasoDeUso = validarAsistenciaQrCasoDeUso;
            _obtenerTodasLasAsistenciasCasoDeUso = obtenerTodasLasAsistenciasCasoDeUso;
            _detectarSociosInactivosCasoDeUso = detectarSociosInactivosCasoDeUso;
        }





        [HttpGet("mias")]
        [Authorize(Roles = "Socio")]
        public async Task<IActionResult> ObtenerMias()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var asistencias = await _obtenerAsistenciasPorUsuarioCasoDeUso.Ejecutar(usuarioId);
            return Ok(asistencias);
        }

        [HttpGet("por-dia")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ObtenerAsistenciasPorDia(
            [FromQuery] DateTime? desde = null,
            [FromQuery] DateTime? hasta = null)
        {
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminIdClaim) || !int.TryParse(adminIdClaim, out int gimnasioId))
                return Unauthorized(new { Mensaje = "Admin ID no válido" });

            var resultado = await _obtenerAsistenciasPorDiaCasoDeUso.Ejecutar(gimnasioId, desde, hasta);
            return Ok(resultado);
        }
        [HttpGet("todas")]
        [Authorize(Roles = "Admin,Profesor")]
        public async Task<IActionResult> ObtenerTodasLasAsistencias()
        {
            var asistencias = await _obtenerTodasLasAsistenciasCasoDeUso.Ejecutar();
            return Ok(asistencias);
        }



        [HttpGet("por-usuario/{usuarioId}")]
        [Authorize(Roles = "Admin,Profesor")]
        public async Task<IActionResult> ObtenerPorUsuario(long usuarioId)
        {
            var asistencias = await _obtenerAsistenciasPorUsuarioCasoDeUso.Ejecutar((int)usuarioId);
            return Ok(asistencias);
        }


        [HttpGet("detalle-usuarioAsistencia/{usuarioId}")]
        [Authorize(Roles = "Admin,Profesor,Socio")]
        public async Task<IActionResult> ObtenerAsistenciasDetalladasPorUsuario(long usuarioId)
        {

            var currentUserId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var isSocio = User.IsInRole("Socio");

            if (isSocio && currentUserId != usuarioId)
                return Forbid("No puedes acceder al historial de otro usuario.");

            var resultado = await _obtenerAsistenciasDetalladasPorUsuarioCasoDeUso.Ejecutar((int)usuarioId);
            return Ok(resultado);
        }


        [HttpPost("validar-qr")]
        [Authorize(Roles = "Admin,Profesor")]
        public async Task<IActionResult> ValidarQr([FromBody] QrValidationDTO dto)
        {
            
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            
            if (string.IsNullOrEmpty(adminIdClaim) || !int.TryParse(adminIdClaim, out int usuarioId))
                return Unauthorized(new { Mensaje = "Usuario no válido" });

          
            var result = await _validarAsistenciaQrCasoDeUso.Ejecutar(dto, usuarioId);

            if (!result.Valido)
                return BadRequest(result);

            return Ok(result);
        }


        [HttpGet("socios-inactivos/{dias?}")]
        [Authorize(Roles = "Admin,Profesor")]
        public async Task<IActionResult> ObtenerSociosInactivos(int dias = 5)
        {
            var resultado = await _detectarSociosInactivosCasoDeUso.Ejecutar(dias);
            return Ok(resultado);
        }

    }
}
