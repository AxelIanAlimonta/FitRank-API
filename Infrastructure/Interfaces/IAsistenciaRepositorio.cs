using FitRank_API.Application.DTOs.Asistencia;
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

        Task<List<AsistenciaPorDiaDTO>> ObtenerConteoPorDiaAsync(int gimnasioId, DateTime? desde = null, DateTime? hasta = null);

        Task<List<AsistenciaDetalleUsuarioDTO>> ObtenerAsistenciasDetalladasPorUsuarioAsync(int usuarioId);

    }
}
