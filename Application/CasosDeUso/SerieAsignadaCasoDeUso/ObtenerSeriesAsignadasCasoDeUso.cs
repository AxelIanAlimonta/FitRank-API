using AutoMapper;
using FitRank_API.Application.DTOs.SerieAsignadaDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SerieAsignadaCasoDeUso;

public class ObtenerSeriesAsignadasCasoDeUso
{
    private readonly ISerieAsignadaRepositorio _serieAsignadaRepositorio;
    private readonly IMapper _mapper;
    public ObtenerSeriesAsignadasCasoDeUso(ISerieAsignadaRepositorio serieAsignadaRepositorio, IMapper mapper)
    {
        _serieAsignadaRepositorio = serieAsignadaRepositorio;
        _mapper = mapper;
    }
    public async Task<List<ObtenerSerieAsignadaDTO>> Ejecutar()
    {
        var seriesAsignadas = await _serieAsignadaRepositorio.ObtenerTodasAsync();
        return _mapper.Map<List<ObtenerSerieAsignadaDTO>>(seriesAsignadas);
    }
}
