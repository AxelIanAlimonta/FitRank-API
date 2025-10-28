using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class ActividadRepositorioImpl : IActividadRepositorio
    {
        private readonly FitRankDbContext _context;

        public ActividadRepositorioImpl(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Actividad>> ObtenerTodasAsync()
        {
            return await _context.Actividades
                .Include(a => a.Serie)
                .Include(a => a.EjercicioAsignado)
                .Include(a => a.Entrenamiento)
                .ToListAsync();
        }

        public async Task<Actividad?> ObtenerPorIdAsync(long id)
        {
            return await _context.Actividades
                .Include(a => a.Serie)
                .Include(a => a.Entrenamiento)
                .Include(a => a.EjercicioAsignado)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Actividad>> ObtenerPorSerieAsync(long serieId)
        {
            return await _context.Actividades
                .Where(a => a.SerieId == serieId)
                .Include(a => a.Entrenamiento)
                .Include(a => a.EjercicioAsignado)
                .ToListAsync();
        }

        public async Task<Actividad> AgregarAsync(Actividad actividad)
        {
            _context.Actividades.Add(actividad);
            await _context.SaveChangesAsync();
            return actividad;
        }

        public async Task ActualizarAsync(Actividad actividad)
        {
            _context.Actividades.Update(actividad);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(long id)
        {
            var act = await _context.Actividades.FindAsync(id);
            if (act != null)
            {
                _context.Actividades.Remove(act);
                await _context.SaveChangesAsync();
            }
        }
    }
}
