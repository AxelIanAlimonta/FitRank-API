using AutoMapper;
using FitRank_API.Application.DTOs.PuntajeDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.PuntajeCasosDeUso
{
    public class ObtenerPuntajePorIdCasoDeUso
    {
        private readonly IPuntajeRepositorio _puntajeRepositorio;
        private readonly IMapper _mapper;
        public ObtenerPuntajePorIdCasoDeUso(IPuntajeRepositorio puntajeRepositorio, IMapper mapper)
        {
            _puntajeRepositorio = puntajeRepositorio;
            _mapper = mapper;
        }

        public virtual async Task<ObtenerPuntajeDTO?> Ejecutar(long id)
        {
            var puntajeEntidad = await _puntajeRepositorio.ObtenerPorIdAsync(id);
            if (puntajeEntidad == null)
            {
                return null;
            }
            return _mapper.Map<ObtenerPuntajeDTO>(puntajeEntidad);
        }
    }
}
