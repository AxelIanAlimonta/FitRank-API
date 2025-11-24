using AutoMapper;
using FitRank_API.Application.DTOs.RankingDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.RankingCasosDeUso
{
    public class ObtenerPosicionPorIdCasoDeUso
    {
        private readonly IRankingRepositorio _rankingRepositorio;
        private readonly IMapper _mapper;
        
        public ObtenerPosicionPorIdCasoDeUso(IRankingRepositorio rankingRepositorio, IMapper mapper)
        {
            _rankingRepositorio = rankingRepositorio;
            _mapper = mapper;
        }

        public virtual async Task<PosicionDTO?> Ejecutar(long socioId)
        {
            var posicionEntidad = await _rankingRepositorio.ObtenerPosicionPorIdAsync(socioId);
            if (posicionEntidad == null)
            {
                return null;
            }
            return _mapper.Map<PosicionDTO>(posicionEntidad);
        }
    }
}
