using FitRank_API.Domain.Entities;

namespace FitRank_API.Domain.Interfaces
{
    public interface ISerieRepositorio
    {
        Task<IEnumerable<Serie>> ObtenerTodasAsync();
        Task<Serie?> ObtenerPorIdAsync(long id);
        Task<IEnumerable<Serie>> ObtenerPorEjercicioAsync(long ejercicioAsignadoId);
        Task<Serie> AgregarAsync(Serie serie);
        Task<Serie?> ActualizarAsync(Serie serie);
        Task<bool> EliminarAsync(long id);
    }
}
