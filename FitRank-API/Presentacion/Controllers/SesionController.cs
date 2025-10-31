using FitRank_API.Application.DTOs;
using FitRank_API.Application.CasosDeUso.SesionCasosDeUso;
using Microsoft.AspNetCore.Mvc;

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
            var sesiones = await _obtenerTodasLasSesionCasoDeUso.Ejecutar();
            return Ok(sesiones);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(long id)
        {
            var sesion = await _obtenerSesionCasoDeUso.Ejecutar(id);
            if (sesion == null) return NotFound();
            return Ok(sesion);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] AgregarSesionDTO dto)
        {
            var nueva = await _agregarSesionCasoDeUso.Ejecutar(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = nueva.Id }, nueva);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarSesionDTO dto)
        {
            if (id != dto.Id) return BadRequest("El ID no coincide.");
            var actualizada = await _actualizarSesionCasoDeUso.Ejecutar(id,dto);
            if (actualizada == null) return NotFound();
            return Ok(actualizada);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(long id)
        {
            var eliminado = await _eliminarSesionCasoDeUso.Ejecutar(id);
            if (!eliminado) return NotFound();
            return NoContent();
        }
    }
}
