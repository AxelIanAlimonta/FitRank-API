using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface IAsistenciaRepositorio
    {
        Task<Asistencia> AgregarAsync(Asistencia asistencia);
        Task<IEnumerable<Asistencia>> ObtenerPorUsuarioAsync(long usuarioId);
        Task<Asistencia?> ObtenerPorIdAsync(long id);
        Task ActualizarAsync(Asistencia asistencia);
        Task<IEnumerable<Asistencia>> ObtenerTodasAsync();
    }
}
