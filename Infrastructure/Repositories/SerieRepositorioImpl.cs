using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class SerieRepositorioImpl : ISerieRepositorio
    {
        private readonly FitRankDbContext _context;
        public SerieRepositorioImpl(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Serie>> ObtenerTodasAsync()
        {
            return await _context.Series
                .Include(s => s.Actividades)
                .Include(s => s.EjercicioAsignado)
                .ToListAsync();
        }

        public async Task<Serie?> ObtenerPorIdAsync(long id)
        {
            return await _context.Series
                .Include(s => s.Actividades)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Serie>> ObtenerPorEjercicioAsync(long ejercicioAsignadoId)
        {
            return await _context.Series
                .Where(s => s.EjercicioAsignadoId == ejercicioAsignadoId)
                .Include(s => s.Actividades)
                .ToListAsync();
        }

        public async Task<Serie> AgregarAsync(Serie serie)
        {
            _context.Series.Add(serie);
            await _context.SaveChangesAsync();
            return serie;
        }

        public async Task<Serie?> ActualizarAsync(Serie serie)
        {
            var serieExistente = await _context.Series.FindAsync(serie.Id);
            if (serieExistente == null)
            {
                return null;
            }

            serieExistente.NumeroDeSerie = serie.NumeroDeSerie;
            serieExistente.Duracion = serie.Duracion;
            serieExistente.Repeticiones = serie.Repeticiones;
            serieExistente.Peso = serie.Peso;
            serieExistente.EjercicioAsignadoId = serie.EjercicioAsignadoId;


            _context.Series.Update(serieExistente);
            await _context.SaveChangesAsync();
            return serieExistente;

        }

        public async Task<bool> EliminarAsync(long id)
        {
            var serieExistente = await _context.Series.FindAsync(id);
            if (serieExistente == null)
            {
                return false;
            }

            _context.Series.Remove(serieExistente);
            await _context.SaveChangesAsync();
            return true;

        }
    }
}
