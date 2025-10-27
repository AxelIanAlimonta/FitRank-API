using FitRank_API.Application.CasosDeUso.DiaDeLaSemanaCasoDeUso;
using FitRank_API.Application.DTOs.DiaDeLaSemanaDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiaDeLaSemanaController : ControllerBase
    {
        private readonly ObtenerTodosLosDiasDeLaSemanaCasoDeUso _obtenerTodosLosDiasDeLaSemanaCasoDeUso;
        private readonly AgregarDiaDeLaSemanaCasoDeUso _agregarDiaDeLaSemanaCasoDeUso;
        private readonly ActualizarDiaDeLaSemanaCasoDeUso _actualizarDiaDeLaSemanaCasoDeUso;
        private readonly EliminarDiaDeLaSemanaCasoDeUso _eliminarDiaDeLaSemanaCasoDeUso;
        private readonly ObtenerDiaDeLaSemanaPorIdCasoDeUso _obtenerDiaDeLaSemanaPorIdCasoDeUso;

        public DiaDeLaSemanaController(
            ObtenerTodosLosDiasDeLaSemanaCasoDeUso obtenerTodosLosDiasDeLaSemanaCasoDeUso,
            AgregarDiaDeLaSemanaCasoDeUso agregarDiaDeLaSemanaCasoDeUso,
            ActualizarDiaDeLaSemanaCasoDeUso actualizarDiaDeLaSemanaCasoDeUso,
            EliminarDiaDeLaSemanaCasoDeUso eliminarDiaDeLaSemanaCasoDeUso,
            ObtenerDiaDeLaSemanaPorIdCasoDeUso obtenerDiaDeLaSemanaPorIdCasoDeUso)
        {
            _obtenerTodosLosDiasDeLaSemanaCasoDeUso = obtenerTodosLosDiasDeLaSemanaCasoDeUso;
            _agregarDiaDeLaSemanaCasoDeUso = agregarDiaDeLaSemanaCasoDeUso;
            _actualizarDiaDeLaSemanaCasoDeUso = actualizarDiaDeLaSemanaCasoDeUso;
            _eliminarDiaDeLaSemanaCasoDeUso = eliminarDiaDeLaSemanaCasoDeUso;
            _obtenerDiaDeLaSemanaPorIdCasoDeUso = obtenerDiaDeLaSemanaPorIdCasoDeUso;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodosAsync()
        {
            var result = await _obtenerTodosLosDiasDeLaSemanaCasoDeUso.Ejecutar();
            return Ok(result);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(long id)
        {
            var DiaObtenido = await _obtenerDiaDeLaSemanaPorIdCasoDeUso.Ejecutar(id);
            if (DiaObtenido == null)
            {
                return NotFound();
            }
            return Ok(DiaObtenido);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarAsync([FromBody] AgregarDiaDeLaSemanaDTO diaDeLaSemanaDTO)
        {
            var nuevoDia = await _agregarDiaDeLaSemanaCasoDeUso.Ejecutar(diaDeLaSemanaDTO);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoDia.Id }, nuevoDia);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarAsync(long id, [FromBody] ActualizarDiaDeLaSemanaDTO diaDeLaSemanaDTO)
        {
            if (id != diaDeLaSemanaDTO.Id)
            {
                return BadRequest();
            }
            var diaActualizado = await _actualizarDiaDeLaSemanaCasoDeUso.Ejecutar(diaDeLaSemanaDTO);
            if (diaActualizado == null)
            {
                return NotFound();
            }
            return Ok(diaActualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarAsync(long id)
        {
            var eliminado = await _eliminarDiaDeLaSemanaCasoDeUso.Ejecutar(id);
            if (!eliminado)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
