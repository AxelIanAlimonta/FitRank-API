using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class ActividadRepositorioImpl : IActividadRepositorio
    {
        private readonly FitRankDbContext _context;

        public ActividadRepositorioImpl(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Actividad>> ObtenerTodasAsync()
        {
            return await _context.Actividades
                .Include(a => a.Serie)
                .Include(a => a.EjercicioAsignado)
                .Include(a => a.Entrenamiento)
                    .ThenInclude(a => a.Socio)
                .ToListAsync();
        }

        public async Task<Actividad?> ObtenerPorIdAsync(long id)
        {
            return await _context.Actividades
                .Include(a => a.Serie)
                .Include(a => a.Entrenamiento)
                    .ThenInclude(a => a.Socio)
                .Include(a => a.EjercicioAsignado)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Actividad>> ObtenerPorSerieAsync(long serieId)
        {
            return await _context.Actividades
                .Where(a => a.SerieId == serieId)
                .Include(a => a.Entrenamiento)
                    .ThenInclude(a => a.Socio)
                .Include(a => a.EjercicioAsignado)
                .ToListAsync();
        }

        public async Task<Actividad> AgregarAsync(Actividad actividad)
        {
            _context.Actividades.Add(actividad);
            await _context.SaveChangesAsync();
            return actividad;
        }

        public async Task<IEnumerable<Actividad>> ObtenerPorEntrenamientoAsync(long entrenamientoId)
        {
            return await _context.Actividades
                                 .Include(a => a.Serie)
                                 .Include(a => a.EjercicioAsignado)
                                 .Where(a => a.EntrenamientoId == entrenamientoId)
                                 .ToListAsync();
        }

        public async Task<Serie> ObtenerSeriePorIdAsync(long serieId)
        {
            return await _context.Series
                .Include(s => s.EjercicioAsignado)
                .ThenInclude(ea => ea.Ejercicio)
                .ThenInclude(e => e.GrupoMuscular)
                .FirstAsync(s => s.Id == serieId);
        }
        
        public async Task<Actividad?> ActualizarAsync(Actividad actividad)
        {
            var actividadExistente = await _context.Actividades.FindAsync(actividad.Id);
            if (actividadExistente == null)
            {
                return null;
            }

            actividadExistente.Repeticiones = actividad.Repeticiones;
            actividadExistente.Peso = actividad.Peso;
            actividadExistente.Punto = actividad.Punto;
            actividadExistente.SerieId = actividad.SerieId;
            actividadExistente.EjercicioAsignadoId = actividad.EjercicioAsignadoId;
            actividadExistente.EntrenamientoId = actividad.EntrenamientoId;

            await _context.SaveChangesAsync();
            return actividadExistente;

        }

        public async Task<bool> EliminarAsync(long id)
        {
            var actividadExistente = await _context.Actividades.FindAsync(id);
            if (actividadExistente == null)
            {
                return false;
            }

            _context.Actividades.Remove(actividadExistente);
            await _context.SaveChangesAsync();
            return true;

        }
    }
}
