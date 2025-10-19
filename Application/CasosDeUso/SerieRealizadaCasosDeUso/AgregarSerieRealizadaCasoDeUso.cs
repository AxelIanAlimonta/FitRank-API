using AutoMapper;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SerieRealizadaCasosDeUso
{
    public class AgregarSerieRealizadaCasoDeUso
    {
        private readonly ISerieRealizadaRepositorio _serieRealizadaRepositorio;
        private readonly IMapper _mapper;

        public AgregarSerieRealizadaCasoDeUso(ISerieRealizadaRepositorio serieRealizadaRepositorio, IMapper mapper)
        {
            _serieRealizadaRepositorio = serieRealizadaRepositorio;
            _mapper = mapper;
        }

        public async Task<ObtenerSerieRealizadaDTO> Ejecutar(AgregarSerieRealizadaDTO nuevaSerieRealizada)
        {
            var serieRealizadaEntidad = _mapper.Map<Domain.Entities.SerieRealizada>(nuevaSerieRealizada);
            var serieRealizadaAgregada = await _serieRealizadaRepositorio.Agregar(serieRealizadaEntidad);
            return _mapper.Map<ObtenerSerieRealizadaDTO>(serieRealizadaAgregada);
        }
    }
}
