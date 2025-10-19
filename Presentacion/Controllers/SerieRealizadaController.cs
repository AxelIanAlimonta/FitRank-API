using FitRank_API.Application.CasosDeUso.SerieRealizadaCasosDeUso;
using FitRank_API.Application.DTOs.SerieRealizadaDTOs;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SerieRealizadaController : ControllerBase
    {
        // Implementación del controlador para SerieRealizada
        private readonly ObtenerTodasLasSerieRealizadaCasoDeUso _obtenerTodasLasSerieRealizadaCasoDeUso;
        private readonly ObtenerSerieRealizadaPorIdCasoDeUso _obtenerSerieRealizadaPorIdCasoDeUso;
        private readonly AgregarSerieRealizadaCasoDeUso _agregarSerieRealizadaCasoDeUso;
        private readonly ActualizarSerieRealizadaCasoDeUso _actualizarSerieRealizadaCasoDeUso;
        private readonly EliminarSerieRealizadaCasoDeUso _eliminarSerieRealizadaCasoDeUso;

        public SerieRealizadaController(
            ObtenerTodasLasSerieRealizadaCasoDeUso obtenerTodasLasSerieRealizadaCasoDeUso,
            ObtenerSerieRealizadaPorIdCasoDeUso obtenerSerieRealizadaPorIdCasoDeUso,
            AgregarSerieRealizadaCasoDeUso agregarSerieRealizadaCasoDeUso,
            ActualizarSerieRealizadaCasoDeUso actualizarSerieRealizadaCasoDeUso,
            EliminarSerieRealizadaCasoDeUso eliminarSerieRealizadaCasoDeUso)
        {
            _obtenerTodasLasSerieRealizadaCasoDeUso = obtenerTodasLasSerieRealizadaCasoDeUso;
            _obtenerSerieRealizadaPorIdCasoDeUso = obtenerSerieRealizadaPorIdCasoDeUso;
            _agregarSerieRealizadaCasoDeUso = agregarSerieRealizadaCasoDeUso;
            _actualizarSerieRealizadaCasoDeUso = actualizarSerieRealizadaCasoDeUso;
            _eliminarSerieRealizadaCasoDeUso = eliminarSerieRealizadaCasoDeUso;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            var seriesRealizadas = await _obtenerTodasLasSerieRealizadaCasoDeUso.Ejecutar();
            return Ok(seriesRealizadas);
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> ObtenerPorId(long id)
        {
            var serieRealizada = await _obtenerSerieRealizadaPorIdCasoDeUso.Ejecutar(id);
            if (serieRealizada == null)
            {
                return NotFound();
            }
            return Ok(serieRealizada);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] AgregarSerieRealizadaDTO serieRealizada)
        {
            var nuevaSerieRealizada = await _agregarSerieRealizadaCasoDeUso.Ejecutar(serieRealizada);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaSerieRealizada.Id }, nuevaSerieRealizada);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarSerieRealizadaDTO serieRealizada)
        {
            if (id != serieRealizada.Id)
            {
                return BadRequest();
            }
            var serieRealizadaActualizada = await _actualizarSerieRealizadaCasoDeUso.Ejecutar(serieRealizada);
            if (serieRealizadaActualizada == null)
            {
                return NotFound();
            }
            return Ok(serieRealizadaActualizada);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Eliminar(long id)
        {
            var eliminado = await _eliminarSerieRealizadaCasoDeUso.Ejecutar(id);
            if (!eliminado)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
