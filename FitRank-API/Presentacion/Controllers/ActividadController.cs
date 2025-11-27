using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.ActividadDTOs;
using FitRank_API.Application.UseCases;
using FitRank_API.Application.UseCases.Actividad;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActividadController : ControllerBase
    {
        private readonly AgregarActividadCasoDeUso _crear;
        private readonly ObtenerActividadesCasoDeUso _obtenerTodas;
        private readonly ObtenerActividadPorIdCasoDeUso _obtenerPorId;
        private readonly ActualizarActividadCasoDeUso _actualizar;
        private readonly EliminarActividadCasoDeUso _eliminar;
        private readonly RegistrarActividadCasoDeUso _registrar;

        public ActividadController(
            AgregarActividadCasoDeUso crear,
            ObtenerActividadesCasoDeUso obtenerTodas,
            ObtenerActividadPorIdCasoDeUso obtenerPorId,
            ActualizarActividadCasoDeUso actualizar,
            EliminarActividadCasoDeUso eliminar,
            RegistrarActividadCasoDeUso registrar)
        {
            _crear = crear;
            _obtenerTodas = obtenerTodas;
            _obtenerPorId = obtenerPorId;
            _actualizar = actualizar;
            _eliminar = eliminar;
            _registrar = registrar;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodasLasActividades()
        {
            try
            {
                var actividades = await _obtenerTodas.Ejecutar();
                return Ok(actividades);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerActividadPorId(long id)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            try
            {
                var act = await _obtenerPorId.Ejecutar(id);
                return act == null 
                    ? NotFound(new { Mensaje = $"La actividad con ID {id} no existe." }) 
                    : Ok(act);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] AgregarActividadDTO dto)
        {
            if (dto == null) 
                return BadRequest(new { Mensaje = "El objeto Actividad es nulo." });

            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            try
            {
                var nueva = await _crear.Ejecutar(dto);
                return CreatedAtAction(nameof(ObtenerActividadPorId), new { id = nueva.Id }, nueva);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarActividadDTO dto)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            if (dto == null) 
                return BadRequest(new { Mensaje = "El objeto Actividad es nulo." });

            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            if (id != dto.Id) 
                return BadRequest(new { Mensaje = "El ID de la ruta no coincide con el ID del objeto." });

            try
            {
                var actividadActualizada = await _actualizar.Ejecutar(dto);
                if (actividadActualizada == null)
                {
                    return NotFound(new { Mensaje = $"La actividad con ID {id} no existe." });
                }
                return Ok(actividadActualizada);
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
                {
                    return NotFound(new { Mensaje = $"La actividad con ID {id} no existe." });
                }
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarActividad([FromBody] RegistrarActividadDTO dto)
        {
            if (dto == null)
                return BadRequest(new { Mensaje = "El objeto RegistrarActividad es nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var actividad = await _registrar.Ejecutar(dto);
                return Ok(new
                {
                    actividad.Id,   
                    actividad.SerieId,
                    actividad.EntrenamientoId,
                    actividad.Repeticiones,
                    actividad.Peso,
                    actividad.EjercicioAsignadoId,
                    actividad.Punto,
                    actividad.Duracion
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
        }
    }
}
