using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.SerieDTOs;
using FitRank_API.Application.CasosDeUso.SerieCasosDeUso;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SerieController : ControllerBase
    {
        private readonly AgregarSerieCasoDeUso _crear;
        private readonly ObtenerSeriesCasoDeUso _obtenerTodas;
        private readonly ObtenerSeriePorIdCasoDeUso _obtenerPorId;
        private readonly ActualizarSerieCasoDeUso _actualizar;
        private readonly EliminarSerieCasoDeUso _eliminar;

        public SerieController(
            AgregarSerieCasoDeUso crear,
            ObtenerSeriesCasoDeUso obtenerTodas,
            ObtenerSeriePorIdCasoDeUso obtenerPorId,
            ActualizarSerieCasoDeUso actualizar,
            EliminarSerieCasoDeUso eliminar)
        {
            _crear = crear;
            _obtenerTodas = obtenerTodas;
            _obtenerPorId = obtenerPorId;
            _actualizar = actualizar;
            _eliminar = eliminar;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodaslasSeries()
        {
            try
            {
                var lista = await _obtenerTodas.Ejecutar();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error en el servidor.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerSeriePorId(long id)
        {
            var serie = await _obtenerPorId.Ejecutar(id);
            if (serie == null) return NotFound($"La serie con ID {id} no existe.");
            return Ok(serie);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] AgregarSerieDTO dto)
        {
            if (dto == null) return BadRequest("El objeto no puede ser nulo.");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var nueva = await _crear.Ejecutar(dto);
                return CreatedAtAction(nameof(ObtenerSeriePorId), new { id = nueva.Id }, nueva);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error en el servidor.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarSerieDTO dto)
        {
            if (dto == null) return BadRequest("El objeto no puede ser nulo.");
            if (id != dto.Id) return BadRequest("El ID en la URL no coincide con el ID en el cuerpo.");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var actualizada = await _actualizar.Ejecutar(dto);
                if (actualizada == null) return NotFound($"La serie con ID {id} no existe.");
                return Ok(actualizada);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error en el servidor.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(long id)
        {
            try
            {
                var resultado = await _eliminar.Ejecutar(id);
                if (!resultado) return NotFound($"La serie con ID {id} no existe.");
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error en el servidor.");
            }
        }
    }
}
