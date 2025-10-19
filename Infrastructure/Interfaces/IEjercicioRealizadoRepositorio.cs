using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces;

public interface IEjercicioRealizadoRepositorio
{
    Task<List<EjercicioRealizado>> ObtenerTodosAsync();
    Task<EjercicioRealizado?> ObtenerPorIdAsync(long id);
    Task<EjercicioRealizado> AgregarAsync(EjercicioRealizado rutina);
    Task<EjercicioRealizado?> ActualizarAsync(EjercicioRealizado rutina);
    Task<bool> EliminarAsync(long id);
}
