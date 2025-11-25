using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface IReporteRepositorio
    {
        Task<Reporte?> ObtenerReportePorIdAsync(long id);

        Task<List<Reporte>> ObtenerReportesPorGimnasioIdAsync(long gimnasioId);

        Task<Reporte> AgregarReporteAsync(Reporte reporte);

        Task<Reporte?> ActualizarReporteAsync(Reporte reporte);

        Task<bool> EliminarReporteAsync(long id);

        Task<List<Reporte>> ObtenerReportesPorUsuarioIdAsync(long usuarioId);

        Task<List<Reporte>> ObtenerReportesActivosPorGimnasioAsync(long gimnasioId);

        Task<List<Reporte>> ObtenerReportesInactivosPorGimnasioAsync(long gimnasioId);
    }
}
