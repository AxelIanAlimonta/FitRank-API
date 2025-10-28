using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.EntrenamientoDTOs;
using FitRank_API.Application.UseCases.Entrenamiento;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntrenamientoController : ControllerBase
    {
        private readonly AgregarEntrenamientoCasoDeUso _crear;
        private readonly ObtenerEntrenamientosCasoDeUso _obtenerTodos;
        private readonly ObtenerEntrenamientoPorIdCasoDeUso _obtenerPorId;
        private readonly ActualizarEntrenamientoCasoDeUso _actualizar;
        private readonly EliminarEntrenamientoCasoDeUso _eliminar;

        public EntrenamientoController(
            AgregarEntrenamientoCasoDeUso crear,
            ObtenerEntrenamientosCasoDeUso obtenerTodos,
            ObtenerEntrenamientoPorIdCasoDeUso obtenerPorId,
            ActualizarEntrenamientoCasoDeUso actualizar,
            EliminarEntrenamientoCasoDeUso eliminar)
        {
            _crear = crear;
            _obtenerTodos = obtenerTodos;
            _obtenerPorId = obtenerPorId;
            _actualizar = actualizar;
            _eliminar = eliminar;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _obtenerTodos.Ejecutar());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var ent = await _obtenerPorId.Ejecutar(id);
            return ent == null ? NotFound() : Ok(ent);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] AgregarEntrenamientoDTO dto)
        {
            var nuevo = await _crear.Ejecutar(dto);
            return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarEntrenamientoDTO dto)
        {
            if (id != dto.Id) return BadRequest();
            await _actualizar.Ejecutar(dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(long id)
        {
            await _eliminar.Ejecutar(id);
            return NoContent();
        }
    }
}
