using FitRank_API.Application.CasosDeUso.RankingCasosDeUso;
using Microsoft.AspNetCore.Http;
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
            {
                return BadRequest("El parámetro 'top' debe ser un número positivo.");
            }

            var ranking = await _obtenerRankingGeneralCasoDeUso.Ejecutar(cantidad);

            if (ranking == null || !ranking.Any())
            {
                return NotFound("No se encontraron datos para el ranking.");
            }
            return Ok(ranking);
        }

        [HttpGet("{id}/puntaje")]
        public async Task<IActionResult> ObtenerPosicionSocio(long id)
        {
            if (id <= 0)
            {
                return BadRequest("El parámetro 'socioId' debe ser un número positivo.");
            }
            var posicion = await _obtenerPosicionPorIdCasoDeUso.Ejecutar(id);
            if (posicion == null)
            {
                return NotFound("No se encontró al socio con el ID proporcionado.");
            }
            return Ok(posicion);
        }
    }
}
