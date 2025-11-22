using AutoMapper;
using FitRank_API.Application.DTOs.LogroSocioDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.LogroSocioCasosDeUso
{
    public class ObtenerLogrosSocioCasoDeUso
    {
        private readonly ILogroSocioRepositorio _logroSocioRepositorio;
        private readonly IMapper _mapper;

        public ObtenerLogrosSocioCasoDeUso(
            ILogroSocioRepositorio logroSocioRepositorio,
            IMapper mapper)
        {
            _logroSocioRepositorio = logroSocioRepositorio;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LogroSocioDTO>> Ejecutar(int socioId, int gimnasioId)
        {
            var entidades = await _logroSocioRepositorio
                .ObtenerPorSocioYGimnasioAsync(socioId, gimnasioId);

            return _mapper.Map<IEnumerable<LogroSocioDTO>>(entidades);
        }
    }
}
