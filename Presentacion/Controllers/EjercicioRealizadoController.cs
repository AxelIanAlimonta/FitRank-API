using FitRank_API.Application.CasosDeUso.EjercicioRealizadoCasosDeUso;
using FitRank_API.Application.DTOs.EjercicioRealizadoDTOs;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EjercicioRealizadoController : ControllerBase
    {
        private readonly ObtenerTodosLosEjercicioRealizadoCasoDeUso _obtenerTodosLosEjercicioRealizadoCasoDeUso;
        private readonly ObtenerEjercicioRealizadoPorIdCasoDeUso _obtenerEjercicioRealizadoPorIdCasoDeUso;
        private readonly AgregarEjercicioRealizadoCasoDeUso _agregarEjercicioRealizadoCasoDeUso;
        private readonly ActualizarEjercicioRealizadoCasoDeUso _actualizarEjercicioRealizadoCasoDeUso;
        private readonly EliminarEjercicioRealizadoCasoDeUso _eliminarEjercicioRealizadoCasoDeUso;

        public EjercicioRealizadoController(
            ObtenerTodosLosEjercicioRealizadoCasoDeUso obtenerTodosLosEjercicioRealizadoCasoDeUso,
            ObtenerEjercicioRealizadoPorIdCasoDeUso obtenerEjercicioRealizadoPorIdCasoDeUso,
            AgregarEjercicioRealizadoCasoDeUso agregarEjercicioRealizadoCasoDeUso,
            ActualizarEjercicioRealizadoCasoDeUso actualizarEjercicioRealizadoCasoDeUso,
            EliminarEjercicioRealizadoCasoDeUso eliminarEjercicioRealizadoCasoDeUso)
        {
            _obtenerTodosLosEjercicioRealizadoCasoDeUso = obtenerTodosLosEjercicioRealizadoCasoDeUso;
            _obtenerEjercicioRealizadoPorIdCasoDeUso = obtenerEjercicioRealizadoPorIdCasoDeUso;
            _agregarEjercicioRealizadoCasoDeUso = agregarEjercicioRealizadoCasoDeUso;
            _actualizarEjercicioRealizadoCasoDeUso = actualizarEjercicioRealizadoCasoDeUso;
            _eliminarEjercicioRealizadoCasoDeUso = eliminarEjercicioRealizadoCasoDeUso;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var ejerciciosRealizados = await _obtenerTodosLosEjercicioRealizadoCasoDeUso.Ejecutar();
            return Ok(ejerciciosRealizados);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> ObtenerPorId(long id)
        {
            var ejercicioRealizado = await _obtenerEjercicioRealizadoPorIdCasoDeUso.Ejecutar(id);
            if (ejercicioRealizado == null)
            {
                return NotFound();
            }
            return Ok(ejercicioRealizado);
        }
        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] AgregarEjercicioRealizadoDTO ejercicioRealizado)
        {
            var nuevoEjercicioRealizado = await _agregarEjercicioRealizadoCasoDeUso.Ejecutar(ejercicioRealizado);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoEjercicioRealizado.Id }, nuevoEjercicioRealizado);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarEjercicioRealizadoDTO ejercicioRealizado)
        {
            if (id != ejercicioRealizado.Id)
            {
                return BadRequest();
            }
            var ejercicioRealizadoActualizado = await _actualizarEjercicioRealizadoCasoDeUso.Ejecutar(ejercicioRealizado);
            if (ejercicioRealizadoActualizado == null)
            {
                return NotFound();
            }
            return Ok(ejercicioRealizadoActualizado);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Eliminar(long id)
        {
            var eliminado = await _eliminarEjercicioRealizadoCasoDeUso.Ejecutar(id);
            if (!eliminado)
            {
                return NotFound();
            }
            return NoContent();
        }

    }
}
