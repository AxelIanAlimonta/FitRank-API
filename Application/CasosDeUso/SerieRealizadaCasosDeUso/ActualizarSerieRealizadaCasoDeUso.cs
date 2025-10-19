using AutoMapper;
using FitRank_API.Application.DTOs.SerieRealizadaDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SerieRealizadaCasosDeUso
{
    public class ActualizarSerieRealizadaCasoDeUso
    {
        private readonly ISerieRealizadaRepositorio _serieRealizadaRepositorio;
        private readonly IMapper _mapper;

        public ActualizarSerieRealizadaCasoDeUso(ISerieRealizadaRepositorio serieRealizadaRepositorio, IMapper mapper)
        {
            _serieRealizadaRepositorio = serieRealizadaRepositorio;
            _mapper = mapper;
        }

        public async Task<ObtenerSerieRealizadaDTO?> Ejecutar(ActualizarSerieRealizadaDTO serieRealizadaActualizada)
        {
            var serieRealizadaEntidad = _mapper.Map<Domain.Entities.SerieRealizada>(serieRealizadaActualizada);
            var serieRealizadaActualizadaEntidad = await _serieRealizadaRepositorio.ActualizarAsync(serieRealizadaEntidad);
            if (serieRealizadaActualizadaEntidad == null)
            {
                return null;
            }
            return _mapper.Map<ObtenerSerieRealizadaDTO>(serieRealizadaActualizadaEntidad);
        }
    }
}
