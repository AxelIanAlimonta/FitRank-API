using AutoMapper;
using FitRank_API.Application.DTOs.ReporteDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.ReporteCasosDeUso
{
    public class ObtenerTodosLosReportesDeGimnasioCasoDeUso
    {
        private readonly IReporteRepositorio _reporteRepositorio;
        private readonly IMapper _mapper;

        public ObtenerTodosLosReportesDeGimnasioCasoDeUso(IReporteRepositorio reporteRepositorio, IMapper mapper)
        {
            _reporteRepositorio = reporteRepositorio;
            _mapper = mapper;
        }

        public virtual async Task<List<ReporteDTO>> Ejecutar(long gimnasioId)
        {
            var reportes = await _reporteRepositorio.ObtenerReportesPorGimnasioIdAsync(gimnasioId);
            return _mapper.Map<List<ReporteDTO>>(reportes);
        }
    }
}
