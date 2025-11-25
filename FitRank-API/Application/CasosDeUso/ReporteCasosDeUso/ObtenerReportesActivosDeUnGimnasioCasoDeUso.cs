using AutoMapper;
using FitRank_API.Application.DTOs.ReporteDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.ReporteCasosDeUso
{
    public class ObtenerReportesActivosDeUnGimnasioCasoDeUso
    {
        private readonly IReporteRepositorio _reporteRepositorio;
        private readonly IMapper _mapper;

        public ObtenerReportesActivosDeUnGimnasioCasoDeUso(IReporteRepositorio reporteRepositorio, IMapper mapper)
        {
            _reporteRepositorio = reporteRepositorio;
            _mapper = mapper;
        }

        public async Task<List<ReporteDTO>> Ejecutar(long gimnasioId)
        {
            var reportes = await _reporteRepositorio.ObtenerReportesActivosPorGimnasioAsync(gimnasioId);
            return _mapper.Map<List<ReporteDTO>>(reportes);
        }
    }
}
