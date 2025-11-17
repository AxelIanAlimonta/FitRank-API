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
                throw new Exception("No se pudo obtener el id de usuario del token.");

            return int.Parse(claim.Value);
        }

        // POST api/amigos/solicitudes
        [HttpPost("solicitudes")]
        public async Task<IActionResult> EnviarSolicitud([FromBody] EnviarSolicitudAmistadDTO dto)
        {
            var usuarioId = ObtenerUsuarioId();

            dto.SolicitanteId = usuarioId;

            var resultado = await _enviarSolicitudCasoDeUso.Ejecutar(dto);

            if (!resultado.Completado)
                return BadRequest(resultado);

            return Ok(resultado);
        }

        // GET api/amigos
        [HttpGet]
        public async Task<IActionResult> ObtenerAmigos()
        {
            var usuarioId = ObtenerUsuarioId();

            var amigos = await _obtenerAmigosCasoDeUso.Ejecutar(usuarioId);

            return Ok(amigos);
        }

        // GET api/amigos/solicitudes
        [HttpGet("solicitudes")]
        public async Task<IActionResult> ObtenerSolicitudesPendientes()
        {
            var usuarioId = ObtenerUsuarioId();

            var solicitudes = await _obtenerSolicitudesCasoDeUso.Ejecutar(usuarioId);

            return Ok(solicitudes);
        }

        // POST api/amigos/solicitudes/{amistadId}/aceptar
        [HttpPost("solicitudes/{amistadId:int}/aceptar")]
        public async Task<IActionResult> AceptarSolicitud([FromRoute] int amistadId)
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

        // DELETE api/amigos/{amigoId}
        [HttpDelete("{amigoId:int}")]
        public async Task<IActionResult> EliminarAmigo([FromRoute] int amigoId)
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
    }
}
