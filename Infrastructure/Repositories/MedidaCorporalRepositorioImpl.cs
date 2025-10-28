using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class MedidaCorporalRepositorioImpl : IMedidaCorporalRepositorio
    {
        private readonly FitRankDbContext _context;

        public MedidaCorporalRepositorioImpl(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<MedidaCorporal> AgregarAsync(MedidaCorporal medida)
        {
            _context.MedidasCorporales.Add(medida);
            await _context.SaveChangesAsync();
            return medida;
        }

        public async Task<MedidaCorporal?> ObtenerPorIdAsync(long id)
        {
            return await _context.MedidasCorporales
                .Include(m => m.Socio)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<List<MedidaCorporal>> ObtenerPorSocioAsync(long socioId)
        {
            return await _context.MedidasCorporales
                .Where(m => m.SocioId == socioId)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();
        }

        public async Task<MedidaCorporal?> ActualizarAsync(MedidaCorporal medida)
        {
            var existente = await _context.MedidasCorporales.FindAsync(medida.Id);
            if (existente == null) return null;

            _context.Entry(existente).CurrentValues.SetValues(medida);
            await _context.SaveChangesAsync();
            return existente;
        }

        public async Task<bool> EliminarAsync(MedidaCorporal medida)
        {
            var medidaExistente = await _context.MedidasCorporales.FindAsync(medida.Id);
            if (medidaExistente == null)
            {
                return false;
            }
            _context.MedidasCorporales.Remove(medidaExistente);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
