using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface IEntrenamientoRepositorio
    {
        Task<IEnumerable<Entrenamiento>> ObtenerTodosAsync();
        Task<Entrenamiento?> ObtenerPorIdAsync(long id);
        Task<IEnumerable<Entrenamiento>> ObtenerPorSocioAsync(long socioId);
        Task<Entrenamiento> AgregarAsync(Entrenamiento entrenamiento);
        Task ActualizarAsync(Entrenamiento entrenamiento);
        Task EliminarAsync(long id);
    }
}
