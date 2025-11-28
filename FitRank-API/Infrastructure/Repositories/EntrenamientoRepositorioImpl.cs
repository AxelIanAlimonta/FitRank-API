using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
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
        public async Task<Socio?> ObtenerSocioPorIdAsync(long socioId)
        {
            return await _context.Socios
                .Include(s => s.MedidasCorporales)
                .FirstOrDefaultAsync(s => s.Id == socioId);
        }

        public async Task<Entrenamiento> AgregarAsync(Entrenamiento entrenamiento)
        {
            _context.Entrenamientos.Add(entrenamiento);
            await _context.SaveChangesAsync();
            return entrenamiento;
        }


        public async Task<Entrenamiento?> ObtenerEntrenamientoActivoPorSocioIdAsync(long socioId)
        {
            var todayUtc = DateTime.UtcNow.Date;

            return await _context.Entrenamientos
                .Where(e => e.SocioId == socioId && e.Fecha.Date == todayUtc)
                .OrderByDescending(e => e.Fecha)
                .FirstOrDefaultAsync();
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

        public async Task<bool> EliminarAsync(long id)
        {
            var entrenamiento = await _context.Entrenamientos.FindAsync(id);
            if (entrenamiento == null)
            {
                return false;
            }

            _context.Entrenamientos.Remove(entrenamiento);
            await _context.SaveChangesAsync();
            return true;

        }


        public async Task<List<Entrenamiento>> ObtenerHistorialCompletoPorSocioAsync(long socioId)
        {
            return await _context.Entrenamientos
                .Include(e => e.Actividades)
                    .ThenInclude(a => a.EjercicioAsignado)
                        .ThenInclude(ea => ea.Sesion)
                            .ThenInclude(s => s.Rutina)

                .Include(e => e.Actividades)
                    .ThenInclude(a => a.EjercicioAsignado)
                        .ThenInclude(ea => ea.Ejercicio)
                            .ThenInclude(ex => ex.EjerciciosAsignados)
                                .ThenInclude(eas => eas.Series)

                .Where(e => e.SocioId == socioId)
                .OrderByDescending(e => e.Fecha)
                .ToListAsync();
        }

        public async Task<List<Entrenamiento>> ObtenerHistorialPorProfesorAsync(long profesorId, string? nombre)
        {
            var query = _context.Entrenamientos
                .Include(e => e.Actividades)
                    .ThenInclude(a => a.EjercicioAsignado)
                        .ThenInclude(ea => ea.Sesion)
                            .ThenInclude(s => s.Rutina)
                .Include(e => e.Actividades)
                    .ThenInclude(a => a.EjercicioAsignado)
                        .ThenInclude(ea => ea.Ejercicio)
                            .ThenInclude(ex => ex.EjerciciosAsignados)
                                .ThenInclude(eas => eas.Series)
                .Include(e => e.Socio)
                .Where(e =>
                    e.Actividades.Any(a =>
                        a.EjercicioAsignado.Sesion.Rutina.UsuarioId == profesorId
                    )
                );

            if (!string.IsNullOrWhiteSpace(nombre))
                query = query.Where(e => e.Socio.Nombre.ToLower().Contains(nombre.ToLower()));

            return await query
                .OrderByDescending(e => e.Fecha)
                .ToListAsync();
        }


    }
}