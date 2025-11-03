using FitRank_API.Application.CasosDeUso.GimnasioCasosDeUso;
using FitRank_API.Application.DTOs.GimnasioDTOs;

using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GimnasioController : ControllerBase
    {
        private readonly ObtenerGimnasiosCasoDeUso _obtenerGimnasiosCasoDeUso;
        private readonly AgregarGimnasioCasoDeUso _agregarGimnasioCasoDeUso;
        private readonly ActualizarGimnasioCasoDeUso _actualizarGimnasioCasoDeUso;
        private readonly EliminarGimnasioCasoDeUso _eliminarGimnasioCasoDeUso;
        private readonly ObtenerGimnasioPorIdCasoDeUso _obtenerGimnasioPorIdCasoDeUso;
        public GimnasioController(ObtenerGimnasiosCasoDeUso obtenerGimnasiosCasoDeUso,
            AgregarGimnasioCasoDeUso agregarGimnasioCasoDeUso,
            ActualizarGimnasioCasoDeUso actualizarGimnasioCasoDeUso,
            EliminarGimnasioCasoDeUso eliminarGimnasioCasoDeUso,
            ObtenerGimnasioPorIdCasoDeUso obtenerGimnasioPorIdCasoDeUso)
        {
            _obtenerGimnasiosCasoDeUso = obtenerGimnasiosCasoDeUso;
            _agregarGimnasioCasoDeUso = agregarGimnasioCasoDeUso;
            _actualizarGimnasioCasoDeUso = actualizarGimnasioCasoDeUso;
            _eliminarGimnasioCasoDeUso = eliminarGimnasioCasoDeUso;
            _obtenerGimnasioPorIdCasoDeUso = obtenerGimnasioPorIdCasoDeUso;
        }

        [HttpGet]
        public async Task<ActionResult> ObtenerTodos()
        {
            try
            {
                var gimnasios = await _obtenerGimnasiosCasoDeUso.Ejecutar();
                return Ok(gimnasios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error en el servidor.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> ObtenerPorId(long id)
        {
            var gimnasio = await _obtenerGimnasioPorIdCasoDeUso.Ejecutar(id);
            if (gimnasio == null)
            {
                return NotFound();
            }
            return Ok(gimnasio);
        }

        [HttpPost]
        public async Task<ActionResult> Agregar([FromBody] AgregarGimnasioDTO crearGimnasioDTO)
        {
            if (crearGimnasioDTO == null)
            {
                return BadRequest("El gimnasio no puede ser nulo.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var gimnasioCreado = await _agregarGimnasioCasoDeUso.Ejecutar(crearGimnasioDTO);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = gimnasioCreado.Id }, gimnasioCreado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error en el servidor.");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Actualizar(long id, [FromBody] ActualizarGimnasioDTO actualizarGimnasioDTO)
        {
            if (actualizarGimnasioDTO == null)
            {
                return BadRequest("El gimnasio no puede ser nulo.");
            }
            if (id != actualizarGimnasioDTO.Id)
            {
                return BadRequest("El ID del gimnasio no coincide.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var gimnasioActualizado = await _actualizarGimnasioCasoDeUso.Ejecutar(actualizarGimnasioDTO);
                if (gimnasioActualizado == null)
                {
                    return NotFound("Gimnasio no encontrado.");
                }
                return Ok(gimnasioActualizado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error en el servidor.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(long id)
        {
            try
            {
                var eliminado = await _eliminarGimnasioCasoDeUso.Ejecutar(id);
                if (!eliminado)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error en el servidor.");
            }
        }
    }
}