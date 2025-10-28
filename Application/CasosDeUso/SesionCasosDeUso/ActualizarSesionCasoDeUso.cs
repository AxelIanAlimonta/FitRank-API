using AutoMapper;
using FitRank_API.Application.DTOs.RutinaDTOs;
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
        public async Task<ObtenerSesionDTO> Ejecutar(long id, ActualizarSesionDTO sesionActualizada)
        {
            var sesionEntidad = _mapper.Map<Domain.Entities.Sesion>(sesionActualizada);
            var sesionActualizadaEntidad = await _sesionRepositorio.ActualizarAsync(id, sesionEntidad);
            return _mapper.Map<ObtenerSesionDTO>(sesionActualizadaEntidad);
        }
    }
}
