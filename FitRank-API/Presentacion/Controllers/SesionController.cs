using FitRank_API.Application.CasosDeUso.SesionCasosDeUso;
using Microsoft.AspNetCore.Mvc;
using FitRank_API.Application.DTOs.SesionDTOs;

namespace FitRank_API.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SesionController : ControllerBase
    {
        private readonly AgregarSesionCasoDeUso _agregarSesionCasoDeUso;
        private readonly ActualizarSesionCasoDeUso _actualizarSesionCasoDeUso;
        private readonly EliminarSesionCasoDeUso _eliminarSesionCasoDeUso;
        private readonly ObtenerSesionPorIdCasoDeUso _obtenerSesionCasoDeUso;
        private readonly ObtenerTodasLasSesionesCasoDeUso _obtenerTodasLasSesionCasoDeUso;

        public SesionController(
            AgregarSesionCasoDeUso agregarSesionCasoDeUso,
            ActualizarSesionCasoDeUso actualizarSesionCasoDeUso,
            EliminarSesionCasoDeUso eliminarSesionCasoDeUso,
            ObtenerSesionPorIdCasoDeUso obtenerSesionCasoDeUso,
            ObtenerTodasLasSesionesCasoDeUso obtenerTodasLasSesionesCasoDeUso)
        {
            _agregarSesionCasoDeUso = agregarSesionCasoDeUso;
            _actualizarSesionCasoDeUso = actualizarSesionCasoDeUso;
            _eliminarSesionCasoDeUso = eliminarSesionCasoDeUso;
            _obtenerSesionCasoDeUso = obtenerSesionCasoDeUso;
            _obtenerTodasLasSesionCasoDeUso = obtenerTodasLasSesionesCasoDeUso;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            try
            {
                var sesiones = await _obtenerTodasLasSesionCasoDeUso.Ejecutar();
                return Ok(sesiones);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(long id)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID de la sesión debe ser mayor a cero." });

            try
            {
                var sesion = await _obtenerSesionCasoDeUso.Ejecutar(id);
                if (sesion == null)
                    return NotFound(new { Mensaje = $"La sesión con ID {id} no fue encontrada." });

                return Ok(sesion);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] AgregarSesionDTO dto)
        {
            if (dto == null)
                return BadRequest(new { Mensaje = "El objeto sesión no puede ser nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var nueva = await _agregarSesionCasoDeUso.Ejecutar(dto);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = nueva.Id }, nueva);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarSesionDTO dto)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID de la sesión debe ser mayor a cero." });

            if (dto == null)
                return BadRequest(new { Mensaje = "El objeto sesión no puede ser nulo." });

            if (id != dto.Id)
                return BadRequest(new { Mensaje = "El ID de la sesión no coincide con el ID del objeto." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var actualizada = await _actualizarSesionCasoDeUso.Ejecutar(dto);
                if (actualizada == null)
                    return NotFound(new { Mensaje = $"La sesión con ID {id} no fue encontrada." });

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
                return BadRequest(new { Mensaje = "El ID de la sesión debe ser mayor a cero." });

            try
            {
                var eliminado = await _eliminarSesionCasoDeUso.Ejecutar(id);
                if (!eliminado)
                    return NotFound(new { Mensaje = $"La sesión con ID {id} no fue encontrada." });

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }
    }
}
