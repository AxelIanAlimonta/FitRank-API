using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class SesionRepositorioImpl : ISesionRepositorio
    {
        private readonly FitRankDbContext _context;
        public SesionRepositorioImpl(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<List<Sesion>> ObtenerTodasAsync()
        {
            return await _context.Sesiones.Include(s => s.Rutina).ToListAsync();
        }

        public async Task<Sesion?> ObtenerPorIdAsync(long id)
        {
            return await _context.Sesiones.Include(s => s.Rutina)
                                          .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Sesion> AgregarAsync(Sesion sesion)
        {
            _context.Sesiones.Add(sesion);
            await _context.SaveChangesAsync();

            var sesionDetallada = await _context.Sesiones
                .Include(s => s.Rutina)
                .FirstOrDefaultAsync(s => s.Id == sesion.Id);
            return sesionDetallada;
        }

        public async Task<Sesion?> ActualizarAsync(Sesion sesion)
        {
            var sesionExistente = await _context.Sesiones.FindAsync(sesion.Id);
            if (sesionExistente == null) return null;

            sesionExistente.NumeroDeSesion = sesion.NumeroDeSesion;
            sesionExistente.Nombre = sesion.Nombre;
            sesionExistente.RutinaId = sesion.RutinaId;

            _context.Sesiones.Update(sesionExistente);
            await _context.SaveChangesAsync();

            await _context.Entry(sesionExistente)
                .Reference(s => s.Rutina)
                .LoadAsync();

            return sesionExistente;
        }

        public async Task<bool> EliminarAsync(long id)
        {
            var sesionExistente = await _context.Sesiones.FindAsync(id);
            if (sesionExistente == null) return false;

            _context.Sesiones.Remove(sesionExistente);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
