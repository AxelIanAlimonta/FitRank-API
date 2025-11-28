using FitRank_API.Domain.Entities;

namespace FitRank_API.Domain.Interfaces
{
    public interface INotificacionRepositorio
    {
        Task<Notificacion> AgregarAsync(Notificacion notificacion);

        Task<IEnumerable<Notificacion>> ObtenerPorUsuarioAsync(long usuarioId);

        Task<Notificacion?> ObtenerPorIdAsync(long id);

        Task<Notificacion?> ActualizarAsync(Notificacion notificacion);

        Task MarcarComoLeidaAsync(long id);

        Task DesactivarAsync(long id);

        Task EnviarNotificacionGlobal(long adminId, string titulo, string mensaje);

        Task<List<Usuario>> ObtenerUsuariosDelGimnasio(long gimnasioId);

        Task<IEnumerable<Notificacion>> ObtenerNotificacionesDelGimnasio(IEnumerable<long> usuarioIds);
    }


}
