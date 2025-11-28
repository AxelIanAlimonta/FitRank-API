using FitRank_API.Application.CasosDeUso.AmistadCasosDeUso;
using FitRank_API.Application.DTOs.AmistadDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitRank_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AmistadController : ControllerBase
    {
        private readonly EnviarSolicitudAmistadCasoDeUso _enviarSolicitudCasoDeUso;
        private readonly ObtenerAmigosCasoDeUso _obtenerAmigosCasoDeUso;
        private readonly ObtenerSolicitudesPendientesCasoDeUso _obtenerSolicitudesCasoDeUso;
        private readonly AceptarSolicitudAmistadCasoDeUso _aceptarSolicitudCasoDeUso;
        private readonly EliminarAmigoCasoDeUso _eliminarAmigoCasoDeUso;

        public AmistadController(
            EnviarSolicitudAmistadCasoDeUso enviarSolicitudCasoDeUso,
            ObtenerAmigosCasoDeUso obtenerAmigosCasoDeUso,
            ObtenerSolicitudesPendientesCasoDeUso obtenerSolicitudesCasoDeUso,
            AceptarSolicitudAmistadCasoDeUso aceptarSolicitudCasoDeUso,
            EliminarAmigoCasoDeUso eliminarAmigoCasoDeUso)
        {
            _enviarSolicitudCasoDeUso = enviarSolicitudCasoDeUso;
            _obtenerAmigosCasoDeUso = obtenerAmigosCasoDeUso;
            _obtenerSolicitudesCasoDeUso = obtenerSolicitudesCasoDeUso;
            _aceptarSolicitudCasoDeUso = aceptarSolicitudCasoDeUso;
            _eliminarAmigoCasoDeUso = eliminarAmigoCasoDeUso;
        }

        private int ObtenerUsuarioId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                throw new UnauthorizedAccessException("No se pudo obtener el id de usuario del token.");

            return int.Parse(claim.Value);
        }

        // POST api/amigos/solicitudes
        [HttpPost("solicitudes")]
        public async Task<IActionResult> EnviarSolicitud([FromBody] EnviarSolicitudAmistadDTO dto)
        {
            if (dto == null)
                return BadRequest("El cuerpo de la solicitud no puede ser nulo.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var usuarioId = ObtenerUsuarioId();
                dto.SolicitanteId = usuarioId;

                var resultado = await _enviarSolicitudCasoDeUso.Ejecutar(dto);

                if (!resultado.Completado)
                    return BadRequest(resultado);

                return Ok(resultado);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Mensaje = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        // GET api/amigos
        [HttpGet]
        public async Task<IActionResult> ObtenerAmigos()
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                var amigos = await _obtenerAmigosCasoDeUso.Ejecutar(usuarioId);
                return Ok(amigos);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Mensaje = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        // GET api/amigos/solicitudes
        [HttpGet("solicitudes")]
        public async Task<IActionResult> ObtenerSolicitudesPendientes()
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                var solicitudes = await _obtenerSolicitudesCasoDeUso.Ejecutar(usuarioId);
                return Ok(solicitudes);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Mensaje = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        // POST api/amigos/solicitudes/{amistadId}/aceptar
        [HttpPost("solicitudes/{amistadId:int}/aceptar")]
        public async Task<IActionResult> AceptarSolicitud([FromRoute] int amistadId)
        {
            if (amistadId <= 0)
                return BadRequest(new { Mensaje = "El ID de la amistad debe ser mayor a cero." });

            try
            {
                var usuarioId = ObtenerUsuarioId();

                var dto = new AceptarSolicitudAmistadDTO
                {
                    SocioId = usuarioId,
                    AmistadId = amistadId
                };

                var resultado = await _aceptarSolicitudCasoDeUso.Ejecutar(dto);

                if (!resultado.Completado)
                    return BadRequest(resultado);

                return Ok(resultado);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Mensaje = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        // DELETE api/amigos/{amigoId}
        [HttpDelete("{amigoId:int}")]
        public async Task<IActionResult> EliminarAmigo([FromRoute] int amigoId)
        {
            if (amigoId <= 0)
                return BadRequest(new { Mensaje = "El ID del amigo debe ser mayor a cero." });

            try
            {
                var usuarioId = ObtenerUsuarioId();

                var dto = new EliminarAmigoDTO
                {
                    SocioId = usuarioId,
                    AmigoId = amigoId
                };

                var completado = await _eliminarAmigoCasoDeUso.Ejecutar(dto);

                if (!completado)
                    return BadRequest(new { Completado = false, Mensaje = "No se pudo eliminar la amistad." });

                return Ok(new { Completado = true, Mensaje = "Amistad eliminada correctamente." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Mensaje = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }
    }
}
