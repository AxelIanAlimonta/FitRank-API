using FitRank_API.Application.CasosDeUso.LogroGimnasioCasosDeUso;
using FitRank_API.Application.DTOs.LogroGimnasioDTOs;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Controllers
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
            var logros = await _obtenerLogrosGimnasioCasoDeUso.Ejecutar(gimnasioId);
            return Ok(logros);
        }

        // PUT: api/gimnasios/10/logrosgimnasio/5
        // Actualiza la config de un logro para ese gimnasio (ej: EstaHabilitado)
        [HttpPut("{logroId:int}")]
        public async Task<ActionResult<LogroGimnasioDTO>> ActualizarLogrosGimnasio(int gimnasioId, int logroId, [FromBody] ActualizarLogroGimnasioDTO dto)
        {
            dto.GimnasioId = gimnasioId;
            dto.LogroId = logroId;

            var actualizado = await _actualizarLogroGimnasioCasoDeUso.Ejecutar(dto);
            if (actualizado is null)
                return NotFound();

            return Ok(actualizado);
        }
    }
}
