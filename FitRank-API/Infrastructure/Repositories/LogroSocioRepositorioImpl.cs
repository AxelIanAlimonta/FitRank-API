using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositorios
{
    public class LogroSocioRepositorio : ILogroSocioRepositorio
    {
        private readonly FitRankDbContext _context;

        public LogroSocioRepositorio(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<LogroSocio?> ObtenerPorIdAsync(long id)
        {
            return await _context.LogrosSocio
                .Include(ls => ls.Logro)
                .AsNoTracking()
                .FirstOrDefaultAsync(ls => ls.Id == id);
        }

        public async Task<bool> ExisteAsync(long logroId, long gimnasioId, long socioId)
        {
            return await _context.LogrosSocio.AnyAsync(ls =>
                ls.LogroId == logroId &&
                ls.GimnasioId == gimnasioId &&
                ls.SocioId == socioId);
        }



        public async Task<LogroSocio> CrearAsync(LogroSocio logroSocio)
        {
            await _context.LogrosSocio.AddAsync(logroSocio);
            await _context.SaveChangesAsync();
            return logroSocio;
        }

        public async Task<IEnumerable<LogroSocio>> ObtenerPorSocioYGimnasioAsync(long socioId, long gimnasioId)
        {
            return await _context.LogrosSocio
                .Include(ls => ls.Logro)
                .Where(ls => ls.SocioId == socioId && ls.GimnasioId == gimnasioId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<LogroSocio>> ObtenerPorSocioAsync(long socioId)
        {
            return await _context.LogrosSocio
                .Include(ls => ls.Logro)
                .Where(ls => ls.SocioId == socioId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
