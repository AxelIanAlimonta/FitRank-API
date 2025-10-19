using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces;

public interface IRutinaEjercicioRepositorio
{
    Task<List<RutinaEjercicio>> ObtenerTodos();
    Task<RutinaEjercicio?> ObtenerPorId(long id);
    Task<RutinaEjercicio> Crear(RutinaEjercicio rutinaEjercicio);
    Task<RutinaEjercicio?> Actualizar(RutinaEjercicio rutinaEjercicio);
    Task<bool> Eliminar(long id);
}
