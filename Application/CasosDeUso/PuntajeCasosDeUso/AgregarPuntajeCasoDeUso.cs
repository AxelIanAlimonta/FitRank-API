using AutoMapper;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.PuntajeCasosDeUso
{
    public class AgregarPuntajeCasoDeUso
    {
        private readonly IPuntajeRepositorio _puntajeRepositorio;
        private readonly IMapper _mapper;
        public AgregarPuntajeCasoDeUso(IPuntajeRepositorio puntajeRepositorio, IMapper mapper)
        {
            _puntajeRepositorio = puntajeRepositorio;
            _mapper = mapper;
        }
        public async Task<ObtenerPuntajeDTO> Ejecutar(AgregarPuntajeDTO nuevoPuntaje)
        {
            var puntajeEntidad = _mapper.Map<Domain.Entities.Puntaje>(nuevoPuntaje);
            var puntajeAgregado = await _puntajeRepositorio.Agregar(puntajeEntidad);
            return _mapper.Map<ObtenerPuntajeDTO>(puntajeAgregado);
        }
    }
}
