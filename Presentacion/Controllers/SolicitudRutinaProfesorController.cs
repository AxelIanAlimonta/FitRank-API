using FitRank_API.Application.CasosDeUso.SolicitudCasosDeUso;
using FitRank_API.Application.DTOs.SolicitudDTO;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
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
        //[Authorize(Roles = "Socio")]
        public async Task<ActionResult> Crear(long socioId, [FromBody] CrearSolicitudRutinaProfesorDTO dto)
        {
            /*
            var socioIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!long.TryParse(socioIdStr, out var socioId))
            return Unauthorized();
            */

            var solicitudId = await _crearCasoDeUso.EjecutarAsync(dto, socioId);
            return Ok(new { SolicitudId = solicitudId });
        }

        [HttpPut("{id}/terminar")]
        public async Task<IActionResult> TerminarSolicitud(long id)
        {
            await _terminarSolicitud.EjecutarAsync(id);
            return Ok(new { mensaje = "Solicitud finalizada correctamente." });
        }

        [HttpGet("pendientes")]
        //[Authorize(Roles = "Profesor")]
        public async Task<ActionResult<List<SolicitudRutinaProfesorDTO>>> ObtenerPendientes()
        {
            var solicitudes = await _repositorio.ObtenerPendientesAsync();
            return Ok(solicitudes);
        }

        [HttpPost("tomar")]
        [Authorize(Roles = "Profesor")]
        public async Task<ActionResult> TomarSolicitud([FromBody] TomarSolicitudDTO dto)
        {
            var profesorIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!long.TryParse(profesorIdStr, out var profesorId))
                return Unauthorized();

            var ok = await _tomarSolicitud.EjecutarAsync(dto.SolicitudId, profesorId);
            return ok ? Ok() : BadRequest("No se pudo tomar la solicitud.");
        }

        [HttpPost("finalizar")]
        [Authorize(Roles = "Profesor")]
        public async Task<ActionResult> FinalizarSolicitud([FromBody] FinalizarSolicitudDTO dto)
        {
            var ok = await _finalizarSolicitud.EjecutarAsync(dto.SolicitudId, dto.RutinaId, dto.MensajeProfesor);
            return ok ? Ok() : BadRequest("No se pudo finalizar la solicitud.");
        }

        [HttpPost("rechazar")]
        [Authorize(Roles = "Profesor")]
        public async Task<ActionResult> RechazarSolicitud([FromBody] RechazarSolicitudDTO dto)
        {
            var profesorIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!long.TryParse(profesorIdStr, out var profesorId))
                return Unauthorized();

            var ok = await _rechazarSolicitud.EjecutarAsync(dto.SolicitudId, profesorId, dto.MensajeProfesor);
            return ok ? Ok() : BadRequest("No se pudo rechazar la solicitud.");
        }
    }
    }
