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
        public async Task<IActionResult> ObtenerTodasLasActividades() => Ok(await _obtenerTodas.Ejecutar());

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerActividadPorId(long id)
        {
            var act = await _obtenerPorId.Ejecutar(id);
            return act == null ? NotFound() : Ok(act);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] AgregarActividadDTO dto)
        {
            var nueva = await _crear.Ejecutar(dto);
            return CreatedAtAction(nameof(ObtenerActividadPorId), new { id = nueva.Id }, nueva);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarActividadDTO dto)
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

        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarActividad([FromBody] RegistrarActividadDTO dto)
        {
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
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
