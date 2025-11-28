using FitRank_API.Application.DTOs.Asistencia;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Domain.Interfaces
{

    public interface IAsistenciaRepositorio
    {
        Task<Asistencia> AgregarAsync(Asistencia asistencia);
        Task<List<Asistencia>> ObtenerPorGimnasioYRangoAsync(long gimnasioId, DateTime? desde = null, DateTime? hasta = null);
        Task<Asistencia?> ObtenerPorIdAsync(long id);
        Task<Asistencia?> ActualizarAsync(Asistencia asistencia);
        Task<IEnumerable<Asistencia>> ObtenerTodasAsync();
        Task<List<Asistencia>> ObtenerPorUsuarioAsync(long usuarioId);

        Task<Asistencia?> ObtenerPorUsuarioYFechaAsync(long usuarioId, DateTime fecha);

        Task<bool> EliminarAsync(long Id);

        Task<List<Asistencia>> ObtenerTodasConUsuarioAsync();
        Task<Asistencia?> ObtenerUltimaAsistenciaPorUsuarioAsync(long usuarioId);

    }
}
