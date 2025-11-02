using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class EntrenamientoRepositorioImpl : IEntrenamientoRepositorio
    {
        private readonly FitRankDbContext _context;

        public EntrenamientoRepositorioImpl(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Entrenamiento>> ObtenerTodosAsync()
        {
            return await _context.Entrenamientos
                .Include(e => e.Socio)
                .Include(e => e.Actividades)
                .ToListAsync();
        }

        public async Task<Entrenamiento?> ObtenerPorIdAsync(long id)
        {
            return await _context.Entrenamientos
                .Include(e => e.Socio)
                .Include(e => e.Actividades)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Entrenamiento>> ObtenerPorSocioAsync(long socioId)
        {
            return await _context.Entrenamientos
                .Where(e => e.SocioId == socioId)
                .Include(e => e.Actividades)
                .ToListAsync();
        }
        public async Task<Socio?> ObtenerSocioPorIdAsync(long socioId)
        {
            return await _context.Socios
                .Include(s => s.MedidasCorporales)
                .FirstOrDefaultAsync(s => s.Id == socioId);
        }

        public async Task<Entrenamiento> AgregarAsync(Entrenamiento entrenamiento)
        {
            _context.Entrenamientos.Add(entrenamiento);
            await _context.SaveChangesAsync();
            return entrenamiento;
        }

        public async Task ActualizarAsync(Entrenamiento entrenamiento)
        {
            _context.Entrenamientos.Update(entrenamiento);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(long id)
        {
            var ent = await _context.Entrenamientos.FindAsync(id);
            if (ent != null)
            {
                _context.Entrenamientos.Remove(ent);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Entrenamiento?> ObtenerEntrenamientoActivoPorSocioIdAsync(long socioId)
        {
            var todayUtc = DateTime.UtcNow.Date;

            return await _context.Entrenamientos
                .Where(e => e.SocioId == socioId && e.Fecha.Date == todayUtc)
                .OrderByDescending(e => e.Fecha)
                .FirstOrDefaultAsync();
        }
    }
}
