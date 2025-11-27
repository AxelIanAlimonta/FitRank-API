using FitRank_API.Application.CasosDeUso.RankingCasosDeUso;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RankingController : ControllerBase
    {
        private readonly ObtenerRankingGeneralCasoDeUso _obtenerRankingGeneralCasoDeUso;
        private readonly ObtenerPosicionPorIdCasoDeUso _obtenerPosicionPorIdCasoDeUso;

        public RankingController(ObtenerRankingGeneralCasoDeUso obtenerRankingGeneralCasoDeUso, ObtenerPosicionPorIdCasoDeUso obtenerPosicionPorIdCasoDeUso)
        {
            _obtenerRankingGeneralCasoDeUso = obtenerRankingGeneralCasoDeUso;
            _obtenerPosicionPorIdCasoDeUso = obtenerPosicionPorIdCasoDeUso;
        }

        [HttpGet("top/{cantidad}")]
        public async Task<IActionResult> ObtenerRankingGeneral(int cantidad)
        {
            if (cantidad <= 0)
                return BadRequest(new { Mensaje = "El parámetro 'cantidad' debe ser un número positivo mayor que cero." });

            try
            {
                var ranking = await _obtenerRankingGeneralCasoDeUso.Ejecutar(cantidad);

                if (ranking == null || !ranking.Any())
                    return NotFound(new { Mensaje = "No se encontraron datos para el ranking." });

                return Ok(ranking);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("{id}/puntaje")]
        public async Task<IActionResult> ObtenerPosicionSocio(long id)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID del socio debe ser mayor a cero." });

            try
            {
                var posicion = await _obtenerPosicionPorIdCasoDeUso.Ejecutar(id);
                if (posicion == null)
                    return NotFound(new { Mensaje = $"No se encontró al socio con ID {id}." });

                return Ok(posicion);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }
    }
}
