using AutoMapper;
using FitRank_API.Application.DTOs.RankingDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.RankingCasosDeUso
{
    public class ObtenerRankingGeneralCasoDeUso
    {
        private readonly IRankingRepositorio _rankingRepositorio;
        private readonly IMapper _mapper;

        public ObtenerRankingGeneralCasoDeUso(IRankingRepositorio rankingRepositorio, IMapper mapper)
        {
            _rankingRepositorio = rankingRepositorio;
            _mapper = mapper;
        }

        public virtual async Task<List<RankingDTO>> Ejecutar(int top)
        {
            var rankingEntidades = await _rankingRepositorio.ObtenerTopSociosAsync(top);
            return _mapper.Map<List<RankingDTO>>(rankingEntidades);
        }
    }
}
