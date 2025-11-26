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
        Task<List<Profesor>> ObtenerPorGimnasioAsync(long gimnasioId);

        Task<bool> ExisteEmailAsync(string email);
        Task<bool> ExisteDniAsync(long dni);

    }
}
