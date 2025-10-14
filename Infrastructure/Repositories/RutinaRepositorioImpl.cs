using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class RutinaRepositorioImpl: IRutinaRepository
    {
        private readonly FitRankDbContext _context;

        public RutinaRepositorioImpl(FitRankDbContext context)
        {
            _context = context;
        }
        //RUTINAS
        public async Task<Rutina> CrearRutinaAsync(Rutina rutina)
        {
            _context.Rutinas.Add(rutina);
            await _context.SaveChangesAsync();
            return rutina;
        }

        public async Task<List<Rutina>> ListarRutinasAsync()
        {
            return await _context.Rutinas
                .Include(b => b.Ejercicios)
                .ToListAsync();
        }

        public async Task<List<Rutina>> ListarRutinasPorUsuarioAsync(int usuarioId)
        {
            return await _context.Rutinas
                .Where(r => r.UsuarioId == usuarioId)
                .Include(b => b.Ejercicios)
                .ToListAsync();
        }

        public async Task<Rutina> ObtenerRutinaPorIdAsync(int rutinaId)
        {
            return await _context.Rutinas
                        .Include(r => r.Ejercicios)
                        .FirstOrDefaultAsync(r => r.Id == rutinaId);
        }

        public async Task<Rutina> ActualizarRutinaAsync(Rutina rutina)
        {
            _context.Rutinas.Update(rutina);
            await _context.SaveChangesAsync();
            return rutina;
        }

        public async Task<bool> EliminarRutinaAsync(int id)
        {
            var rutina = await _context.Rutinas.FindAsync(id);
            if (rutina == null)
            {
                return false;
            }

            _context.Rutinas.Remove(rutina);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
