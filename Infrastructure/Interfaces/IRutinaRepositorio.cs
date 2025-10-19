using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces;

public interface IRutinaRepositorio
{
    Task<List<Rutina>> ObtenerTodas();
    Task<Rutina?> ObtenerPorId(long id);
    Task<Rutina> Agregar(Rutina rutina);
    Task<Rutina?> Actualizar(Rutina rutina);
    Task<bool> Eliminar(long id);

}
