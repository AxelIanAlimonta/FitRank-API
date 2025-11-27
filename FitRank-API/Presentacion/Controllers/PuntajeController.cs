using FitRank_API.Application.CasosDeUso.CalculoPuntajeCasosDeUso;
using FitRank_API.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PuntajeController : ControllerBase
    {
        private readonly CalcularEstadisticaCorporalSocioCasoDeUso _calcularEstadisticaSocioCasoDeUso;
        private readonly CalcularEstadisticaCombinadaPuntajeSocioCasoDeUso _calcularEstadisticaCombinadaPuntajeSocioCasoDeUso;
        private readonly ObtenerPuntajePorGrupoMuscularSocioCasoDeUso _obtenerPuntajePorGrupoMuscularSocioCasoDeUso;
        private readonly ObtenerRankingSociosCasoDeUso _obtenerRankingSociosCasoDeUso;
        private readonly ObtenerPuntajeTotalSocioCasoDeUso _obtenerPuntajeTotalSocioCasoDeUso;
        private readonly ObtenerRankingPorGrupoMuscularCasoDeUso _obtenerRankingPorGrupoMuscularCasoDeUso;
        private readonly ObtenerRankingPorFechaCasoDeUso _obtenerRankingPorFechaCasoDeUso;
        private readonly ObtenerRankingFiltradoCasoDeUso _obtenerRankingFiltradoCasoDeUso;

        public PuntajeController(
            CalcularEstadisticaCorporalSocioCasoDeUso calcularEstadisticaSocioCasoDeUso,
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
            _obtenerRankingSociosCasoDeUso = obtenerRankingSociosCasoDeUso;
            _obtenerPuntajeTotalSocioCasoDeUso = obtenerPuntajeTotalSocioCasoDeUso;
            _obtenerRankingPorGrupoMuscularCasoDeUso = obtenerRankingPorGrupoMuscularCasoDeUso;
            _obtenerRankingPorFechaCasoDeUso = obtenerRankingPorFechaCasoDeUso;
            _obtenerRankingFiltradoCasoDeUso = obtenerRankingFiltradoCasoDeUso;
        }

        [HttpGet("{socioId}/estadisticas")]
        public async Task<IActionResult> ObtenerEstadisticaCorporal(long socioId)
        {
            if (socioId <= 0)
                return BadRequest(new { Mensaje = "El ID del socio debe ser mayor a cero." });

            try
            {
                var resultado = await _calcularEstadisticaSocioCasoDeUso.Ejecutar(socioId);
                if (resultado == null)
                    return NotFound(new { Mensaje = "No se encontraron medidas corporales para este socio." });

                return Ok(resultado);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        //Es un endpoint que trae el puntaje total de un socio y el puntaje de sus grupos musculares
        [HttpGet ("{socioId}/puntaje-combinado")]
        public async Task<IActionResult> ObtenerPuntajeCombinado(long socioId)
        {
            if (socioId <= 0)
                return BadRequest(new { Mensaje = "El ID del socio debe ser mayor a cero." });

            try
            {
                var resultado = await _calcularEstadisticaCombinadaPuntajeSocioCasoDeUso.Ejecutar(socioId);
                if (resultado == null)
                    return NotFound(new { Mensaje = "No se encontraron actividades para este socio." });

                return Ok(resultado);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("{socioId}/puntaje-por-grupo")]
        public async Task<IActionResult> ObtenerPuntajePorGrupoMuscular(long socioId)
        {
            if (socioId <= 0)
                return BadRequest(new { Mensaje = "El ID del socio debe ser mayor a cero." });

            try
            {
                var resultado = await _obtenerPuntajePorGrupoMuscularSocioCasoDeUso.Ejecutar(socioId);
                if (resultado == null)
                    return NotFound(new { Mensaje = "No se encontraron actividades para este socio." });

                return Ok(resultado);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("ranking")]
        public async Task<IActionResult> ObtenerRanking(
            [FromQuery] long gimnasioId,
            [FromQuery] int cantidad = 20)
        {
            if (gimnasioId <= 0)
                return BadRequest(new { Mensaje = "El ID del gimnasio debe ser mayor a cero." });

            if (cantidad <= 0)
                return BadRequest(new { Mensaje = "La cantidad debe ser un número positivo mayor que cero." });

            try
            {
                var resultado = await _obtenerRankingSociosCasoDeUso.Ejecutar(gimnasioId, cantidad);
                if (!resultado.Any())
                    return NotFound(new { Mensaje = "No se encontraron datos para el gimnasio especificado." });

                return Ok(resultado);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("ranking/grupoMuscular/{grupoId}")]
        public async Task<IActionResult> ObtenerRankingPorGrupoMuscular(
            [FromRoute] long grupoId,
            [FromQuery] long gimnasioId,
            [FromQuery] int cantidad = 20)
        {
            if (grupoId <= 0)
                return BadRequest(new { Mensaje = "El ID del grupo muscular debe ser mayor a cero." });

            if (gimnasioId <= 0)
                return BadRequest(new { Mensaje = "El ID del gimnasio debe ser mayor a cero." });

            if (cantidad <= 0)
                return BadRequest(new { Mensaje = "La cantidad debe ser un número positivo mayor que cero." });

            try
            {
                var resultado = await _obtenerRankingPorGrupoMuscularCasoDeUso.Ejecutar(gimnasioId, grupoId, cantidad);
                if (!resultado.Any())
                    return NotFound(new { Mensaje = "No se encontraron datos para los parámetros especificados." });

                return Ok(resultado);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("ranking/fecha")]
        public async Task<IActionResult> ObtenerRankingPorFecha(
            [FromQuery] long gimnasioId,
            [FromQuery] DateTime desde,
            [FromQuery] DateTime hasta,
            [FromQuery] int cantidad = 20)
        {
            if (gimnasioId <= 0)
                return BadRequest(new { Mensaje = "El ID del gimnasio debe ser mayor a cero." });

            if (cantidad <= 0)
                return BadRequest(new { Mensaje = "La cantidad debe ser un número positivo mayor que cero." });

            if (desde > hasta)
                return BadRequest(new { Mensaje = "La fecha 'desde' no puede ser mayor que la fecha 'hasta'." });

            try
            {
                var desdeDateOnly = DateOnly.FromDateTime(desde);
                var hastaDateOnly = DateOnly.FromDateTime(hasta);

                var resultado = await _obtenerRankingPorFechaCasoDeUso.Ejecutar(
                    gimnasioId, cantidad, desdeDateOnly, hastaDateOnly);

                if (!resultado.Any())
                    return NotFound(new { Mensaje = "No se encontraron datos para el rango de fechas especificado." });

                return Ok(resultado);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }


        [HttpGet("{socioId}/puntaje-total")]
        public async Task<IActionResult> ObtenerPuntajeTotal(long socioId)
        {
            if (socioId <= 0)
                return BadRequest(new { Mensaje = "El ID del socio debe ser mayor a cero." });

            try
            {
                var resultado = await _obtenerPuntajeTotalSocioCasoDeUso.Ejecutar(socioId);
                return Ok(new { PuntajeTotal = resultado });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("ranking/filtrar")]
        public async Task<IActionResult> ObtenerRankingFiltrado(
            [FromQuery] long gimnasioId,
            [FromQuery] long? grupoId,
            [FromQuery] DateOnly? desde,
            [FromQuery] DateOnly? hasta,
            [FromQuery] int cantidad = 20)
        {
            if (gimnasioId <= 0)
                return BadRequest(new { Mensaje = "El ID del gimnasio debe ser mayor a cero." });

            if (grupoId.HasValue && grupoId.Value <= 0)
                return BadRequest(new { Mensaje = "El ID del grupo muscular debe ser mayor a cero." });

            if (cantidad <= 0)
                return BadRequest(new { Mensaje = "La cantidad debe ser un número positivo mayor que cero." });

            if (desde.HasValue && hasta.HasValue && desde > hasta)
                return BadRequest(new { Mensaje = "La fecha 'desde' no puede ser mayor que la fecha 'hasta'." });

            try
            {
                var resultado = await _obtenerRankingFiltradoCasoDeUso.Ejecutar(
                    gimnasioId, grupoId, desde, hasta, cantidad);

                return Ok(resultado);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }
    }
}
