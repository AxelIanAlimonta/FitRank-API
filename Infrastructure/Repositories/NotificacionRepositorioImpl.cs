using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
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
            if (notificacion == null) return;

            notificacion.Leido = true;
            _context.Notificaciones.Update(notificacion);
            await _context.SaveChangesAsync();
        }

        public async Task DesactivarAsync(long id)
        {
            var notificacion = await _context.Notificaciones.FindAsync(id);
            if (notificacion == null) return;

            notificacion.Activa = false;
            _context.Notificaciones.Update(notificacion);
            await _context.SaveChangesAsync();
        }
    }
}
