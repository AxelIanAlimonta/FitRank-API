using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces;

public interface IRutinaRepositorio
{
    Task<List<Rutina>> ObtenerTodasAsync();
    Task<Rutina?> ObtenerPorIdAsync(long id);
    Task<Rutina> AgregarAsync(Rutina rutina);
    Task<Rutina?> ActualizarAsync(Rutina rutina);
    Task<bool> EliminarAsync(long id);
    Task<Rutina> ObtenerPorSocioIdAsync(long socioId);
}
