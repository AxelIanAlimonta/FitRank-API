using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class SocioRepositorio : ISocioRepositorio
    {
        private readonly FitRankDbContext _context;
        public SocioRepositorio(FitRankDbContext context)
        {
            _context = context;
        }

        public Task<List<SocioRealizaLogro>> MisLogrosAsync(int socioId)
            =>
            _context.SocioRealizaLogros.AsNoTracking()
                .Where(srl => srl.SocioId == socioId)
                .Include(srl => srl.Logro)
                .OrderByDescending(srl => srl.FechaOtorgado)
                .ToListAsync();
    }
}
