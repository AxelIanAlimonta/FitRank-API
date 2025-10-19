using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class RutinaEjercicioRepositorioImpl : IRutinaEjercicioRepositorio
    {
        private readonly FitRankDbContext _context;
        public RutinaEjercicioRepositorioImpl(FitRankDbContext context)
        {
            _context = context;
        }
        public async Task<List<RutinaEjercicio>> ObtenerTodos()
        {
            return await _context.RutinasEjercicios.Include(re => re.Ejercicio).Include(re => re.Rutina).ToListAsync();
        }
        public async Task<RutinaEjercicio?> ObtenerPorId(long id)
        {
            return await _context.RutinasEjercicios.Include(re => re.Ejercicio).Include(re => re.Rutina).FirstOrDefaultAsync(re => re.Id == id);
        }
        public async Task<RutinaEjercicio> Crear(RutinaEjercicio rutinaEjercicio)
        {
            _context.RutinasEjercicios.Add(rutinaEjercicio);
            await _context.SaveChangesAsync();
            return rutinaEjercicio;
        }
        public async Task<RutinaEjercicio?> Actualizar(RutinaEjercicio rutinaEjercicio)
        {
            var existente = await _context.RutinasEjercicios.FindAsync(rutinaEjercicio.Id);
            if (existente == null) return null;
            existente.RutinaId = rutinaEjercicio.RutinaId;
            existente.EjercicioId = rutinaEjercicio.EjercicioId;
            existente.NumeroDeSesion = rutinaEjercicio.NumeroDeSesion;
            existente.Orden = rutinaEjercicio.Orden;
            await _context.SaveChangesAsync();
            return existente;
        }
        public async Task<bool> Eliminar(long id)
        {
            var rutinaEjercicio = await _context.RutinasEjercicios.FindAsync(id);
            if (rutinaEjercicio == null) return false;
            _context.RutinasEjercicios.Remove(rutinaEjercicio);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
