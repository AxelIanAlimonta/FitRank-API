using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.NotificacionCasosDeUso
{
    public class MarcarNotificacionLeidaCasoDeUso
    {
        private readonly INotificacionRepositorio _notificacionRepositorio;

        public MarcarNotificacionLeidaCasoDeUso(INotificacionRepositorio notificacionRepositorio)
        {
            _notificacionRepositorio = notificacionRepositorio;
        }

        public async Task<bool> Ejecutar(long usuarioId, long notificacionId)
        {
            var notificacion = await _notificacionRepositorio.ObtenerPorIdAsync(notificacionId);
            if (notificacion == null || notificacion.UsuarioReceptorId != usuarioId)
                return false;

            notificacion.Leido = true;
            await _notificacionRepositorio.ActualizarAsync(notificacion);
            return true;
        }
    }
}
