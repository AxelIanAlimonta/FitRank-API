using FitRank_API.Application.CasosDeUso.EntrenamientoCasosDeUso;
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
        private readonly ObtenerHistorialEntrenamientosDeProfesorCasoDeUso _obtenerHistorialProfesorCasoDeUso;

        public EntrenamientoController(
            AgregarEntrenamientoCasoDeUso crear,
            ObtenerEntrenamientosCasoDeUso obtenerTodos,
            ObtenerEntrenamientoPorIdCasoDeUso obtenerPorId,
            ActualizarEntrenamientoCasoDeUso actualizar,
            EliminarEntrenamientoCasoDeUso eliminar,
            ObtenerHistorialEntrenamientosDeUnUsuarioCasoDeUso obtenerHistorialEntrenamientoDeUnUsuarioCasoDeUso,
            ObtenerHistorialEntrenamientosDeProfesorCasoDeUso obtenerHistorialProfesorCasoDeUso
            )
        {
            _crear = crear;
            _obtenerTodos = obtenerTodos;
            _obtenerPorId = obtenerPorId;
            _actualizar = actualizar;
            _eliminar = eliminar;
            _obtenerHostialEntrenamientosCasoDeUso = obtenerHistorialEntrenamientoDeUnUsuarioCasoDeUso;
            _obtenerHistorialProfesorCasoDeUso = obtenerHistorialProfesorCasoDeUso;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var entrenamientos = await _obtenerTodos.Ejecutar();
                return Ok(entrenamientos);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            try
            {
                var ent = await _obtenerPorId.Ejecutar(id);
                return ent == null ? NotFound(new { Mensaje = "Entrenamiento no encontrado." }) : Ok(ent);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] AgregarEntrenamientoDTO dto)
        {
            if (dto == null)
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var nuevo = await _crear.Ejecutar(dto);
                return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarEntrenamientoDTO dto)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            if (dto == null)
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != dto.Id)
                return BadRequest(new { Mensaje = "El ID de la URL no coincide con el ID del entrenamiento." });

            try
            {
                var resultado = await _actualizar.Ejecutar(dto);
                if (resultado == null)
                    return NotFound(new { Mensaje = "Entrenamiento no encontrado." });

                return Ok(resultado);
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
                var resultado = await _eliminar.Ejecutar(id);
                if (!resultado)
                    return NotFound(new { Mensaje = "Entrenamiento no encontrado." });

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("socio/{socioId}/historial")]
        public async Task<ActionResult<List<EntrenamientoHistorialDTO>>> ObtenerHistorial(long socioId)
        {
            if (socioId <= 0)
                return BadRequest(new { Mensaje = "El ID del socio debe ser mayor a cero." });

            try
            {
                var result = await _obtenerHostialEntrenamientosCasoDeUso.EjecutarAsync(socioId);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("profesor/{profesorId}/historial")]
        public async Task<ActionResult<List<EntrenamientoHistorialDTO>>> ObtenerHistorialProfesor(
            long profesorId,
            [FromQuery] string? nombre = null)
        {
            if (profesorId <= 0)
                return BadRequest(new { Mensaje = "El ID del profesor debe ser mayor a cero." });

            try
            {
                var result = await _obtenerHistorialProfesorCasoDeUso.EjecutarAsync(profesorId, nombre);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }
    }
}
