
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Application.CasosDeUso.SocioCasosDeUso
{
    public class BorrarSocioCompletoCasoDeUso
    {
        private readonly FitRankDbContext _context;

        public BorrarSocioCompletoCasoDeUso(FitRankDbContext context)
        {
            _context = context;
        }

        public virtual async Task<string> Ejecutar(long usuarioId)
        {
            // ====== 1. INVITACIONES ======
            var invitaciones = await _context.Invitaciones
                .Where(i => i.UsuarioId == usuarioId)
                .ToListAsync();

            if (invitaciones.Any())
                _context.Invitaciones.RemoveRange(invitaciones);

            // ====== 2. ASISTENCIAS ======
            var asistencias = await _context.Asistencias
                .Where(a => a.UsuarioId == usuarioId)
                .ToListAsync();

            if (asistencias.Any())
                _context.Asistencias.RemoveRange(asistencias);

            // ====== 3. SOCIO ======
            var socio = await _context.Socios
                .FirstOrDefaultAsync(s => s.Id == usuarioId);

            if (socio != null)
                _context.Socios.Remove(socio);

            // ====== 4. USUARIO BASE ======
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario != null)
                _context.Usuarios.Remove(usuario);

            // ====== 5. GUARDAR ======
            await _context.SaveChangesAsync();

            return $"✔ Usuario {usuarioId} eliminado completamente (socios + asistencias + invitaciones).";
        }
    }
}
