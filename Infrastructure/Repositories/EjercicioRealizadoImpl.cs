using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{

    public class EjercicioRealizadoImpl : IEjercicioRealizadoRepository

    {
        private readonly FitRankDbContext _context;

        public EjercicioRealizadoImpl(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task AddEjercicioRealizado(EjercicioRealizado ejercicioRealizado)
        {
            _context.EjerciciosRealizados.Add(ejercicioRealizado);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<EjercicioRealizado>> GetByUsuarioAsync(int usuarioId)
        {
            return await _context.EjerciciosRealizados
                .Include(e => e.Ejercicio)
                .Where(e => e.UsuarioId == usuarioId)
                .ToListAsync();
        }
       
    }
}

