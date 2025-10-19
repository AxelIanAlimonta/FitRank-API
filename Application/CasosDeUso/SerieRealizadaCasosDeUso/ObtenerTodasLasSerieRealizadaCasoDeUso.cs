using AutoMapper;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SerieRealizadaCasosDeUso
{
    public class ObtenerTodasLasSerieRealizadaCasoDeUso
    {
        private readonly ISerieRealizadaRepositorio _serieRealizadaRepositorio;
        private readonly IMapper _mapper;
        public ObtenerTodasLasSerieRealizadaCasoDeUso(ISerieRealizadaRepositorio serieRealizadaRepositorio, IMapper mapper)
        {
            _serieRealizadaRepositorio = serieRealizadaRepositorio;
            _mapper = mapper;
        }
        public async Task<List<ObtenerSerieRealizadaDTO>> Ejecutar()
        {
            var seriesRealizadasEntidad = await _serieRealizadaRepositorio.ObtenerTodos();
            return _mapper.Map<List<ObtenerSerieRealizadaDTO>>(seriesRealizadasEntidad);
        }
    }
}
