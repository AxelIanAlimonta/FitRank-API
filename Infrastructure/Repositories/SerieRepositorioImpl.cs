using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class SerieRepositorioImpl : ISerieRepositorio
    {
        private readonly FitRankDbContext _context;
        public SerieRepositorioImpl(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Serie>> ObtenerTodasAsync()
        {
            return await _context.Series
                .Include(s => s.Actividades)
                .Include(s => s.EjercicioAsignado)
                .ToListAsync();
        }

        public async Task<Serie?> ObtenerPorIdAsync(long id)
        {
            return await _context.Series
                .Include(s => s.Actividades)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Serie>> ObtenerPorEjercicioAsync(long ejercicioAsignadoId)
        {
            return await _context.Series
                .Where(s => s.EjercicioAsignadoId == ejercicioAsignadoId)
                .Include(s => s.Actividades)
                .ToListAsync();
        }

        public async Task<Serie> AgregarAsync(Serie serie)
        {
            _context.Series.Add(serie);
            await _context.SaveChangesAsync();
            return serie;
        }

        public async Task ActualizarAsync(Serie serie)
        {
            _context.Series.Update(serie);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(long id)
        {
            var serie = await _context.Series.FindAsync(id);
            if (serie != null)
            {
                _context.Series.Remove(serie);
                await _context.SaveChangesAsync();
            }
        }
    }
}
