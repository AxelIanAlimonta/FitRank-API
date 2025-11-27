using FitRank_API.Application.CasosDeUso.JornadaCasosDeUso;
using FitRank_API.Application.DTOs.JornadaDTOs;
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
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(long id)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            try
            {
                var jornada = await _obtenerJornadaPorIdCasoDeUso.Ejecutar(id);
                if (jornada == null)
                {
                    return NotFound(new { Mensaje = "Jornada no encontrada." });
                }
                return Ok(jornada);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] AgregarJornadaDTO agregarJornadaDTO)
        {
            if (agregarJornadaDTO == null)
            {
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });
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
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarJornadaDTO actualizarJornadaDTO)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            if (actualizarJornadaDTO == null)
            {
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });
            }

            if (id != actualizarJornadaDTO.Id)
            {
                return BadRequest(new { Mensaje = "El ID de la URL no coincide con el ID de la jornada." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var jornadaActualizada = await _actualizarJornadaCasoDeUso.Ejecutar(actualizarJornadaDTO);
                if (jornadaActualizada == null)
                {
                    return NotFound(new { Mensaje = "Jornada no encontrada." });
                }
                return Ok(jornadaActualizada);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(long id)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            try
            {
                var eliminado = await _eliminarJornadaCasoDeUso.Ejecutar(id);
                if (!eliminado)
                {
                    return NotFound(new { Mensaje = "Jornada no encontrada." });
                }
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }
    }
}
