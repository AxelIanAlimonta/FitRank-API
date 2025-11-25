using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface ILogroSocioRepositorio
    {
        Task<LogroSocio?> ObtenerPorIdAsync(long id);
        Task<bool> ExisteAsync(long logroId, long gimnasioId, long socioId);
        Task<LogroSocio> CrearAsync(LogroSocio logroSocio);
        Task<IEnumerable<LogroSocio>> ObtenerPorSocioYGimnasioAsync(long socioId, long gimnasioId);
        Task<IEnumerable<LogroSocio>> ObtenerPorSocioAsync(long socioId);
    }
}
