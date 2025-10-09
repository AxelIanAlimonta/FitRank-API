using FitRank_API.Application.DTOs.Rankig;
using FitRank_API.Application.DTOs.Ranking;
using FitRank_API.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RankingController : ControllerBase
    {
        private readonly IRankingService _rankingService;
        public RankingController(IRankingService rankingService)
        {
            _rankingService = rankingService;
        }

       

        [HttpGet]
        public async Task<ActionResult<List<MostrarRankingDTO>>> GetRanking()
        {
            var ranking = await _rankingService.MostrarRankingAsync();



            return Ok(ranking);


        }
        [HttpGet("Grupomuscular")]
        public async Task<ActionResult<List<MostrarRankingPorGrupoMuscular>>> GetRankingGrupoMuscular()
        {
            var rankignGrupoMuscular = await _rankingService.MostrarRankingPorGrupoMuscularAsync();
            return Ok(rankignGrupoMuscular);
        }
    }
}
