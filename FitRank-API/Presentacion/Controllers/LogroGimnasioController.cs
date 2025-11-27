using FitRank_API.Application.CasosDeUso.LogroGimnasioCasosDeUso;
using FitRank_API.Application.DTOs.LogroGimnasioDTOs;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [ApiController]
    [Route("api/gimnasios/{gimnasioId:int}/[controller]")]
    public class LogrosGimnasioController : ControllerBase
    {
        private readonly ObtenerLogrosGimnasioCasoDeUso _obtenerLogrosGimnasioCasoDeUso;
        private readonly ActualizarLogroGimnasioCasoDeUso _actualizarLogroGimnasioCasoDeUso;

        public LogrosGimnasioController(
            ObtenerLogrosGimnasioCasoDeUso obtenerLogrosGimnasioCasoDeUso,
            ActualizarLogroGimnasioCasoDeUso actualizarLogroGimnasioCasoDeUso)
        {
            _obtenerLogrosGimnasioCasoDeUso = obtenerLogrosGimnasioCasoDeUso;
            _actualizarLogroGimnasioCasoDeUso = actualizarLogroGimnasioCasoDeUso;
        }

        // GET: api/gimnasios/10/logrosgimnasio
        // Devuelve todos los logros globales + estado de configuración para ese gimnasio
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LogroGimnasioDTO>>> ObtenerLogrosGimnasio(int gimnasioId)
        {
            if (gimnasioId <= 0)
                return BadRequest(new { Mensaje = "El ID del gimnasio debe ser mayor a cero." });

            try
            {
                var logros = await _obtenerLogrosGimnasioCasoDeUso.Ejecutar(gimnasioId);
                return Ok(logros);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        // PUT: api/gimnasios/10/logrosgimnasio/5
        // Actualiza la config de un logro para ese gimnasio (ej: EstaHabilitado)
        [HttpPut("{logroId:int}")]
        public async Task<ActionResult<LogroGimnasioDTO>> ActualizarLogrosGimnasio(
            int gimnasioId, 
            int logroId, 
            [FromBody] ActualizarLogroGimnasioDTO dto)
        {
            if (gimnasioId <= 0)
                return BadRequest(new { Mensaje = "El ID del gimnasio debe ser mayor a cero." });

            if (logroId <= 0)
                return BadRequest(new { Mensaje = "El ID del logro debe ser mayor a cero." });

            if (dto == null)
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            dto.GimnasioId = gimnasioId;
            dto.LogroId = logroId;

            try
            {
                var actualizado = await _actualizarLogroGimnasioCasoDeUso.Ejecutar(dto);
                if (actualizado is null)
                    return NotFound(new { Mensaje = "Logro no encontrado." });

                return Ok(actualizado);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }
    }
}
