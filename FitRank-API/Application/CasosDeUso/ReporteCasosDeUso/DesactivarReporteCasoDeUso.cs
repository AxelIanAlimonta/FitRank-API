using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.ReporteCasosDeUso
{
    public class DesactivarReporteCasoDeUso
    {
        private readonly IReporteRepositorio _reporteRepositorio;
        public DesactivarReporteCasoDeUso(IReporteRepositorio reporteRepositorio)
        {
            _reporteRepositorio = reporteRepositorio;
        }

        public virtual async Task<bool> Ejecutar(long id)
        {
            var reporte = await _reporteRepositorio.ObtenerReportePorIdAsync(id);

            if (reporte == null)
            {
                return false;
            }

            reporte.Activo = false;
            await _reporteRepositorio.ActualizarReporteAsync(reporte);

            return true;
        }
    }
}
