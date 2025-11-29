using FitRank_API.Application.CasosDeUso.LogroSocioCasosDeUso;
using FitRank_API.Application.DTOs.LogroSocioDTOs;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [ApiController]
    [Route("api/socios/{socioId:int}/gimnasios/{gimnasioId:int}/[controller]")]
    public class LogrosSocioController : ControllerBase
    {
        private readonly ObtenerLogrosSocioCasoDeUso _obtenerLogrosSocioCasoDeUso;
        private readonly ObtenerLogrosDisponiblesPorSocioCasoDeUso _obtenerLogrosDisponiblesSocioCasoDeUso;

        public LogrosSocioController(
            ObtenerLogrosSocioCasoDeUso obtenerLogrosSocioCasoDeUso,
            ObtenerLogrosDisponiblesPorSocioCasoDeUso obtenerLogrosDisponiblesSocioCasoDeUso)
        {
            _obtenerLogrosSocioCasoDeUso = obtenerLogrosSocioCasoDeUso;
            _obtenerLogrosDisponiblesSocioCasoDeUso = obtenerLogrosDisponiblesSocioCasoDeUso;
        }

        // GET: api/socios/1/gimnasios/10/logrossocio
        // Logros ya obtenidos por el socio en ese gimnasio
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LogroSocioDTO>>> ObtenerLogrosObtenidosPorSocio(
            long socioId, 
            long gimnasioId)
        {
            if (socioId <= 0)
                return BadRequest(new { Mensaje = "El ID del socio debe ser mayor a cero." });

            if (gimnasioId <= 0)
                return BadRequest(new { Mensaje = "El ID del gimnasio debe ser mayor a cero." });

            try
            {
                var logros = await _obtenerLogrosSocioCasoDeUso.Ejecutar(socioId, gimnasioId);
                return Ok(logros);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        // GET: api/socios/1/gimnasios/10/logrossocio/disponibles
        // Logros que el gimnasio tiene habilitados pero el socio aún no ganó
        [HttpGet("disponibles")]
        public async Task<ActionResult<IEnumerable<LogroDisponibleDTO>>> ObtenerLogrosDisponiblesPorSocio(
            int socioId, 
            int gimnasioId)
        {
            if (socioId <= 0)
                return BadRequest(new { Mensaje = "El ID del socio debe ser mayor a cero." });

            if (gimnasioId <= 0)
                return BadRequest(new { Mensaje = "El ID del gimnasio debe ser mayor a cero." });

            try
            {
                var logros = await _obtenerLogrosDisponiblesSocioCasoDeUso.Ejecutar(socioId, gimnasioId);
                return Ok(logros);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }
    }
}
