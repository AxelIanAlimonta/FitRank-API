using AutoMapper;
using FitRank_API.Application.DTOs.ReporteDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.ReporteCasosDeUso
{
    public class ObtenerReportesInactivosDeUnGimnasioCasoDeUso
    {
        private readonly IReporteRepositorio _reporteRepositorio;
        private readonly IMapper _mapper;

        public ObtenerReportesInactivosDeUnGimnasioCasoDeUso(IReporteRepositorio reporteRepositorio, IMapper mapper)
        {
            _reporteRepositorio = reporteRepositorio;
            _mapper = mapper;
        }

        public async Task<List<ReporteDTO>> Ejecutar(long gimnasioId)
        {
            var reportes = await _reporteRepositorio.ObtenerReportesInactivosPorGimnasioAsync(gimnasioId);
            return _mapper.Map<List<ReporteDTO>>(reportes);
        }
    }
}
