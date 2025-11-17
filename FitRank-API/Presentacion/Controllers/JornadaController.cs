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
            try
            {
                var jornadas = await _obtenerTodasLasJornadaCasoDeUso.Ejecutar();
                return Ok(jornadas);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener las jornadas");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(long id)
        {
            var jornada = await _obtenerJornadaPorIdCasoDeUso.Ejecutar(id);
            if (jornada == null)
            {
                return NotFound($"La jornada con ID {id} no fue encontrada.");
            }
            return Ok(jornada);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] AgregarJornadaDTO agregarJornadaDTO)
        {
            if (agregarJornadaDTO == null)
            {
                return BadRequest("El objeto JornadaDTO no puede ser nulo.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var nuevaJornada = await _agregarJornadaCasoDeUso.Ejecutar(agregarJornadaDTO);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaJornada.Id }, nuevaJornada);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al agregar la jornada");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarJornadaDTO actualizarJornadaDTO)
        {
            if (actualizarJornadaDTO == null)
            {
                return BadRequest("El objeto ActualizarJornadaDTO no puede ser nulo.");
            }
            if (id != actualizarJornadaDTO.Id)
            {
                return BadRequest("El ID de la jornada no coincide con el ID proporcionado en la ruta.");
            }
            try
            {
                var jornadaActualizada = await _actualizarJornadaCasoDeUso.Ejecutar(actualizarJornadaDTO);
                if (jornadaActualizada == null)
                {
                    return NotFound($"La jornada con ID {id} no fue encontrada para actualizar.");
                }
                return Ok(jornadaActualizada);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al actualizar la jornada");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(long id)
        {
            try{var eliminado = await _eliminarJornadaCasoDeUso.Ejecutar(id);
            if (!eliminado)
            {
                return NotFound($"La jornada con ID {id} no fue encontrada para eliminar.");
            }
            return NoContent();}catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al eliminar la jornada");
            }
        }


    }
}
