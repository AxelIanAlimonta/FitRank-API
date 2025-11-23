
using AutoMapper;
using FitRank_API.Application.DTOs.SesionDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SesionCasosDeUso
{
    public class ObtenerTodasLasSesionesCasoDeUso
    {
        private readonly ISesionRepositorio _sesionRepositorio;
        private readonly IMapper _mapper;

        public ObtenerTodasLasSesionesCasoDeUso(ISesionRepositorio sesionRepositorio, IMapper mapper)
        {
            _sesionRepositorio = sesionRepositorio;
            _mapper = mapper;
        }
        public virtual async Task<List<ObtenerSesionDTO>> Ejecutar()
        {
           var sesiones = await _sesionRepositorio.ObtenerTodasAsync();
            return _mapper.Map<List<ObtenerSesionDTO>>(sesiones);
        }
    }
}
