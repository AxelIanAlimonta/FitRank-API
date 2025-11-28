using FitRank_API.Application.CasosDeUso.SolicitudCasosDeUso;
using FitRank_API.Application.DTOs.SolicitudDTO;
using FitRank_API.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitRank_API.Presentacion.Controllers
{
    [ApiController]
    [Route("api/solicitudes")]
    public class SolicitudRutinaProfesorController : ControllerBase
    {
        private readonly CrearSolicitudRutinaProfesorCasoDeUso _crearCasoDeUso;
        private readonly ISolicitudRutinaProfesorRepositorio _repositorio;
        private readonly TomarSolicitudCasoDeUso _tomarSolicitud;
        private readonly FinalizarSolicitudCasoDeUso _finalizarSolicitud;
        private readonly RechazarSolicitudCasoDeUso _rechazarSolicitud;
        private readonly TerminarSolicitudCasoDeUso _terminarSolicitud;

        public SolicitudRutinaProfesorController(
            CrearSolicitudRutinaProfesorCasoDeUso crearCasoDeUso,
            ISolicitudRutinaProfesorRepositorio repositorio,
            TomarSolicitudCasoDeUso tomarSolicitud,
            FinalizarSolicitudCasoDeUso finalizarSolicitud,
            RechazarSolicitudCasoDeUso rechazarSolicitud,
            TerminarSolicitudCasoDeUso terminarSolicitud)
        {
            _crearCasoDeUso = crearCasoDeUso;
            _repositorio = repositorio;
            _tomarSolicitud = tomarSolicitud;
            _finalizarSolicitud = finalizarSolicitud;
            _rechazarSolicitud = rechazarSolicitud;
            _terminarSolicitud = terminarSolicitud;
        }

        [HttpPost]
        public async Task<ActionResult> Crear(long socioId, [FromBody] CrearSolicitudRutinaProfesorDTO dto)
        {
            if (socioId <= 0)
                return BadRequest(new { Mensaje = "El ID del socio debe ser mayor a cero." });

            if (dto == null)
                return BadRequest(new { Mensaje = "El objeto solicitud no puede ser nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var solicitudId = await _crearCasoDeUso.EjecutarAsync(dto, socioId);
                return Ok(new { SolicitudId = solicitudId, Mensaje = "Solicitud creada correctamente." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPut("{id}/terminar")]
        public async Task<IActionResult> TerminarSolicitud(long id)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID de la solicitud debe ser mayor a cero." });

            try
            {
                var resultado = await _terminarSolicitud.EjecutarAsync(id);
                if (!resultado)
                    return NotFound(new { Mensaje = $"La solicitud con ID {id} no fue encontrada." });

                return Ok(new { Mensaje = "Solicitud finalizada correctamente." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("pendientes")]
        [Authorize(Roles = "Admin,Profesor")]
        public async Task<ActionResult<List<SolicitudRutinaProfesorDTO>>> ObtenerPendientes()
        {
            try
            {
                var solicitudes = await _repositorio.ObtenerPendientesAsync();
                return Ok(solicitudes);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPost("tomar")]
        [Authorize(Roles = "Profesor")]
        public async Task<ActionResult> TomarSolicitud([FromBody] TomarSolicitudDTO dto)
        {
            if (dto == null)
                return BadRequest(new { Mensaje = "El objeto solicitud no puede ser nulo." });

            if (dto.SolicitudId <= 0)
                return BadRequest(new { Mensaje = "El ID de la solicitud debe ser mayor a cero." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var profesorIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!long.TryParse(profesorIdStr, out var profesorId))
                    return Unauthorized(new { Mensaje = "No se pudo obtener el ID del profesor." });

                var ok = await _tomarSolicitud.EjecutarAsync(dto.SolicitudId, profesorId);
                if (!ok)
                    return BadRequest(new { Mensaje = "No se pudo tomar la solicitud. Verifique que esté pendiente." });

                return Ok(new { Mensaje = "Solicitud tomada correctamente.", SolicitudId = dto.SolicitudId });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPost("finalizar")]
        [Authorize(Roles = "Profesor")]
        public async Task<ActionResult> FinalizarSolicitud([FromBody] FinalizarSolicitudDTO dto)
        {
            if (dto == null)
                return BadRequest(new { Mensaje = "El objeto solicitud no puede ser nulo." });

            if (dto.SolicitudId <= 0)
                return BadRequest(new { Mensaje = "El ID de la solicitud debe ser mayor a cero." });

            if (dto.RutinaId <= 0)
                return BadRequest(new { Mensaje = "El ID de la rutina debe ser mayor a cero." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var ok = await _finalizarSolicitud.EjecutarAsync(dto.SolicitudId, dto.RutinaId, dto.MensajeProfesor);
                if (!ok)
                    return BadRequest(new { Mensaje = "No se pudo finalizar la solicitud. Verifique que esté tomada por un profesor." });

                return Ok(new { Mensaje = "Solicitud finalizada correctamente.", SolicitudId = dto.SolicitudId, RutinaId = dto.RutinaId });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPost("rechazar")]
        [Authorize(Roles = "Profesor")]
        public async Task<ActionResult> RechazarSolicitud([FromBody] RechazarSolicitudDTO dto)
        {
            if (dto == null)
                return BadRequest(new { Mensaje = "El objeto solicitud no puede ser nulo." });

            if (dto.SolicitudId <= 0)
                return BadRequest(new { Mensaje = "El ID de la solicitud debe ser mayor a cero." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var profesorIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!long.TryParse(profesorIdStr, out var profesorId))
                    return Unauthorized(new { Mensaje = "No se pudo obtener el ID del profesor." });

                var ok = await _rechazarSolicitud.EjecutarAsync(dto.SolicitudId, profesorId, dto.MensajeProfesor);
                if (!ok)
                    return BadRequest(new { Mensaje = "No se pudo rechazar la solicitud. Verifique que esté pendiente." });

                return Ok(new { Mensaje = "Solicitud rechazada correctamente.", SolicitudId = dto.SolicitudId });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }
    }
}
