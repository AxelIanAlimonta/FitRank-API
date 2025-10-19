using AutoMapper;
using FitRank_API.Application.DTOs.SerieAsignadaDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SerieAsignadaCasoDeUso;

public class ActualizarSerieAsignadaCasoDeUso
{
    private readonly ISerieAsignadaRepositorio _serieAsignadaRepositorio;
    private readonly IMapper _mapper;
    public ActualizarSerieAsignadaCasoDeUso(ISerieAsignadaRepositorio serieAsignadaRepositorio, IMapper mapper)
    {
        _serieAsignadaRepositorio = serieAsignadaRepositorio;
        _mapper = mapper;
    }
    public async Task<ObtenerSerieAsignadaDTO?> Ejecutar(ActualizarSerieAsignadaDTO actualizarSerieAsignadaDTO)
    {
        var serieAsignadaEntidad = _mapper.Map<Domain.Entities.SerieAsignada>(actualizarSerieAsignadaDTO);
        var serieAsignadaActualizada = await _serieAsignadaRepositorio.ActualizarAsync(serieAsignadaEntidad);
        return _mapper.Map<ObtenerSerieAsignadaDTO?>(serieAsignadaActualizada);
    }

}
