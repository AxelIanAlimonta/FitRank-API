using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Enums;

namespace FitRank_API.Domain.Interfaces
{
    public interface IAmistadRepositorio
    {
        Task<Amistad?> ObtenerPorIdAsync(long id);
        Task<Amistad?> ObtenerPorIdDeSociosAsync(long socioId1, long socioId2);
        Task<Amistad> CrearAsync(Amistad amistad);
        Task<bool> EliminarAsync(Amistad amistad);
        Task<Amistad> ActualizarAsync(Amistad amistad);
        Task<List<Amistad>> ObtenerSolicitudesPendientesAsync(long socioId);
        Task<List<Amistad>> ObtenerPorSocioIdAsync(long socioId, EstadoAmistad estado);
    }
}
