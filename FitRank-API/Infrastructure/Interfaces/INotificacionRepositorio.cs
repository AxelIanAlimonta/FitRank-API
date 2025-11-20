using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface INotificacionRepositorio
    {
        Task<Notificacion> AgregarAsync(Notificacion notificacion);

        Task<IEnumerable<Notificacion>> ObtenerPorUsuarioAsync(long usuarioId);

        Task<Notificacion?> ObtenerPorIdAsync(long id);

        Task ActualizarAsync(Notificacion notificacion);

        Task MarcarComoLeidaAsync(long id);

        Task DesactivarAsync(long id);
        Task<IEnumerable<Notificacion>> ObtenerNotificacionesDelGimnasio(long adminId);

        Task EnviarNotificacionGlobal(long adminId, string titulo, string mensaje);
      
        Task<List<Usuario>> ObtenerUsuariosDelGimnasio(long gimnasioId);

        Task<long?> ObtenerNotificacionesDelGimnasioadmin(long userId);

        Task<IEnumerable<Notificacion>> ObtenerNotificacionesDelGimnasio(IEnumerable<long> usuarioIds);



    }


}
