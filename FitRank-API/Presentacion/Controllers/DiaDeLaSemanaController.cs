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
            try
            {
                var result = await _obtenerTodosLosDiasDeLaSemanaCasoDeUso.Ejecutar();
                return Ok(result);
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
                var DiaObtenido = await _obtenerDiaDeLaSemanaPorIdCasoDeUso.Ejecutar(id);
                if (DiaObtenido == null)
                {
                    return NotFound(new { Mensaje = "Día de la semana no encontrado." });
                }
                return Ok(DiaObtenido);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AgregarAsync([FromBody] AgregarDiaDeLaSemanaDTO diaDeLaSemanaDTO)
        {
            if (diaDeLaSemanaDTO == null)
            {
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var nuevoDia = await _agregarDiaDeLaSemanaCasoDeUso.Ejecutar(diaDeLaSemanaDTO);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoDia.Id }, nuevoDia);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarAsync(long id, [FromBody] ActualizarDiaDeLaSemanaDTO diaDeLaSemanaDTO)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            if (diaDeLaSemanaDTO == null)
            {
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != diaDeLaSemanaDTO.Id)
            {
                return BadRequest(new { Mensaje = "El ID de la URL no coincide con el ID del objeto." });
            }

            try
            {
                var diaActualizado = await _actualizarDiaDeLaSemanaCasoDeUso.Ejecutar(diaDeLaSemanaDTO);
                if (diaActualizado == null)
                {
                    return NotFound(new { Mensaje = "Día de la semana no encontrado." });
                }
                return Ok(diaActualizado);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarAsync(long id)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            try
            {
                var eliminado = await _eliminarDiaDeLaSemanaCasoDeUso.Ejecutar(id);
                if (!eliminado)
                {
                    return NotFound(new { Mensaje = "Día de la semana no encontrado." });
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
