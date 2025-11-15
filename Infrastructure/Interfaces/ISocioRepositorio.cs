using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces;

public interface ISocioRepositorio
{
    Task<List<Socio>> ObtenerTodosAsync();
    Task<Socio?> ObtenerPorIdAsync(long id);
    Task<Socio> AgregarAsync(Socio socio);
    Task<Socio?> ActualizarAsync(Socio socio);
    Task<bool> EliminarAsync(long id);
    Task<Socio?> ObtenerSocioConMedidasAsync(long socioId);
    Task<Socio?> ObtenerSocioConEntrenamientosAsync(long socioId);
    Task<IEnumerable<Socio>> ObtenerTodosConEntrenamientoAsync();

    Task<IEnumerable<Socio>> ObtenerTodosPorGimnasio(long gimnasioId);
}
