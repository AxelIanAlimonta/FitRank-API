using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces;

public interface IDificultadRepositorio
{
    Task<List<Dificultad>> ObtenerTodosAsync();
    Task<Dificultad?> ObtenerPorIdAsync(long id);
    Task<Dificultad?> AgregarAsync(Dificultad dificultad);
    Task<Dificultad?> ActualizarAsync(Dificultad dificultad);
    Task EliminarAsync(long id);
}
