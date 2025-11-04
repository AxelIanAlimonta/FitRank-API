using System.Security.Claims;
using FitRank_API.Application.CasosDeUso.NotificacionCasosDeUso;
using FitRank_API.Application.DTOs.NotificacionDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificacionController : ControllerBase
    {
        private readonly AgregarNotificacionCasoDeUso _agregarCaso;
        private readonly ObtenerNotificacionPorUsuarioCasoDeUso _obtenerCaso;
        private readonly RetenerSocioCasoDeUso _retenerSocioCasoDeUso;
        private readonly MarcarNotificacionLeidaCasoDeUso _marcarNotificacionLeidaCasoDeUso;

        public NotificacionController(
            AgregarNotificacionCasoDeUso agregarCaso,
            ObtenerNotificacionPorUsuarioCasoDeUso obtenerCaso,
            RetenerSocioCasoDeUso retenerSocioCasoDeUso,
            MarcarNotificacionLeidaCasoDeUso marcarNotificacionLeidaCasoDeUso)
        {
            _agregarCaso = agregarCaso;
            _obtenerCaso = obtenerCaso;
            _retenerSocioCasoDeUso = retenerSocioCasoDeUso;
            _marcarNotificacionLeidaCasoDeUso = marcarNotificacionLeidaCasoDeUso;
        }

        // 🔹 Crear una notificación manual (por ejemplo, mensaje directo)
        [HttpPost]
        [Authorize(Roles = "Admin,Profesor")]
        public async Task<IActionResult> Crear([FromBody] AgregarNotificacionDTO dto)
        {
            if (dto == null)
                return BadRequest(new { exito = false, mensaje = "Datos de notificación inválidos." });

            var notificacionCreada = await _agregarCaso.Ejecutar(dto);

            return CreatedAtAction(nameof(Crear),
                new { id = notificacionCreada.Id },
                new
                {
                    exito = true,
                    mensaje = "✅ Notificación creada correctamente.",
                    notificacion = notificacionCreada
                });
        }

        // 🔹 Obtener todas las notificaciones del usuario logueado
        [HttpGet("usuario")]
        public async Task<IActionResult> ObtenerPorUsuario()
        {
            var usuarioId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var notificaciones = await _obtenerCaso.Ejecutar(usuarioId);

            return Ok(new
            {
                exito = true,
                mensaje = notificaciones.Any()
                    ? "Notificaciones obtenidas correctamente."
                    : "No hay notificaciones disponibles.",
                notificaciones
            });
        }

        // 🔹 Enviar una notificación de retención (cuando un socio lleva días sin asistir)
        [HttpPost("retener/{socioId}")]
        [Authorize(Roles = "Admin,Profesor")]
        public async Task<IActionResult> RetenerSocio(long socioId)
        {
            var adminId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var resultado = await _retenerSocioCasoDeUso.Ejecutar(adminId, socioId);

            if (!resultado)
                return BadRequest(new { exito = false, mensaje = "No se pudo enviar la notificación de retención." });

            return Ok(new
            {
                exito = true,
                mensaje = "📩 Notificación de retención enviada al socio correctamente."
            });
        }

        // 🔹 Marcar una notificación como leída
        [HttpPut("marcar-leida/{id}")]
        public async Task<IActionResult> MarcarLeida(long id)
        {
            var usuarioId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var resultado = await _marcarNotificacionLeidaCasoDeUso.Ejecutar(usuarioId, id);

            if (!resultado)
                return BadRequest(new { exito = false, mensaje = "No se pudo marcar como leída." });

            return Ok(new
            {
                exito = true,
                mensaje = "✅ Notificación marcada como leída correctamente."
            });
        }
    }
}
