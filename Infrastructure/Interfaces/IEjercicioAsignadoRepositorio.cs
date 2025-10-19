using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces;

public interface IEjercicioAsignadoRepositorio
{
    Task<List<EjercicioAsignado>> ObtenerTodosAsync();
    Task<EjercicioAsignado?> ObtenerPorIdAsync(long id);
    Task<EjercicioAsignado> AgregarAsync(EjercicioAsignado ejercicioAsignado);
    Task<EjercicioAsignado?> ActualizarAsync(EjercicioAsignado ejercicioAsignado);
    Task<bool> EliminarAsync(long id);


}
