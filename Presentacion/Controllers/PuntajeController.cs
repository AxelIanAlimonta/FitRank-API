using FitRank_API.Application.CasosDeUso.PuntajeCasosDeUso;
using FitRank_API.Application.DTOs.PuntajeDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PuntajeController : ControllerBase
    {
        private readonly ObtenerTodosLosPuntajeCasoDeUso _obtenerTodosLosPuntajeCasoDeUso;
        private readonly ObtenerPuntajePorIdCasoDeUso _obtenerPuntajePorIdCasoDeUso;
        private readonly AgregarPuntajeCasoDeUso _agregarPuntajeCasoDeUso;
        private readonly ActualizarPuntajeCasoDeUso _actualizarPuntajeCasoDeUso;
        private readonly EliminarPuntajeCasoDeUso _eliminarPuntajeCasoDeUso;

        public PuntajeController(
            ObtenerTodosLosPuntajeCasoDeUso obtenerTodosLosPuntajeCasoDeUso,
            ObtenerPuntajePorIdCasoDeUso obtenerPuntajePorIdCasoDeUso,
            AgregarPuntajeCasoDeUso agregarPuntajeCasoDeUso,
            ActualizarPuntajeCasoDeUso actualizarPuntajeCasoDeUso,
            EliminarPuntajeCasoDeUso eliminarPuntajeCasoDeUso)
        {
            _obtenerTodosLosPuntajeCasoDeUso = obtenerTodosLosPuntajeCasoDeUso;
            _obtenerPuntajePorIdCasoDeUso = obtenerPuntajePorIdCasoDeUso;
            _agregarPuntajeCasoDeUso = agregarPuntajeCasoDeUso;
            _actualizarPuntajeCasoDeUso = actualizarPuntajeCasoDeUso;
            _eliminarPuntajeCasoDeUso = eliminarPuntajeCasoDeUso;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var puntajes = await _obtenerTodosLosPuntajeCasoDeUso.Ejecutar();
            return Ok(puntajes);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> ObtenerPorId(long id)
        {
            var puntaje = await _obtenerPuntajePorIdCasoDeUso.Ejecutar(id);
            if (puntaje == null)
            {
                return NotFound();
            }
            return Ok(puntaje);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] AgregarPuntajeDTO puntaje)
        {
            var nuevoPuntaje = await _agregarPuntajeCasoDeUso.Ejecutar(puntaje);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoPuntaje.Id }, nuevoPuntaje);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarPuntajeDTO puntaje)
        {
            if (id != puntaje.Id)
            {
                return BadRequest();
            }
            var puntajeActualizado = await _actualizarPuntajeCasoDeUso.Ejecutar(puntaje);
            if (puntajeActualizado == null)
            {
                return NotFound();
            }
            return Ok(puntajeActualizado);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Eliminar(long id)
        {
            var eliminado = await _eliminarPuntajeCasoDeUso.Ejecutar(id);
            if (!eliminado)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
