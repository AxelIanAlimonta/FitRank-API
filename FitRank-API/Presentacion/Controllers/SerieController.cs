using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.SerieDTOs;
using FitRank_API.Application.CasosDeUso.SerieCasosDeUso;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
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
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerSeriePorId(long id)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID de la serie debe ser mayor a cero." });

            try
            {
                var serie = await _obtenerPorId.Ejecutar(id);
                if (serie == null)
                    return NotFound(new { Mensaje = $"La serie con ID {id} no fue encontrada." });

                return Ok(serie);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] AgregarSerieDTO dto)
        {
            if (dto == null)
                return BadRequest(new { Mensaje = "El objeto no puede ser nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var nueva = await _crear.Ejecutar(dto);
                return CreatedAtAction(nameof(ObtenerSeriePorId), new { id = nueva.Id }, nueva);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarSerieDTO dto)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID de la serie debe ser mayor a cero." });

            if (dto == null)
                return BadRequest(new { Mensaje = "El objeto no puede ser nulo." });

            if (id != dto.Id)
                return BadRequest(new { Mensaje = "El ID en la URL no coincide con el ID del objeto." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var actualizada = await _actualizar.Ejecutar(dto);
                if (actualizada == null)
                    return NotFound(new { Mensaje = $"La serie con ID {id} no fue encontrada." });

                return Ok(actualizada);
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
                return BadRequest(new { Mensaje = "El ID de la serie debe ser mayor a cero." });

            try
            {
                var resultado = await _eliminar.Ejecutar(id);
                if (!resultado)
                    return NotFound(new { Mensaje = $"La serie con ID {id} no fue encontrada." });

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }
    }
}
