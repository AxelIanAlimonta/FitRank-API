using FitRank_API.Application.CasosDeUso.DificultadCasosDeUso;
using FitRank_API.Application.CasosDeUso.GrupoMuscularCasosDeUso;
using FitRank_API.Application.DTOs.DificultadDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DificultadController : ControllerBase
    {
        private readonly ObtenerTodasLasDificultadesCasoDeUso obtenerTodasLasDificultadesCasoDeUso;
        private readonly ObtenerDificultadPorIdCasoDeUso obtenerDificultadPorIdCasoDeUso;
        private readonly AgregarDificultadCasoDeUso agregarDificultadCasoDeUso;
        private readonly ActualizarDificultadCasoDeUso actualizarDificultadCasoDeUso;
        private readonly EliminarDificultadCasoDeUso eliminarDificultadCasoDeUso;

        public DificultadController(ObtenerTodasLasDificultadesCasoDeUso obtenerTodasLasDificultadesCasoDeUso,
            ObtenerDificultadPorIdCasoDeUso obtenerDificultadPorIdCasoDeUso,
            AgregarDificultadCasoDeUso agregarDificultadCasoDeUso,
            ActualizarDificultadCasoDeUso actualizarDificultadCasoDeUso,
            EliminarDificultadCasoDeUso eliminarDificultadCasoDeUso)
        {
            this.obtenerTodasLasDificultadesCasoDeUso = obtenerTodasLasDificultadesCasoDeUso;
            this.obtenerDificultadPorIdCasoDeUso = obtenerDificultadPorIdCasoDeUso;
            this.agregarDificultadCasoDeUso = agregarDificultadCasoDeUso;
            this.actualizarDificultadCasoDeUso = actualizarDificultadCasoDeUso;
            this.eliminarDificultadCasoDeUso = eliminarDificultadCasoDeUso;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var dificultades = await obtenerTodasLasDificultadesCasoDeUso.Ejecutar();
            return Ok(dificultades);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var dificultad = await obtenerDificultadPorIdCasoDeUso.Ejecutar(id);
            if (dificultad == null)
            {
                return NotFound();
            }
            return Ok(dificultad);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] AgregarDificultadDTO dificultad)
        {
            var nuevaDificultad = await agregarDificultadCasoDeUso.Ejecutar(dificultad);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaDificultad.Id }, nuevaDificultad);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] DificultadDTO dificultad)
        {
            if (id != dificultad.Id)
            {
                return BadRequest("El ID del grupo muscular no coincide.");
            }
            var dificultadActualizada = await actualizarDificultadCasoDeUso.Ejecutar(dificultad);
            if (dificultadActualizada == null)
            {
                return NotFound();
            }
            return Ok(dificultadActualizada);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            await eliminarDificultadCasoDeUso.Ejecutar(id);
            return NoContent();
        }

    }
}
