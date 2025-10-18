using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces;

public interface ISocioRepositorio
{
    Task<IEnumerable<Socio>> ObtenerTodosLosSociosAsync();
    Task<Socio?> ObtenerSocioPorIdAsync(long id);
    Task<Socio> AgregarSocioAsync(Socio socio);
    Task<Socio?> ActualizarSocioAsync(Socio socio);
    Task<bool> EliminarSocioAsync(long id);
}
