using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.NotificacionCasosDeUso
{
    public class ActualizarNotificacionCasoDeUso
    {
        private readonly INotificacionRepositorio _notificacionRepositorio;

        public ActualizarNotificacionCasoDeUso(INotificacionRepositorio notificacionRepositorio)
        {
            _notificacionRepositorio = notificacionRepositorio;
        }


        public virtual async Task<bool> Ejecutar(long notificacionId, bool? leida = null, bool? activa = null)
        {
            var notificacion = await _notificacionRepositorio.ObtenerPorIdAsync(notificacionId);

            if (notificacion == null)
                return false;

            if (leida.HasValue)
                notificacion.Leido = leida.Value;

            if (activa.HasValue)
                notificacion.Activa = activa.Value;

            await _notificacionRepositorio.ActualizarAsync(notificacion);
            return true;
        }
    }
}
