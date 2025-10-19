using AutoMapper;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.PuntajeCasosDeUso
{
    public class ActualizarPuntajeCasoDeUso
    {
        private readonly IPuntajeRepositorio _puntajeRepositorio;
        private readonly IMapper _mapper;

        public ActualizarPuntajeCasoDeUso(IPuntajeRepositorio puntajeRepositorio, IMapper mapper)
        {
            _puntajeRepositorio = puntajeRepositorio;
            _mapper = mapper;
        }

        public async Task<ObtenerPuntajeDTO?> Ejecutar(ActualizarPuntajeDTO puntajeActualizado)
        {
            var puntajeEntidad = _mapper.Map<Domain.Entities.Puntaje>(puntajeActualizado);
            var puntajeActualizadoEntidad = await _puntajeRepositorio.Actualizar(puntajeEntidad);
            if (puntajeActualizadoEntidad == null)
            {
                return null;
            }
            return _mapper.Map<ObtenerPuntajeDTO>(puntajeActualizadoEntidad);
        }
    }
}
