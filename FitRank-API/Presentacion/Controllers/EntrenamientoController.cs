using FitRank_API.Application.CasosDeUso.EntrenamientoCasosDeUso;
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
        private readonly ObtenerHistorialEntrenamientosDeUnUsuarioCasoDeUso _obtenerHostialEntrenamientosCasoDeUso;

        public EntrenamientoController(
            AgregarEntrenamientoCasoDeUso crear,
            ObtenerEntrenamientosCasoDeUso obtenerTodos,
            ObtenerEntrenamientoPorIdCasoDeUso obtenerPorId,
            ActualizarEntrenamientoCasoDeUso actualizar,
            EliminarEntrenamientoCasoDeUso eliminar,
            ObtenerHistorialEntrenamientosDeUnUsuarioCasoDeUso obtenerHistorialEntrenamientoDeUnUsuarioCasoDeUso
            )
        {
            _crear = crear;
            _obtenerTodos = obtenerTodos;
            _obtenerPorId = obtenerPorId;
            _actualizar = actualizar;
            _eliminar = eliminar;
            _obtenerHostialEntrenamientosCasoDeUso = obtenerHistorialEntrenamientoDeUnUsuarioCasoDeUso;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var entrenamientos = await _obtenerTodos.Ejecutar();
                return Ok(entrenamientos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var ent = await _obtenerPorId.Ejecutar(id);
                return ent == null ? NotFound($"No se encontró ningún entrenamiento con ID {id}.") : Ok(ent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] AgregarEntrenamientoDTO dto)
        {
            if (dto == null) return BadRequest("El cuerpo de la solicitud no puede ser nulo.");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var nuevo = await _crear.Ejecutar(dto);
                return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarEntrenamientoDTO dto)
        {
            if (dto == null) return BadRequest("El cuerpo de la solicitud no puede ser nulo.");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != dto.Id) return BadRequest("El ID en la URL no coincide con el ID en el cuerpo de la solicitud.");
            try
            {
                var resultado = await _actualizar.Ejecutar(dto);
                if (resultado == null) return NotFound($"No se encontró ningún entrenamiento con ID {id}.");
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(long id)
        {
            try
            {
                var resultado = await _eliminar.Ejecutar(id);
                if (!resultado) return NotFound($"No se encontró ningún entrenamiento con ID {id}.");
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpGet("socio/{socioId}/historial")]
        public async Task<ActionResult<List<EntrenamientoHistorialDTO>>> ObtenerHistorial(long socioId)
        {
            var result = await _obtenerHostialEntrenamientosCasoDeUso.EjecutarAsync(socioId);
            return Ok(result);
        }

    }
}
