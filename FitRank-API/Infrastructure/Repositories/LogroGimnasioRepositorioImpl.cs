using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositorios
{
    public class LogroGimnasioRepositorio : ILogroGimnasioRepositorio
    {
        private readonly FitRankDbContext _context;

        public LogroGimnasioRepositorio(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<LogroGimnasio?> ObtenerPorIdAsync(long id)
        {
            return await _context.LogrosGimnasio
                .Include(lg => lg.Logro)
                .AsNoTracking()
                .FirstOrDefaultAsync(lg => lg.Id == id);
        }

        public async Task<LogroGimnasio?> ObtenerPorGimnasioYLogroAsync(long gimnasioId, long logroId)
        {
            return await _context.LogrosGimnasio
                .Include(lg => lg.Logro)
                .FirstOrDefaultAsync(lg =>
                    lg.GimnasioId == gimnasioId &&
                    lg.LogroId == logroId);
        }

        public async Task<IEnumerable<LogroGimnasio>> ObtenerPorGimnasioAsync(long gimnasioId)
        {
            return await _context.LogrosGimnasio
                .Include(lg => lg.Logro)
                .Where(lg => lg.GimnasioId == gimnasioId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<LogroGimnasio> CrearAsync(LogroGimnasio entidad)
        {
            await _context.LogrosGimnasio.AddAsync(entidad);
            await _context.SaveChangesAsync();
            return entidad;
        }

        public async Task<LogroGimnasio?> ActualizarAsync(LogroGimnasio entidad)
        {
            var existente = await _context.LogrosGimnasio
                .FirstOrDefaultAsync(lg => lg.Id == entidad.Id);

            if (existente == null)
                return null;

            _context.Entry(existente).CurrentValues.SetValues(entidad);
            await _context.SaveChangesAsync();
            return existente;
        }
    }
}
