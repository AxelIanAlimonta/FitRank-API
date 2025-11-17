using AutoMapper;
using FitRank_API.Application.DTOs.JornadaDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.JornadaCasosDeUso
{
    public class ObtenerJornadaPorIdCasoDeUso
    {
        private readonly IJornadaRepositorio _jornadaRepositorio;
        private readonly IMapper _mapper;

        public ObtenerJornadaPorIdCasoDeUso(IJornadaRepositorio jornadaRepositorio, IMapper mapper)
        {
            _jornadaRepositorio = jornadaRepositorio;
            _mapper = mapper;
        }
        public virtual async Task<ObtenerJornadaDTO?> Ejecutar(long id)
        {
            var jornada = await _jornadaRepositorio.ObtenerJornadaPorIdAsync(id);
            if (jornada == null)
            {
                return null;
            }
            return _mapper.Map<ObtenerJornadaDTO>(jornada);
        }
    }
}
