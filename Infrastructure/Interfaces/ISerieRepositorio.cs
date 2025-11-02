using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface ISerieRepositorio
    {
        Task<IEnumerable<Serie>> ObtenerTodasAsync();
        Task<Serie?> ObtenerPorIdAsync(long id);
        Task<IEnumerable<Serie>> ObtenerPorEjercicioAsync(long ejercicioAsignadoId);
        Task<Serie> AgregarAsync(Serie serie);
        Task ActualizarAsync(Serie serie);
        Task EliminarAsync(long id);
    }
}
