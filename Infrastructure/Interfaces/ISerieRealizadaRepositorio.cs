using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface ISerieRealizadaRepositorio
    {
        Task<List<SerieRealizada>> ObtenerTodas();
        Task<SerieRealizada?> ObtenerPorId(long id);
        Task<SerieRealizada> Agregar(SerieRealizada rutina);
        Task<SerieRealizada?> Actualizar(SerieRealizada rutina);
        Task<bool> Eliminar(long id);
    }
}
