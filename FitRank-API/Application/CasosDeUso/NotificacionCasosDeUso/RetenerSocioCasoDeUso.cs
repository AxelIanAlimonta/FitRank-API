using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

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

        public async Task<bool> Ejecutar(long adminId, long socioId)
        {
          
            var socio = await _usuarioRepositorio.ObtenerPorIdAsync(socioId);
            if (socio == null)
                throw new Exception("No se encontró el socio seleccionado.");

           /* // 🔹 Evita duplicar notificaciones activas de retención recientes
            var notificacionesPrevias = await _notificacionRepositorio.ObtenerPorUsuarioAsync(socioId);
            bool yaNotificado = notificacionesPrevias.Any(n =>
                n.Activa &&
                n.Titulo.Contains("Retención", StringComparison.OrdinalIgnoreCase) &&
                (DateTime.UtcNow - n.FechaEnvio).TotalDays < 7);

            if (yaNotificado)
                throw new Exception("El socio ya tiene una notificación de retención reciente.");*/

            // 🔹 Crear nueva notificación
            var notificacion = new Notificacion
            {
                UsuarioEmisorId = adminId,
                UsuarioReceptorId = socio.Id,
                Titulo = "Te extrañamos en FitRank",
                Mensaje = $"Hola {socio.Nombre}, notamos que hace varios días no venís al gimnasio. " +
                          $"Podés ajustar tu rutina o hablar con un entrenador para retomar con motivación 💥.",
                Activa = true,
                Leido = false,
                FechaEnvio = DateTime.UtcNow
            };

            await _notificacionRepositorio.AgregarAsync(notificacion);
            return true;
        }
    }
}
