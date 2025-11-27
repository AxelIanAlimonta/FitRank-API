using AutoMapper;
using FitRank_API.Application.DTOs.Asistencia;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.AsistenciaCasosDeUso
{
    public class ObtenerAsistenciasPorUsuarioCasoDeUso
    {
        private readonly IAsistenciaRepositorio _asistenciaRepositorio;
        private readonly IMapper _mapper;

        public ObtenerAsistenciasPorUsuarioCasoDeUso(
            IAsistenciaRepositorio asistenciaRepositorio,
            IMapper mapper)
        {
            _asistenciaRepositorio = asistenciaRepositorio;
            _mapper = mapper;
        }

        public virtual async Task<List<AsistenciaResponseDTO>> Ejecutar(int usuarioId)
        {
           
            var asistencias = await _asistenciaRepositorio.ObtenerPorUsuarioAsync(usuarioId);

            if (asistencias == null || !asistencias.Any())
                return new List<AsistenciaResponseDTO>();

           
            return _mapper.Map<List<AsistenciaResponseDTO>>(asistencias);
        }
    }
}
