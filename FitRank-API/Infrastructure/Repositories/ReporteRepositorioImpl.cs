using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class ReporteRepositorioImpl : IReporteRepositorio
    {
        private readonly FitRankDbContext _context;

        public ReporteRepositorioImpl(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<Reporte?> ObtenerReportePorIdAsync(long id)
        {
            return await _context.Reportes.FindAsync(id);
        }
        public async Task<List<Reporte>> ObtenerReportesPorGimnasioIdAsync(long gimnasioId)
        {
            return await _context.Reportes
                .Where(r => r.GimnasioId == gimnasioId)
                .OrderByDescending(r => r.FechaCreacion)
                .ToListAsync();
        }
        public async Task<Reporte> AgregarReporteAsync(Reporte reporte)
        {
            _context.Reportes.Add(reporte);
            await _context.SaveChangesAsync();
            return reporte;
        }
        public async Task<Reporte?> ActualizarReporteAsync(Reporte reporte)
        {
            var reporteExistente = await _context.Reportes.FindAsync(reporte.Id);

            if (reporteExistente == null)
            {
                return null;
            }

            _context.Entry(reporteExistente).CurrentValues.SetValues(reporte);

            await _context.SaveChangesAsync();

            return reporteExistente;
        }

        public async Task<bool> EliminarReporteAsync(long id)
        {
            var reporte = await _context.Reportes.FindAsync(id);

            if (reporte == null)
            {
                return false;
            }

            _context.Reportes.Remove(reporte);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<Reporte>> ObtenerReportesPorUsuarioIdAsync(long usuarioId)
        {
            return await _context.Reportes
                .Where(r => r.UsuarioId == usuarioId)
                .OrderByDescending(r => r.FechaCreacion)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Reporte>> ObtenerReportesActivosPorGimnasioAsync(long gimnasioId)
        {
            return await _context.Reportes
                .Where(r => r.GimnasioId == gimnasioId && r.Activo)
                .OrderByDescending(r => r.FechaCreacion)
                .ToListAsync();
        }

        public async Task<List<Reporte>> ObtenerReportesInactivosPorGimnasioAsync(long gimnasioId)
        {
            return await _context.Reportes
                .Where(r => r.GimnasioId == gimnasioId && !r.Activo)
                .OrderByDescending(r => r.FechaCreacion)
                .ToListAsync();
        }


    }
}
