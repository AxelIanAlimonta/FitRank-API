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
            IHubContext<NotificacionesHub> notiHub)
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
            if (dto == null)
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });

            if (dto.UsuarioReceptorId <= 0)
                return BadRequest(new { Mensaje = "El ID del usuario receptor debe ser mayor a cero." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var emisorIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrWhiteSpace(emisorIdClaim))
                    return Unauthorized(new { Mensaje = "No se encontró el ID del usuario en el token." });

                if (!long.TryParse(emisorIdClaim, out var emisorId) || emisorId <= 0)
                    return BadRequest(new { Mensaje = "El ID del usuario en el token es inválido." });

                var notificacion = await _enviarIndividual.Ejecutar(
                    emisorId,
                    dto.UsuarioReceptorId,
                    dto.Titulo,
                    dto.Mensaje
                );

                try
                {
                    await _notiHub.Clients
                        .Group($"user-{dto.UsuarioReceptorId}")
                        .SendAsync("NotificacionRecibida", new
                        {
                            id = notificacion.Id,
                            titulo = notificacion.Titulo,
                            mensaje = notificacion.Mensaje,
                            fechaCreacion = notificacion.FechaEnvio
                        });
                }
                catch
                {
                    // Silenciar errores de SignalR para no afectar la respuesta
                }

                return Ok(new
                {
                    Mensaje = "Notificación enviada correctamente.",
                    Notificacion = notificacion
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Profesor")]
        public async Task<IActionResult> Crear([FromBody] AgregarNotificacionDTO dto)
        {
            if (dto == null)
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var notificacionCreada = await _agregarCaso.Ejecutar(dto);

                return CreatedAtAction(nameof(Crear),
                    new { id = notificacionCreada.Id },
                    new
                    {
                        Mensaje = "Notificación creada correctamente.",
                        Notificacion = notificacionCreada
                    });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("usuario")]
        public async Task<IActionResult> ObtenerPorUsuario()
        {
            try
            {
                var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (usuarioIdClaim == null || string.IsNullOrWhiteSpace(usuarioIdClaim.Value))
                    return Unauthorized(new { Mensaje = "No se encontró el ID del usuario en el token." });

                if (!long.TryParse(usuarioIdClaim.Value, out var usuarioId) || usuarioId <= 0)
                    return BadRequest(new { Mensaje = "El ID del usuario en el token es inválido." });

                var notificaciones = await _obtenerCaso.Ejecutar(usuarioId);

                return Ok(new
                {
                    Mensaje = notificaciones.Any()
                        ? "Notificaciones obtenidas correctamente."
                        : "No hay notificaciones disponibles.",
                    Notificaciones = notificaciones
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPost("retener/{socioId}")]
        [Authorize(Roles = "Admin,Profesor")]
        public async Task<IActionResult> RetenerSocio(long socioId)
        {
            if (socioId <= 0)
                return BadRequest(new { Mensaje = "El ID del socio debe ser mayor a cero." });

            try
            {
                var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (adminIdClaim == null || string.IsNullOrWhiteSpace(adminIdClaim.Value))
                    return Unauthorized(new { Mensaje = "No se encontró el ID del usuario en el token." });

                if (!long.TryParse(adminIdClaim.Value, out var adminId) || adminId <= 0)
                    return BadRequest(new { Mensaje = "El ID del usuario en el token es inválido." });

                var resultado = await _retenerSocioCasoDeUso.Ejecutar(adminId, socioId);

                if (!resultado)
                    return BadRequest(new { Mensaje = "No se pudo enviar la notificación de retención." });

                return Ok(new
                {
                    Mensaje = "Notificación de retención enviada al socio correctamente."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPut("marcar-leida/{id}")]
        public async Task<IActionResult> MarcarLeida(long id)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            try
            {
                var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (usuarioIdClaim == null || string.IsNullOrWhiteSpace(usuarioIdClaim.Value))
                    return Unauthorized(new { Mensaje = "No se encontró el ID del usuario en el token." });

                if (!long.TryParse(usuarioIdClaim.Value, out var usuarioId) || usuarioId <= 0)
                    return BadRequest(new { Mensaje = "El ID del usuario en el token es inválido." });

                var resultado = await _marcarNotificacionLeidaCasoDeUso.Ejecutar(usuarioId, id);

                if (!resultado)
                    return BadRequest(new { Mensaje = "No se pudo marcar como leída." });

                return Ok(new
                {
                    Mensaje = "Notificación marcada como leída correctamente."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPost("masiva")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EnviarMasiva([FromBody] EnviarMasivaDTO dto)
        {
            if (dto == null)
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var emisorIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrWhiteSpace(emisorIdClaim))
                    return Unauthorized(new { Mensaje = "No se encontró el ID del usuario en el token." });

                if (!long.TryParse(emisorIdClaim, out var emisorId) || emisorId <= 0)
                    return BadRequest(new { Mensaje = "El ID del usuario en el token es inválido." });

                int cantidad = await _enviarMasiva.Ejecutar(emisorId, dto.Titulo, dto.Mensaje);

                return Ok(new
                {
                    Mensaje = "Notificaciones enviadas correctamente.",
                    Cantidad = cantidad
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("historial")]
        public async Task<IActionResult> ObtenerHistorial()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null || string.IsNullOrWhiteSpace(userIdClaim.Value))
                    return Unauthorized(new { Mensaje = "No se encontró el ID del usuario en el token." });

                if (!long.TryParse(userIdClaim.Value, out var userId) || userId <= 0)
                    return BadRequest(new { Mensaje = "El ID del usuario en el token es inválido." });

                var data = await _obtenerHistorialNoti.Ejecutar(userId);
                return Ok(data);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("usuarios")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ObtenerUsuarios()
        {
            try
            {
                var lista = await _obtenerUsuariosParaNotificacion.Ejecutar();
                return Ok(lista);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }
    }
}
