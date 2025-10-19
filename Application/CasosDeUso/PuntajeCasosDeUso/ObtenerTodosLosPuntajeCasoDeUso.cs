using AutoMapper;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.PuntajeCasosDeUso
{
    public class ObtenerTodosLosPuntajeCasoDeUso
    {
        private readonly IPuntajeRepositorio _puntajeRepositorio;
        private readonly IMapper _mapper;
        public ObtenerTodosLosPuntajeCasoDeUso(IPuntajeRepositorio puntajeRepositorio, IMapper mapper)
        {
            _puntajeRepositorio = puntajeRepositorio;
            _mapper = mapper;
        }
        public async Task<List<ObtenerPuntajeDTO>> Ejecutar()
        {
            var puntajesEntidad = await _puntajeRepositorio.ObtenerTodos();
            return _mapper.Map<List<ObtenerPuntajeDTO>>(puntajesEntidad);
        }
    }
}
