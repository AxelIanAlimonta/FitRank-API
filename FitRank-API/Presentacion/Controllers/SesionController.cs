using FitRank_API.Application.DTOs;
using FitRank_API.Application.CasosDeUso.SesionCasosDeUso;
using Microsoft.AspNetCore.Mvc;
using FitRank_API.Application.DTOs.SesionDTOs;

namespace FitRank_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SesionController : ControllerBase
    {
        private readonly AgregarSesionCasoDeUso _agregarSesionCasoDeUso;
        private readonly ActualizarSesionCasoDeUso _actualizarSesionCasoDeUso;
        private readonly EliminarSesionCasoDeUso _eliminarSesionCasoDeUso;
        private readonly ObtenerSesionPorIdCasoDeUso _obtenerSesionCasoDeUso;
        private readonly ObtenerTodasLasSesionesCasoDeUso _obtenerTodasLasSesionCasoDeUso;

        public SesionController(AgregarSesionCasoDeUso agregarSesionCasoDeUso, ActualizarSesionCasoDeUso actualizarSesionCasoDeUso, EliminarSesionCasoDeUso eliminarSesionCasoDeUso, ObtenerSesionPorIdCasoDeUso obtenerSesionCasoDeUso, ObtenerTodasLasSesionesCasoDeUso obtenerTodasLasSesionesCasoDeUso)
        {
            _agregarSesionCasoDeUso = agregarSesionCasoDeUso;
            _actualizarSesionCasoDeUso = actualizarSesionCasoDeUso;
            _eliminarSesionCasoDeUso = eliminarSesionCasoDeUso;
            _obtenerSesionCasoDeUso = obtenerSesionCasoDeUso;
            _obtenerTodasLasSesionCasoDeUso = obtenerTodasLasSesionesCasoDeUso;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            try
            {
                var sesiones = await _obtenerTodasLasSesionCasoDeUso.Ejecutar();
                return Ok(sesiones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(long id)
        {
            try
            {
                var sesion = await _obtenerSesionCasoDeUso.Ejecutar(id);
                if (sesion == null) return NotFound("Sesión no encontrada.");
                return Ok(sesion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] AgregarSesionDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Solicitud inválida.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var nueva = await _agregarSesionCasoDeUso.Ejecutar(dto);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = nueva.Id }, nueva);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarSesionDTO dto)
        {
            if (dto == null) return BadRequest("Solicitud inválida.");
            if (id != dto.Id) return BadRequest("El ID no coincide.");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var actualizada = await _actualizarSesionCasoDeUso.Ejecutar(dto);
                if (actualizada == null) return NotFound("Sesión no encontrada.");
                return Ok(actualizada);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(long id)
        {
            try
            {
                var eliminado = await _eliminarSesionCasoDeUso.Ejecutar(id);
                if (!eliminado) return NotFound("Sesión no encontrada.");
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}
