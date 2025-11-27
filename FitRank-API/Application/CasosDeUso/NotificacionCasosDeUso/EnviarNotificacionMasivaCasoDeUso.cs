using FitRank_API.Application.Hubs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using MercadoPago.Resource.User;
using Microsoft.AspNetCore.SignalR;


namespace FitRank_API.Application.CasosDeUso.NotificacionCasosDeUso
{
    public class EnviarNotificacionMasivaCasoDeUso
    {
        private readonly INotificacionRepositorio _notiRepo;
        private readonly IAdministradorRepositorio _adminRepo;
        private readonly IProfesorRepositorio _profRepo;
        private readonly ISocioRepositorio _socioRepo;
        private readonly IHubContext<NotificacionesHub> _hub;

        public EnviarNotificacionMasivaCasoDeUso(
             INotificacionRepositorio notiRepo,
             IAdministradorRepositorio adminRepo,
             IProfesorRepositorio profRepo,
             ISocioRepositorio socioRepo,
             IHubContext<NotificacionesHub> hub)
        {
            _notiRepo = notiRepo;
            _adminRepo = adminRepo;
            _profRepo = profRepo;
            _socioRepo = socioRepo;
            _hub = hub;
        }

        private async Task<long?> ObtenerGimnasioDeUsuario(long userId)
        {
            var admin = await _adminRepo.ObtenerPorIdAsync(userId);
            if (admin != null) return admin.GimnasioId;

            var profesor = await _profRepo.ObtenerPorIdAsync(userId);
            if (profesor != null) return profesor.GimnasioId;

            var socio = await _socioRepo.ObtenerPorIdAsync(userId);
            if (socio != null) return socio.GimnasioId;

            return null;
        }

        public virtual async Task<int> Ejecutar(long emisorId, string titulo, string mensaje)
        {
            var gymId = await ObtenerGimnasioDeUsuario(emisorId);
            if (gymId == null)
                throw new Exception("No se pudo determinar el gimnasio del usuario.");

            var socios = (await _socioRepo.ObtenerTodosPorGimnasio(gymId.Value))
                .Select(s => s.Id)
                .ToList();

            var profes = (await _profRepo.ObtenerPorGimnasioAsync(gymId.Value))
                .Select(p => p.Id)
                .ToList();

            var admins = (await _adminRepo.ObtenerTodosPorGimnasio(gymId.Value))
                .Select(a => a.Id)
                .ToList();

            var usuarios = socios.Concat(profes).Concat(admins).ToList();

            int count = 0;

            foreach (var id in usuarios)
            {
                var noti = new Notificacion
                {
                    UsuarioEmisorId = emisorId,
                    UsuarioReceptorId = id,
                    Titulo = titulo,
                    Mensaje = mensaje,
                    FechaEnvio = DateTime.UtcNow
                };

                await _notiRepo.AgregarAsync(noti);

                count++;

                // ⭐ EMITIR AL USUARIO POR SIGNALR
                await _hub.Clients.Group($"user-{id}")
                    .SendAsync("NotificacionRecibida", new
                    {
                        id = noti.Id,
                        titulo = noti.Titulo,
                        mensaje = noti.Mensaje,
                        fechaCreacion = noti.FechaEnvio
                    });
            }

            return count;
        }
    }

}