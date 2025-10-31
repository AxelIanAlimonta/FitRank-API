using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class DiaDeLaSemanaRepositorioImpl : IDiaDeLaSemanaRepositorio
    {
        private readonly FitRankDbContext _context;
        public DiaDeLaSemanaRepositorioImpl(FitRankDbContext context)
        {
            _context = context;
        }
        public async Task<List<DiaDeLaSemana>> ObtenerTodosLosDiasDeLaSemanaAsync()
        {
            return await _context.DiasDeLaSemana.ToListAsync();
        }

        public async Task<DiaDeLaSemana?> ObtenerDiaDeLaSemanaPorIdAsync(long id)
        {
            return await _context.DiasDeLaSemana.FindAsync(id);
        }

        public async Task<DiaDeLaSemana> AgregarDiaDeLaSemanaAsync(DiaDeLaSemana diaDeLaSemana)
        {
            _context.DiasDeLaSemana.Add(diaDeLaSemana);
            await _context.SaveChangesAsync();
            return diaDeLaSemana;
        }

        public async Task<DiaDeLaSemana?> ActualizarDiaDeLaSemanaAsync(DiaDeLaSemana diaDeLaSemana)
        {
            var DiaEncontrado = await _context.DiasDeLaSemana.FindAsync(diaDeLaSemana.Id);
            if (DiaEncontrado == null)
            {
                return null;
            }
            DiaEncontrado.Nombre = diaDeLaSemana.Nombre;
            await _context.SaveChangesAsync();
            return DiaEncontrado;
        }

        public async Task<bool> EliminarDiaDeLaSemanaAsync(long id)
        {
            var dia = await _context.DiasDeLaSemana.FindAsync(id);
            if (dia == null)
            {
                return false;
            }
            _context.DiasDeLaSemana.Remove(dia);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
