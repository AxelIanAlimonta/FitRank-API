using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.ReporteCasosDeUso
{
    public class EliminarReporteCasoDeUso
    {
        private readonly IReporteRepositorio _reporteRepositorio;

        public EliminarReporteCasoDeUso(IReporteRepositorio reporteRepositorio)
        {
            _reporteRepositorio = reporteRepositorio;
        }

        public virtual async Task<bool> Ejecutar(long id)
        {
            return await _reporteRepositorio.EliminarReporteAsync(id);
        }
    }
}
