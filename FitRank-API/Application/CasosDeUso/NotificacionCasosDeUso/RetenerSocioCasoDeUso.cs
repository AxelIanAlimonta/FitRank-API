using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.NotificacionCasosDeUso
{
    public class RetenerSocioCasoDeUso
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly INotificacionRepositorio _notificacionRepositorio;

        public RetenerSocioCasoDeUso(
            IUsuarioRepositorio usuarioRepositorio,
            INotificacionRepositorio notificacionRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _notificacionRepositorio = notificacionRepositorio;
        }

        public virtual async Task<bool> Ejecutar(long adminId, long socioId)
        {
          
            var socio = await _usuarioRepositorio.ObtenerPorIdAsync(socioId);
            if (socio == null)
                throw new Exception("No se encontró el socio seleccionado.");

            var notificacion = new Notificacion
            {
                UsuarioEmisorId = adminId,
                UsuarioReceptorId = socio.Id,
                Titulo = "Te extrañamos en FitRank",
                Mensaje = $"Hola {socio.Nombre}, notamos que hace varios días no venís al gimnasio. " +
                          $"Podés ajustar tu rutina o hablar con un entrenador para retomar con motivación .",
                Activa = true,
                Leido = false,
                FechaEnvio = DateTime.UtcNow
            };

            await _notificacionRepositorio.AgregarAsync(notificacion);
            return true;
        }
    }
}
