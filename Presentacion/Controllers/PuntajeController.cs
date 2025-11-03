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
        public PuntajeController(CalcularEstadisticaCorporalSocioCasoDeUso calcularEstadisticaSocioCasoDeUso, CalcularEstadisticaCombinadaPuntajeSocioCasoDeUso calcularEstadisticaCombinadaPuntajeSocioCasoDeUso, ObtenerPuntajePorGrupoMuscularSocioCasoDeUso obtenerPuntajePorGrupoMuscularSocioCasoDeUso, ObtenerRankingSociosCasoDeUso obtenerRankingSociosCasoDeUso, ObtenerPuntajeTotalSocioCasoDeUso obtenerPuntajeTotalSocioCasoDeUso)
        {
            _calcularEstadisticaSocioCasoDeUso = calcularEstadisticaSocioCasoDeUso;
            _calcularEstadisticaCombinadaPuntajeSocioCasoDeUso = calcularEstadisticaCombinadaPuntajeSocioCasoDeUso;
            _obtenerPuntajePorGrupoMuscularSocioCasoDeUso = obtenerPuntajePorGrupoMuscularSocioCasoDeUso;
            _obtenerRankingSociosCasoDeUso =  obtenerRankingSociosCasoDeUso;
            _obtenerPuntajeTotalSocioCasoDeUso = obtenerPuntajeTotalSocioCasoDeUso;
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
        public async Task<IActionResult> ObtenerRanking()
        {
            var resultado = await _obtenerRankingSociosCasoDeUso.Ejecutar();
            return Ok(resultado);
        }

        [HttpGet("{socioId}/puntaje-total")]
        public async Task<IActionResult> ObtenerPuntajeTotal(long socioId)
        {
            var resultado = await _obtenerPuntajeTotalSocioCasoDeUso.Ejecutar(socioId);
            return resultado == null ? NotFound("No se encontraron actividades para este socio.") : Ok(resultado);
        }

    }
}
