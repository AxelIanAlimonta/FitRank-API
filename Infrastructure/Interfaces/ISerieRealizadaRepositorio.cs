using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface ISerieRealizadaRepositorio
    {
        Task<List<SerieRealizada>> ObtenerTodasAsync();
        Task<SerieRealizada?> ObtenerPorIdAsync(long id);
        Task<SerieRealizada> AgregarAsync(SerieRealizada rutina);
        Task<SerieRealizada?> ActualizarAsync(SerieRealizada rutina);
        Task<bool> EliminarAsync(long id);
    }
}
