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

        public async Task<Entrenamiento> AgregarAsync(Entrenamiento entrenamiento)
        {
            _context.Entrenamientos.Add(entrenamiento);
            await _context.SaveChangesAsync();
            return entrenamiento;
        }

        public async Task<Entrenamiento?> ActualizarAsync(Entrenamiento entrenamiento)
        {
            var entrenamientoExistente = await _context.Entrenamientos.FindAsync(entrenamiento.Id);
            if (entrenamientoExistente == null)
            {
                return null;
            }

            entrenamientoExistente.Fecha = entrenamiento.Fecha;
            entrenamientoExistente.Duracion = entrenamiento.Duracion;
            entrenamientoExistente.SocioId = entrenamiento.SocioId;

            await _context.SaveChangesAsync();
            return entrenamientoExistente;
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
    }
}
