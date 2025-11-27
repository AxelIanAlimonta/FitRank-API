using AutoMapper;
using FitRank_API.Application.DTOs.Asistencia;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.AsistenciaCasosDeUso
{
    public class ObtenerTodasLasAsistenciasCasoDeUso
    {
        private readonly IAsistenciaRepositorio _asistenciaRepositorio;
        private readonly IMapper _mapper;

        public ObtenerTodasLasAsistenciasCasoDeUso(
            IAsistenciaRepositorio asistenciaRepositorio,
            IMapper mapper)
        {
            _asistenciaRepositorio = asistenciaRepositorio;
            _mapper = mapper;
        }

        public virtual async Task<List<AsistenciaListadoDTO>> Ejecutar()
        {
            var asistencias = await _asistenciaRepositorio.ObtenerTodasConUsuarioAsync();
            return _mapper.Map<List<AsistenciaListadoDTO>>(asistencias);
        }
    }
}
