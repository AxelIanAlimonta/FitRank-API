using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class FotoRepositorioImpl : IFotoRepositorio
    {
        private readonly FitRankDbContext _context;
        public FotoRepositorioImpl(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<Foto> AgregarAsync(Foto foto)
        {
            _context.Fotos.Add(foto);
            await _context.SaveChangesAsync();
            return foto;
        }

        public async Task<IEnumerable<Foto>> ObtenerPorSocioAsync(long socioId)
        {
            return await _context.Fotos
                .Where(f => f.SocioId == socioId)
                .OrderByDescending(f => f.Fecha)
                .ToListAsync();
        }

        public async Task<bool> EliminarAsync(long id)
        {
            var foto = await _context.Fotos.FindAsync(id);
            if (foto == null)
            {
                return false;
            }
            _context.Fotos.Remove(foto);
            await _context.SaveChangesAsync();
            return true;

        }
    }
}
