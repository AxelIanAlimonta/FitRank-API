using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface ISesionRepositorio
    {
        Task<List<Sesion>> ObtenerTodasAsync();
        Task<Sesion?> ObtenerPorIdAsync(long id);
        Task<Sesion> AgregarAsync(Sesion sesion);
        Task<Sesion?> ActualizarAsync(Sesion sesion);
        Task<bool> EliminarAsync(long id);
    }
}
