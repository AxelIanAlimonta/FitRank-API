using FitRank_API.Application.CasosDeUso.DificultadCasosDeUso;
using FitRank_API.Application.DTOs.DificultadDTOs;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DificultadController : ControllerBase
    {
        private readonly ObtenerTodasLasDificultadesCasoDeUso _obtenerTodasLasDificultadesCasoDeUso;
        private readonly ObtenerDificultadPorIdCasoDeUso _obtenerDificultadPorIdCasoDeUso;
        private readonly AgregarDificultadCasoDeUso _agregarDificultadCasoDeUso;
        private readonly ActualizarDificultadCasoDeUso _actualizarDificultadCasoDeUso;
        private readonly EliminarDificultadCasoDeUso _eliminarDificultadCasoDeUso;

        public DificultadController(
            ObtenerTodasLasDificultadesCasoDeUso obtenerTodasLasDificultadesCasoDeUso,
            ObtenerDificultadPorIdCasoDeUso obtenerDificultadPorIdCasoDeUso,
            AgregarDificultadCasoDeUso agregarDificultadCasoDeUso,
            ActualizarDificultadCasoDeUso actualizarDificultadCasoDeUso,
            EliminarDificultadCasoDeUso eliminarDificultadCasoDeUso)
        {
            _obtenerTodasLasDificultadesCasoDeUso = obtenerTodasLasDificultadesCasoDeUso;
            _obtenerDificultadPorIdCasoDeUso = obtenerDificultadPorIdCasoDeUso;
            _agregarDificultadCasoDeUso = agregarDificultadCasoDeUso;
            _actualizarDificultadCasoDeUso = actualizarDificultadCasoDeUso;
            _eliminarDificultadCasoDeUso = eliminarDificultadCasoDeUso;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            try
            {
                var dificultades = await _obtenerTodasLasDificultadesCasoDeUso.Ejecutar();
                return Ok(dificultades);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            try
            {
                var dificultad = await _obtenerDificultadPorIdCasoDeUso.Ejecutar(id);
                if (dificultad == null)
                {
                    return NotFound(new { Mensaje = "Dificultad no encontrada." });
                }
                return Ok(dificultad);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] AgregarDificultadDTO dificultad)
        {
            if (dificultad == null)
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var nuevaDificultad = await _agregarDificultadCasoDeUso.Ejecutar(dificultad);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaDificultad.Id }, nuevaDificultad);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] DificultadDTO dificultad)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            if (dificultad == null)
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != dificultad.Id)
            {
                return BadRequest(new { Mensaje = "El ID de la URL no coincide con el ID de la dificultad." });
            }

            try
            {
                var dificultadActualizada = await _actualizarDificultadCasoDeUso.Ejecutar(dificultad);
                if (dificultadActualizada == null)
                {
                    return NotFound(new { Mensaje = "Dificultad no encontrada." });
                }
                return Ok(dificultadActualizada);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            try
            {
                await _eliminarDificultadCasoDeUso.Ejecutar(id);
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }
    }
}
