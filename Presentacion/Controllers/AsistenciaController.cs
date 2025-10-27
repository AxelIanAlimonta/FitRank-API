using FitRank_API.Application.CasosDeUso.AsistenciaCasosDeUso;
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

        public AsistenciaController(
             ObtenerAsistenciasPorUsuarioCasoDeUso obtenerAsistenciasPorUsuarioCasoDeUso,
             ObtenerAsistenciasPorDiaCasoDeUso obtenerAsistenciasPorDiaCasoDeUso,
             ObtenerAsistenciasDetalladasPorUsuarioCasoDeUso obtenerAsistenciasDetalladasPorUsuarioCasoDeUso)
        {
            _obtenerAsistenciasPorUsuarioCasoDeUso = obtenerAsistenciasPorUsuarioCasoDeUso;
            _obtenerAsistenciasPorDiaCasoDeUso = obtenerAsistenciasPorDiaCasoDeUso;
            _obtenerAsistenciasDetalladasPorUsuarioCasoDeUso = obtenerAsistenciasDetalladasPorUsuarioCasoDeUso;
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


        [HttpGet("por-usuario/{usuarioId}")]
        [Authorize(Roles = "Admin,Profesor")]
        public async Task<IActionResult> ObtenerPorUsuario(long usuarioId)
        {
            var asistencias = await _obtenerAsistenciasPorUsuarioCasoDeUso.Ejecutar((int)usuarioId);
            return Ok(asistencias);
        }


        [HttpGet("detalle-usuarioAsistencia/{usuarioId}")]
        [Authorize(Roles = "Admin,Profesor")]
        public async Task<IActionResult> ObtenerAsistenciasDetalladasPorUsuario(long usuarioId)
        {

            var currentUserId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var isSocio = User.IsInRole("Socio");

            if (isSocio && currentUserId != usuarioId)
                return Forbid("No puedes acceder al historial de otro usuario.");

            var resultado = await _obtenerAsistenciasDetalladasPorUsuarioCasoDeUso.Ejecutar((int)usuarioId);
            return Ok(resultado);
        }

    }
}
