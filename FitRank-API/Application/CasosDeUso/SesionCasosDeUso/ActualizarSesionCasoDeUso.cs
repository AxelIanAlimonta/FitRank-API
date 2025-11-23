using AutoMapper;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Application.DTOs.SesionDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SesionCasosDeUso
{
    public class ActualizarSesionCasoDeUso
    {
        private readonly ISesionRepositorio _sesionRepositorio;
        private readonly IMapper _mapper;
        public ActualizarSesionCasoDeUso(ISesionRepositorio sesionRepositorio, IMapper mapper)
        {
            _sesionRepositorio = sesionRepositorio;
            _mapper = mapper;
        }
        
        public virtual async Task<ObtenerSesionDTO> Ejecutar(ActualizarSesionDTO sesionActualizada)
        {
            var sesionEntidad = _mapper.Map<Domain.Entities.Sesion>(sesionActualizada);
            var sesionActualizadaEntidad = await _sesionRepositorio.ActualizarAsync(sesionEntidad);
            return _mapper.Map<ObtenerSesionDTO>(sesionActualizadaEntidad);
        }
    }
}
