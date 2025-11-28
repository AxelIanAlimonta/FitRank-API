using FitRank_API.Application.DTOs.NotificacionDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.NotificacionCasosDeUso
{
    public class ObtenerHistorialNotificacionesCasoDeUso
    {
        private readonly INotificacionRepositorio _notiRepo;
        private readonly IAdministradorRepositorio _adminRepo;
        private readonly IProfesorRepositorio _profRepo;
        private readonly ISocioRepositorio _socioRepo;

        public ObtenerHistorialNotificacionesCasoDeUso(
            INotificacionRepositorio notiRepo,
            IAdministradorRepositorio adminRepo,
            IProfesorRepositorio profRepo,
            ISocioRepositorio socioRepo)
        {
            _notiRepo = notiRepo;
            _adminRepo = adminRepo;
            _profRepo = profRepo;
            _socioRepo = socioRepo;
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

        public virtual async Task<IEnumerable<HistorialNotificacionDTO>> Ejecutar(long userId)
        {
            var gymId = await ObtenerGimnasioDeUsuario(userId);
            if (gymId == null) return Enumerable.Empty<HistorialNotificacionDTO>();

            // obtener listado
            var socios = await _socioRepo.ObtenerTodosPorGimnasio(gymId.Value);
            var profes = await _profRepo.ObtenerPorGimnasioAsync(gymId.Value);
            var admins = await _adminRepo.ObtenerTodosPorGimnasio(gymId.Value);

            var ids = socios.Select(s => s.Id)
                            .Concat(profes.Select(p => p.Id))
                            .Concat(admins.Select(a => a.Id))
                            .ToList();

            var notificaciones = await _notiRepo.ObtenerNotificacionesDelGimnasio(ids);

            // 🔥 Mapeo manual para evitar loops
            var resultado = notificaciones.Select(n => new HistorialNotificacionDTO
            {
                Id = n.Id,
                Titulo = n.Titulo,
                Mensaje = n.Mensaje,
                FechaCreacion = n.FechaEnvio,
                Emisor = n.UsuarioEmisor?.Nombre ?? "Desconocido",
                Receptor = n.UsuarioReceptor?.Nombre ?? "Desconocido"
            });

            return resultado;
        }



    }
}
