using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface IActividadRepositorio
    {
        Task<IEnumerable<Actividad>> ObtenerTodasAsync();
        Task<Actividad?> ObtenerPorIdAsync(long id);
        Task<IEnumerable<Actividad>> ObtenerPorSerieAsync(long serieId);
        Task<Actividad> AgregarAsync(Actividad actividad);
        Task<Actividad?> ActualizarAsync(Actividad actividad);
        Task EliminarAsync(long id);
    }
}
