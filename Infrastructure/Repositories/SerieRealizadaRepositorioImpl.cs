using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class SerieRealizadaRepositorioImpl : ISerieRealizadaRepositorio
    {
        private readonly FitRankDbContext _context;
        public SerieRealizadaRepositorioImpl(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<List<SerieRealizada>> ObtenerTodas()
        {
            return await _context.SeriesRealizadas
                .Include(sr => sr.EjercicioRealizado)
                .Include(sr => sr.Puntaje)
                .ToListAsync();
        }
        public async Task<SerieRealizada?> ObtenerPorId(long id)
        {
            return await _context.SeriesRealizadas
                .Include(sr => sr.EjercicioRealizado)
                .Include(sr => sr.Puntaje)
                .FirstOrDefaultAsync(sr => sr.Id == id);
        }
        public async Task<SerieRealizada> Agregar(SerieRealizada serieRealizada)
        {
            _context.SeriesRealizadas.Add(serieRealizada);
            await _context.SaveChangesAsync();
            return serieRealizada;
        }
        public async Task<SerieRealizada?> Actualizar(SerieRealizada serieRealizada)
        {
            var existingSerieRealizada = await _context.SeriesRealizadas.FindAsync(serieRealizada.Id);
            if (existingSerieRealizada == null)
            {
                return null;
            }
            _context.Entry(existingSerieRealizada).CurrentValues.SetValues(serieRealizada);
            await _context.SaveChangesAsync();
            return existingSerieRealizada;
        }
        public async Task<bool> Eliminar(long id)
        {
            var serieRealizada = await _context.SeriesRealizadas.FindAsync(id);
            if (serieRealizada == null)
            {
                return false;
            }
            _context.SeriesRealizadas.Remove(serieRealizada);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
