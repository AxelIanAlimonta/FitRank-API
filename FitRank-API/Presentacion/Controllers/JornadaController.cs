using FitRank_API.Application.CasosDeUso.JornadaCasosDeUso;
using FitRank_API.Application.DTOs.JornadaDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JornadaController : ControllerBase
    {
        private readonly ObtenerTodasLasJornadasCasoDeUso _obtenerTodasLasJornadaCasoDeUso;
        private readonly ObtenerJornadaPorIdCasoDeUso _obtenerJornadaPorIdCasoDeUso;
        private readonly AgregarJornadaCasoDeUso _agregarJornadaCasoDeUso;
        private readonly ActualizarJornadaCasoDeUso _actualizarJornadaCasoDeUso;
        private readonly EliminarJornadaCasoDeUso _eliminarJornadaCasoDeUso;

        public JornadaController(
            ObtenerTodasLasJornadasCasoDeUso obtenerTodasLasJornadaCasoDeUso,
            ObtenerJornadaPorIdCasoDeUso obtenerJornadaPorIdCasoDeUso,
            AgregarJornadaCasoDeUso agregarJornadaCasoDeUso,
            ActualizarJornadaCasoDeUso actualizarJornadaCasoDeUso,
            EliminarJornadaCasoDeUso eliminarJornadaCasoDeUso)
        {
            _obtenerTodasLasJornadaCasoDeUso = obtenerTodasLasJornadaCasoDeUso;
            _obtenerJornadaPorIdCasoDeUso = obtenerJornadaPorIdCasoDeUso;
            _agregarJornadaCasoDeUso = agregarJornadaCasoDeUso;
            _actualizarJornadaCasoDeUso = actualizarJornadaCasoDeUso;
            _eliminarJornadaCasoDeUso = eliminarJornadaCasoDeUso;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodasAsync()
        {
            var jornadas = await _obtenerTodasLasJornadaCasoDeUso.Ejecutar();
            return Ok(jornadas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(long id)
        {
            var jornada = await _obtenerJornadaPorIdCasoDeUso.Ejecutar(id);
            if (jornada == null)
            {
                return NotFound();
            }
            return Ok(jornada);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] AgregarJornadaDTO agregarJornadaDTO)
        {
            var nuevaJornada = await _agregarJornadaCasoDeUso.Ejecutar(agregarJornadaDTO);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaJornada.Id }, nuevaJornada);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarJornadaDTO actualizarJornadaDTO)
        {
            if (id != actualizarJornadaDTO.Id)
            {
                return BadRequest();
            }
            var jornadaActualizada = await _actualizarJornadaCasoDeUso.Ejecutar(actualizarJornadaDTO);
            if (jornadaActualizada == null)
            {
                return NotFound();
            }
            return Ok(jornadaActualizada);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(long id)
        {
            var eliminado = await _eliminarJornadaCasoDeUso.Ejecutar(id);
            if (!eliminado)
            {
                return NotFound();
            }
            return NoContent();
        }


    }
}
