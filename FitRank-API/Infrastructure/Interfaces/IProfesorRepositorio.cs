using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface IProfesorRepositorio
    {
        Task<List<Profesor>> ObtenerTodosAsync();
        Task<Profesor?> ObtenerPorIdAsync(long id);
        Task<Profesor> AgregarAsync(Profesor profesor);
        Task<Profesor?> ActualizarAsync(Profesor profesor);
        Task<bool> EliminarAsync(long id);
    }
}
