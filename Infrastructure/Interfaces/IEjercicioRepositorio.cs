using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces;

public interface IEjercicioRepositorio
{
    Task<List<Ejercicio>> ObtenerEjerciciosAsync();
    Task<Ejercicio?> ObtenerEjercicioPorIdAsync(long id);
    Task<Ejercicio> AgregarEjercicioAsync(Ejercicio ejercicio);
    Task<Ejercicio?> ActualizarEjercicioAsync(Ejercicio ejercicio);
    Task<bool> EliminarEjercicioAsync(long id);
}
