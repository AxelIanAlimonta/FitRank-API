using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.NotificacionCasosDeUso
{
    public class EnviarNotificacionIndividualCasoDeUso
    {
        private readonly INotificacionRepositorio _notiRepo;

        public EnviarNotificacionIndividualCasoDeUso(INotificacionRepositorio notiRepo)
        {
            _notiRepo = notiRepo;
        }

        public virtual async Task<Notificacion> Ejecutar(long emisorId, long receptorId, string titulo, string mensaje)
        {
            var noti = new Notificacion
            {
                UsuarioEmisorId = emisorId,
                UsuarioReceptorId = receptorId,
                Titulo = titulo,
                Mensaje = mensaje
            };

            return await _notiRepo.AgregarAsync(noti);
        }
    }
}
