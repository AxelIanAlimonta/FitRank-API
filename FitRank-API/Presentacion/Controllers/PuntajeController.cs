using FitRank_API.Application.CasosDeUso.CalculoPuntajeCasosDeUso;
using FitRank_API.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PuntajeController : Controller
    {
        private readonly CalcularEstadisticaCorporalSocioCasoDeUso _calcularEstadisticaSocioCasoDeUso;
        private readonly CalcularEstadisticaCombinadaPuntajeSocioCasoDeUso _calcularEstadisticaCombinadaPuntajeSocioCasoDeUso;
        private readonly ObtenerPuntajePorGrupoMuscularSocioCasoDeUso _obtenerPuntajePorGrupoMuscularSocioCasoDeUso;
        private readonly ObtenerRankingSociosCasoDeUso _obtenerRankingSociosCasoDeUso;
        private readonly ObtenerPuntajeTotalSocioCasoDeUso _obtenerPuntajeTotalSocioCasoDeUso;
        private readonly ObtenerRankingPorGrupoMuscularCasoDeUso _obtenerRankingPorGrupoMuscularCasoDeUso;
        private readonly ObtenerRankingPorFechaCasoDeUso _obtenerRankingPorFechaCasoDeUso;
        private readonly ObtenerRankingFiltradoCasoDeUso _obtenerRankingFiltradoCasoDeUso;
        public PuntajeController(CalcularEstadisticaCorporalSocioCasoDeUso calcularEstadisticaSocioCasoDeUso, 
            CalcularEstadisticaCombinadaPuntajeSocioCasoDeUso calcularEstadisticaCombinadaPuntajeSocioCasoDeUso, 
            ObtenerPuntajePorGrupoMuscularSocioCasoDeUso obtenerPuntajePorGrupoMuscularSocioCasoDeUso, 
            ObtenerRankingSociosCasoDeUso obtenerRankingSociosCasoDeUso, 
            ObtenerPuntajeTotalSocioCasoDeUso obtenerPuntajeTotalSocioCasoDeUso,
            ObtenerRankingPorFechaCasoDeUso obtenerRankingPorFechaCasoDeUso,
            ObtenerRankingPorGrupoMuscularCasoDeUso obtenerRankingPorGrupoMuscularCasoDeUso,
            ObtenerRankingFiltradoCasoDeUso obtenerRankingFiltradoCasoDeUso)
        {
            _calcularEstadisticaSocioCasoDeUso = calcularEstadisticaSocioCasoDeUso;
            _calcularEstadisticaCombinadaPuntajeSocioCasoDeUso = calcularEstadisticaCombinadaPuntajeSocioCasoDeUso;
            _obtenerPuntajePorGrupoMuscularSocioCasoDeUso = obtenerPuntajePorGrupoMuscularSocioCasoDeUso;
            _obtenerRankingSociosCasoDeUso =  obtenerRankingSociosCasoDeUso;
            _obtenerPuntajeTotalSocioCasoDeUso = obtenerPuntajeTotalSocioCasoDeUso;
            _obtenerRankingPorGrupoMuscularCasoDeUso = obtenerRankingPorGrupoMuscularCasoDeUso;
            _obtenerRankingPorFechaCasoDeUso = obtenerRankingPorFechaCasoDeUso;
            _obtenerRankingFiltradoCasoDeUso = obtenerRankingFiltradoCasoDeUso;
        }


        [HttpGet("{socioId}/estadisticas")]
        public async Task<IActionResult> ObtenerEstadisticaCorporal(long socioId)
        {
            var resultado = await _calcularEstadisticaSocioCasoDeUso.Ejecutar(socioId);
            if (resultado == null) return NotFound("No se encontraron medidas corporales para este socio.");

            return Ok(resultado);
        }

        //Es un endpoint que trae el puntaje total de un socio y el puntaje de sus grupos musculares
        [HttpGet ("{socioId}/puntaje-combinado")]
        public async Task<IActionResult> ObtenerPuntajeCombiado(long socioId)
        {
            var resultado = await _calcularEstadisticaCombinadaPuntajeSocioCasoDeUso.Ejecutar(socioId);
            return resultado == null ? NotFound("No se encontraron actividades para este socio.") : Ok(resultado);
        }

        [HttpGet("{socioId}/puntaje-por-grupo")]
        public async Task<IActionResult> ObtenerPuntajePorGrupoMuscular(long socioId)
        {
            var resultado = await _obtenerPuntajePorGrupoMuscularSocioCasoDeUso.Ejecutar(socioId);
            return resultado == null ? NotFound("No se encontraron actividades para este socio.") : Ok(resultado);
        }

        [HttpGet("ranking")]
        public async Task<IActionResult> ObtenerRanking(
            [FromQuery]long gimnasioId, 
            [FromQuery]int cantidad = 20)
        {
            if (cantidad <= 0)
            {
                return BadRequest("La cantidad debe ser un número positivo mayor que cero.");
            }
            var resultado = await _obtenerRankingSociosCasoDeUso.Ejecutar(gimnasioId, cantidad);
            if (!resultado.Any())
            {
                return NotFound("No se encontraron datos para el rango de fechas especificado.");
            }
            return Ok(resultado);
        }

        [HttpGet("ranking/grupoMuscular/{grupoId}")]
        public async Task<IActionResult> ObtenerRankingPorGrupoMuscular(
            [FromRoute] long grupoId,
            [FromQuery] long gimnasioId,
            [FromQuery] int cantidad = 20)
        {
            if (cantidad <= 0)
            {
                return BadRequest("La cantidad debe ser un número positivo mayor que cero.");
            }
            var resultado = await _obtenerRankingPorGrupoMuscularCasoDeUso.Ejecutar(gimnasioId, grupoId, cantidad);
            if (!resultado.Any())
            {
                return NotFound("No se encontraron datos para el rango de fechas especificado.");
            }
            return Ok(resultado);
        }

        [HttpGet("ranking/fecha")]
        public async Task<IActionResult> ObtenerRankingPorFecha(
        [FromQuery] long gimnasioId,
        [FromQuery] DateTime desde,
        [FromQuery] DateTime hasta,
        [FromQuery] int cantidad = 0)
        {
            if(cantidad <= 0)
            {
                return BadRequest("La cantidad debe ser un número positivo mayor que cero.");
            }
            if (desde > hasta)
            {
                return BadRequest("La fecha 'desde' no puede ser mayor que la fecha 'hasta'.");
            }
            var desdeDateOnly = DateOnly.FromDateTime(desde);
            var hastaDateOnly = DateOnly.FromDateTime(hasta);

            var resultado = await _obtenerRankingPorFechaCasoDeUso.Ejecutar(
                gimnasioId, cantidad, desdeDateOnly, hastaDateOnly);

            if (!resultado.Any())
            {
                return NotFound("No se encontraron datos para el rango de fechas especificado.");
            }
            return Ok(resultado);
        }


        [HttpGet("{socioId}/puntaje-total")]
        public async Task<IActionResult> ObtenerPuntajeTotal(long socioId)
        {
            var resultado = await _obtenerPuntajeTotalSocioCasoDeUso.Ejecutar(socioId);
            return resultado == null ? NotFound("No se encontraron actividades para este socio.") : Ok(resultado);
        }

        [HttpGet("ranking/filtrar")]
        public async Task<IActionResult> ObtenerRankingFiltrado(
        [FromQuery] long gimnasioId,
        [FromQuery] long? grupoId,
        [FromQuery] DateOnly? desde,
        [FromQuery] DateOnly? hasta,
        [FromQuery] int cantidad = 20)
            {
            if (cantidad <= 0)
            {
                return BadRequest("La cantidad debe ser un número positivo mayor que cero.");
            }
            if (desde.HasValue && hasta.HasValue && desde > hasta)
            {
                return BadRequest("La fecha 'desde' no puede ser mayor que la fecha 'hasta'.");
            }
            var resultado = await _obtenerRankingFiltradoCasoDeUso.Ejecutar(
                    gimnasioId, grupoId, desde, hasta, cantidad
                );

            return Ok(resultado);
            }


    }
}
