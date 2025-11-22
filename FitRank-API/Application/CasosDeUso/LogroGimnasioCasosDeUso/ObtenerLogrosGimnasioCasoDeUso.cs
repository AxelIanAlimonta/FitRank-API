using AutoMapper;
using FitRank_API.Application.DTOs.LogroGimnasioDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.LogroGimnasioCasosDeUso
{
    public class ObtenerLogrosGimnasioCasoDeUso
    {
        private readonly ILogroGimnasioRepositorio _logroGimnasioRepositorio;
        private readonly IMapper _mapper;

        public ObtenerLogrosGimnasioCasoDeUso(
            ILogroGimnasioRepositorio logroGimnasioRepositorio,
            IMapper mapper)
        {
            _logroGimnasioRepositorio = logroGimnasioRepositorio;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LogroGimnasioDTO>> Ejecutar(long gimnasioId)
        {
            var entidades = await _logroGimnasioRepositorio.ObtenerPorGimnasioAsync(gimnasioId);
            return _mapper.Map<IEnumerable<LogroGimnasioDTO>>(entidades);
        }
    }
}
