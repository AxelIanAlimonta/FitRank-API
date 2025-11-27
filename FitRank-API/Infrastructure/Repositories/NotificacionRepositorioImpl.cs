using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class NotificacionRepositorioImpl : INotificacionRepositorio
    {
        private readonly FitRankDbContext _context;

        public NotificacionRepositorioImpl(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<Notificacion> AgregarAsync(Notificacion notificacion)
        {
            _context.Notificaciones.Add(notificacion);
            await _context.SaveChangesAsync();
            return notificacion;
        }

        public async Task<IEnumerable<Notificacion>> ObtenerPorUsuarioAsync(long usuarioId)
        {
            return await _context.Notificaciones
                .Where(n => n.UsuarioReceptorId == usuarioId && n.Activa)
                .Include(n => n.UsuarioEmisor)
                .OrderByDescending(n => n.FechaEnvio)
                .ToListAsync();
        }

        public async Task<Notificacion?> ObtenerPorIdAsync(long id)
        {
            return await _context.Notificaciones
                .Include(n => n.UsuarioEmisor)
                .Include(n => n.UsuarioReceptor)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task MarcarComoLeidaAsync(long id)
        {
            var notificacion = await _context.Notificaciones.FindAsync(id);
            if (notificacion != null)
            {
                notificacion.Leido = true;
                _context.Notificaciones.Update(notificacion);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DesactivarAsync(long id)
        {
            var notificacion = await _context.Notificaciones.FindAsync(id);
            if (notificacion != null)
            {
                notificacion.Activa = false;
                _context.Notificaciones.Update(notificacion);
                await _context.SaveChangesAsync();
            }

        }




        public async Task<Notificacion?> ActualizarAsync(Notificacion notificacion)
        {
            var notificacionExistente = await _context.Notificaciones.FindAsync(notificacion.Id);
            if (notificacionExistente == null)
            {
                return null;
            }
            _context.Entry(notificacionExistente).CurrentValues.SetValues(notificacion);
            await _context.SaveChangesAsync();
            return notificacionExistente;
        }


        public async Task<long?> ObtenerGimnasioIdDeUsuario(long userId)
        {

            var admin = await _context.Administradores
                .FirstOrDefaultAsync(a => a.Id == userId);
            if (admin != null) return admin.GimnasioId;

            var profesor = await _context.Profesores
                .FirstOrDefaultAsync(p => p.Id == userId);
            if (profesor != null) return profesor.GimnasioId;

            var socio = await _context.Socios
                .FirstOrDefaultAsync(s => s.Id == userId);
            if (socio != null) return socio.GimnasioId;

            return null;
        }

        public async Task<List<Usuario>> ObtenerUsuariosDelGimnasio(long gimnasioId)
        {
            var socios = await _context.Socios
                .Where(s => s.GimnasioId == gimnasioId)
                .ToListAsync<Usuario>();

            var profes = await _context.Profesores
                .Where(p => p.GimnasioId == gimnasioId)
                .ToListAsync<Usuario>();

            var admins = await _context.Administradores
                .Where(a => a.GimnasioId == gimnasioId)
                .ToListAsync<Usuario>();

            return socios.Concat(profes).Concat(admins).ToList();
        }

        public async Task EnviarNotificacionGlobal(long adminId, string titulo, string mensaje)
        {
            var gymId = await ObtenerGimnasioIdDeUsuario(adminId);

            var usuarios = await ObtenerUsuariosDelGimnasio(gymId.Value);

            foreach (var u in usuarios)
            {
                var notificacion = new Notificacion
                {
                    Titulo = titulo,
                    Mensaje = mensaje,
                    UsuarioEmisorId = adminId,
                    UsuarioReceptorId = u.Id,
                    FechaEnvio = DateTime.UtcNow
                };

                _context.Notificaciones.Add(notificacion);
            }

            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Notificacion>> ObtenerNotificacionesDelGimnasioadmin(long adminId)
        {
            var gymId = await ObtenerGimnasioIdDeUsuario(adminId);
            if (gymId == null) return Enumerable.Empty<Notificacion>();

            var usuarios = await ObtenerUsuariosDelGimnasio(gymId.Value);
            var userIds = usuarios.Select(u => u.Id).ToList();

            return await _context.Notificaciones
                .Where(n => userIds.Contains(n.UsuarioReceptorId))
                .Include(n => n.UsuarioEmisor)
                .OrderByDescending(n => n.FechaEnvio)
                .ToListAsync();
        }
        public async Task<IEnumerable<Notificacion>> ObtenerNotificacionesDelGimnasio(IEnumerable<long> usuarioIds)
        {
            return await _context.Notificaciones
                .Where(n =>
                    usuarioIds.Contains(n.UsuarioEmisorId) ||
                    usuarioIds.Contains(n.UsuarioReceptorId)
                )
                .Include(n => n.UsuarioEmisor)
                .Include(n => n.UsuarioReceptor)
                .OrderByDescending(n => n.FechaEnvio)
                .ToListAsync();
        }
    }
}