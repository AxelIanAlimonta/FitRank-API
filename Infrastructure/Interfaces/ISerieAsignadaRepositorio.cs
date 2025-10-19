using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces;

public interface ISerieAsignadaRepositorio
{
    Task<IEnumerable<SerieAsignada>> ObtenerTodasAsync();
    Task<SerieAsignada?> ObtenerPorIdAsync(long id);
    Task<SerieAsignada> AgregarAsync(SerieAsignada serieAsignada);
    Task<SerieAsignada?> ActualizarAsync(SerieAsignada serieAsignada);
    Task<bool> EliminarAsync(long id);

}
