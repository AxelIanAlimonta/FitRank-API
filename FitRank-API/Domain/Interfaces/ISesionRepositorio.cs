using FitRank_API.Domain.Entities;

namespace FitRank_API.Domain.Interfaces
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
