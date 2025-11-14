using System.Security.Claims;
using FitRank_API.Application.CasosDeUso.NotificacionCasoDeUso;
using FitRank_API.Application.CasosDeUso.NotificacionCasosDeUso;
using FitRank_API.Application.DTOs.NotificacionDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using FitRank_API.Application.Hubs;


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
        private readonly EnviarNotificacionMasivaCasoDeUso _enviarMasiva;
        private readonly ObtenerHistorialNotificacionesCasoDeUso _obtenerHistorialNoti;
        private readonly ObtenerUsuariosParaNotificacionCasoDeUso _obtenerUsuariosParaNotificacion;
        private readonly EnviarNotificacionIndividualCasoDeUso _enviarIndividual;
        private readonly IHubContext<NotificacionesHub> _notiHub;


        public NotificacionController(
            AgregarNotificacionCasoDeUso agregarCaso,
            ObtenerNotificacionPorUsuarioCasoDeUso obtenerCaso,
            RetenerSocioCasoDeUso retenerSocioCasoDeUso,
            MarcarNotificacionLeidaCasoDeUso marcarNotificacionLeidaCasoDeUso,
            EnviarNotificacionMasivaCasoDeUso enviarMasiva,
            ObtenerHistorialNotificacionesCasoDeUso obtenerHistorialNoti,
            ObtenerUsuariosParaNotificacionCasoDeUso obtenerUsuariosParaNotificacion,
            EnviarNotificacionIndividualCasoDeUso enviarIndividual,
            IHubContext<NotificacionesHub> notiHub
        )
        {
            _agregarCaso = agregarCaso;
            _obtenerCaso = obtenerCaso;
            _retenerSocioCasoDeUso = retenerSocioCasoDeUso;
            _marcarNotificacionLeidaCasoDeUso = marcarNotificacionLeidaCasoDeUso;
            _enviarMasiva = enviarMasiva;
            _obtenerHistorialNoti = obtenerHistorialNoti;
            _obtenerUsuariosParaNotificacion = obtenerUsuariosParaNotificacion;
            _enviarIndividual = enviarIndividual;
            _notiHub = notiHub;
        }

        [HttpPost("enviar")]
        [Authorize(Roles = "Admin,Profesor")]
        public async Task<IActionResult> EnviarIndividual([FromBody] EnviarIndividualDTO dto)
        {
            if (dto == null || dto.UsuarioReceptorId <= 0)
                return BadRequest(new { exito = false, mensaje = "Datos inválidos." });

            long emisorId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var notificacion = await _enviarIndividual.Ejecutar(
                emisorId,
                dto.UsuarioReceptorId,
                dto.Titulo,
                dto.Mensaje
            );

            await _notiHub.Clients
                .Group($"user-{dto.UsuarioReceptorId}")
                .SendAsync("NotificacionRecibida", new
                {
                    id = notificacion.Id,
                    titulo = notificacion.Titulo,
                    mensaje = notificacion.Mensaje,
                    fechaCreacion = notificacion.FechaEnvio
                });
            return Ok(new
            {
                exito = true,
                mensaje = "📩 Notificación enviada correctamente.",
                notificacion
            });
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

        [HttpPost("masiva")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EnviarMasiva([FromBody] EnviarMasivaDTO dto)
        {
            long emisorId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            int cantidad = await _enviarMasiva.Ejecutar(emisorId, dto.Titulo, dto.Mensaje);

            return Ok(new
            {
                exito = true,
                mensaje = "Notificaciones enviadas correctamente",
                cantidad
            });
        }

        [HttpGet("historial")]
        public async Task<IActionResult> ObtenerHistorial()
        {
            long userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var data = await _obtenerHistorialNoti.Ejecutar(userId);
            return Ok(data);
        }



        [HttpGet("usuarios")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ObtenerUsuarios()
        {
            var lista = await _obtenerUsuariosParaNotificacion.Ejecutar();
            return Ok(lista);
        }


    }
}
