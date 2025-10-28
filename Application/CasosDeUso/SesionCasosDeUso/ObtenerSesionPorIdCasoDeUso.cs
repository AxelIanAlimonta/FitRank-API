using AutoMapper;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SesionCasosDeUso;

    public class ObtenerSesionPorIdCasoDeUso
    {
        private readonly ISesionRepositorio _sesionRepositorio;
        private readonly IMapper _mapper;
   

    public ObtenerSesionPorIdCasoDeUso(ISesionRepositorio sesionRepositorio, IMapper mapper)
        {
            _sesionRepositorio = sesionRepositorio;
            _mapper = mapper;
        }

        public async Task<ObtenerSesionDTO?> Ejecutar(long id)
        {
            var sesion = await _sesionRepositorio.ObtenerPorIdAsync(id);
            if (sesion == null)
            {
                return null;
            }
            return _mapper.Map<ObtenerSesionDTO>(sesion);
        }
    }
