using AutoMapper;
using FitRank_API.Application.DTOs.ReporteDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.ReporteCasosDeUso
{
    public class ObtenerReportesPorUsuarioCasoDeUso
    {
        private readonly IReporteRepositorio _reporteRepositorio;
        private readonly IMapper _mapper;

        public ObtenerReportesPorUsuarioCasoDeUso(IReporteRepositorio reporteRepositorio, IMapper mapper)
        {
            _reporteRepositorio = reporteRepositorio;
            _mapper = mapper;
        }

        public async Task<List<ReporteDTO>> Ejecutar(long usuarioId)
        {
            var reportes = await _reporteRepositorio.ObtenerReportesPorUsuarioIdAsync(usuarioId);
            return _mapper.Map<List<ReporteDTO>>(reportes);
        }
    }
}
