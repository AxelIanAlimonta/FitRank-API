using AutoMapper;
using FitRank_API.Application.DTOs.ReporteDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.ReporteCasosDeUso
{
    public class AgregarReporteCasoDeUso
    {
        private readonly IReporteRepositorio _reporteRepositorio;
        private readonly IMapper _mapper;

        public AgregarReporteCasoDeUso(IReporteRepositorio reporteRepositorio, IMapper mapper)
        {
            _reporteRepositorio = reporteRepositorio;
            _mapper = mapper;
        }

        public virtual async Task<ReporteDTO?> Ejecutar(AgregarReporteDTO agregarReporteDTO)
        {
            var reporteEntidad = _mapper.Map<Reporte>(agregarReporteDTO);
            var reporteCreado = await _reporteRepositorio.AgregarReporteAsync(reporteEntidad);
            return _mapper.Map<ReporteDTO?>(reporteCreado);
        }
    }
}
