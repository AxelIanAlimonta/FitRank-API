using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SerieAsignadaCasoDeUso;

public class EliminarSerieAsignadaCasoDeUso
{
    private readonly ISerieAsignadaRepositorio _serieAsignadaRepositorio;
    public EliminarSerieAsignadaCasoDeUso(ISerieAsignadaRepositorio serieAsignadaRepositorio)
    {
        _serieAsignadaRepositorio = serieAsignadaRepositorio;
    }
    public async Task<bool> Ejecutar(long id)
    {
        return await _serieAsignadaRepositorio.EliminarAsync(id);
    }
}
