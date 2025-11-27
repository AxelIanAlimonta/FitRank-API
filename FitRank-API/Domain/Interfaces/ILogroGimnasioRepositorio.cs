using FitRank_API.Domain.Entities;

namespace FitRank_API.Domain.Interfaces
{
    public interface ILogroGimnasioRepositorio
    {
        Task<LogroGimnasio?> ObtenerPorIdAsync(long id);
        Task<LogroGimnasio?> ObtenerPorGimnasioYLogroAsync(long gimnasioId, long logroId);
        Task<IEnumerable<LogroGimnasio>> ObtenerPorGimnasioAsync(long gimnasioId);
        Task<LogroGimnasio> CrearAsync(LogroGimnasio entidad);
        Task<LogroGimnasio?> ActualizarAsync(LogroGimnasio entidad);
    }
}
