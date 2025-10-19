using AutoMapper;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SerieAsignadaCasoDeUso;

public class ObtenerSerieAsignadaPorIdCasoDeUso
{
    private readonly ISerieAsignadaRepositorio _serieAsignadaRepositorio;
    private readonly IMapper _mapper;
    public ObtenerSerieAsignadaPorIdCasoDeUso(ISerieAsignadaRepositorio serieAsignadaRepositorio, IMapper mapper)
    {
        _serieAsignadaRepositorio = serieAsignadaRepositorio;
        _mapper = mapper;
    }
    public async Task<SerieAsignada?> Ejecutar(long id)
    {
        var serieAsignada = await _serieAsignadaRepositorio.ObtenerPorIdAsync(id);
        return serieAsignada;
    }
}
