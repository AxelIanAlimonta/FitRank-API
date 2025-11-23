using AutoMapper;
using FitRank_API.Application.DTOs.ReporteDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.ReporteCasosDeUso
{
    public class ActualizarReporteCasoDeUso
    {
        private readonly IReporteRepositorio _reporteRepositorio;
        private readonly IMapper _mapper;

        public ActualizarReporteCasoDeUso(IReporteRepositorio reporteRepositorio, IMapper mapper)
        {
            _reporteRepositorio = reporteRepositorio;
            _mapper = mapper;
        }

        public virtual async Task<ReporteDTO?> Ejecutar(ActualizarReporteDTO actualizarReporteDTO)
        {
            var reporteExistente = await _reporteRepositorio.ObtenerReportePorIdAsync(actualizarReporteDTO.Id);

            if (reporteExistente == null)
                return null;

            // Solo actualizamos campos permitidos
            reporteExistente.Titulo = actualizarReporteDTO.Titulo;
            reporteExistente.Descripcion = actualizarReporteDTO.Descripcion;

            var actualizado = await _reporteRepositorio.ActualizarReporteAsync(reporteExistente);

            return _mapper.Map<ReporteDTO>(actualizado);
        }
    }
}
