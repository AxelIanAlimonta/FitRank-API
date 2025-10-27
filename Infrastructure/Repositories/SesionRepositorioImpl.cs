using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
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
            return sesion;
        }

        public async Task<Sesion?> ActualizarAsync(long id, Sesion sesion)
        {
            var sesionExistente = await _context.Sesiones.FirstOrDefaultAsync(x => x.Id == id);
            if (sesionExistente is null)
                return null;
            if (sesionExistente == null) return null;

            sesionExistente.Nombre = sesion.Nombre;
            sesionExistente.NumeroDeSesion = sesion.NumeroDeSesion;
            sesionExistente.RutinaId = sesion.RutinaId;

            await _context.SaveChangesAsync();
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
