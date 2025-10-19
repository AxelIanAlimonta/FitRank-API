using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces;

public interface IEjercicioRealizadoRepositorio
{
    Task<List<EjercicioRealizado>> ObtenerTodos();
    Task<EjercicioRealizado?> ObtenerPorId(long id);
    Task<EjercicioRealizado> Agregar(EjercicioRealizado rutina);
    Task<EjercicioRealizado?> Actualizar(EjercicioRealizado rutina);
    Task<bool> Eliminar(long id);
}
