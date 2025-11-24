using AutoMapper;
using FitRank_API.Application.DTOs.ReporteDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.ReporteCasosDeUso
{
    public class ObtenerReportePorIdCasoDeUso
    {
        private readonly IReporteRepositorio _reporteRepositorio;
        private readonly IMapper _mapper;

        public ObtenerReportePorIdCasoDeUso(IReporteRepositorio reporteRepositorio, IMapper mapper)
        {
            _reporteRepositorio = reporteRepositorio;
            _mapper = mapper;
        }

        public virtual async Task<ReporteDTO?> Ejecutar(long id)
        {
            var reporte = await _reporteRepositorio.ObtenerReportePorIdAsync(id);

            if (reporte == null)
            {
                return null;
            }

            return _mapper.Map<ReporteDTO>(reporte);
        }
    }
}
