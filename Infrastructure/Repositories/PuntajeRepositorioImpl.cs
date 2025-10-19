using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class PuntajeRepositorioImpl : IPuntajeRepositorio
    {
        private readonly FitRankDbContext _context;
        public PuntajeRepositorioImpl(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<List<Puntaje>> ObtenerTodas()
        {
            return await _context.Puntajes
                .Include(p => p.SerieRealizada)
                .ToListAsync();
        }
        public async Task<Puntaje?> ObtenerPorId(long id)
        {
            return await _context.Puntajes
                .Include(p => p.SerieRealizada)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<Puntaje> Agregar(Puntaje puntaje)
        {
            _context.Puntajes.Add(puntaje);
            await _context.SaveChangesAsync();
            return puntaje;
        }
        public async Task<Puntaje?> Actualizar(Puntaje puntaje)
        {
            var existingPuntaje = await _context.Puntajes.FindAsync(puntaje.Id);
            if (existingPuntaje == null)
            {
                return null;
            }
            _context.Entry(existingPuntaje).CurrentValues.SetValues(puntaje);
            await _context.SaveChangesAsync();
            return existingPuntaje;
        }
        public async Task<bool> Eliminar(long id)
        {
            var puntaje = await _context.Puntajes.FindAsync(id);
            if (puntaje == null)
            {
                return false;
            }
            _context.Puntajes.Remove(puntaje);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
