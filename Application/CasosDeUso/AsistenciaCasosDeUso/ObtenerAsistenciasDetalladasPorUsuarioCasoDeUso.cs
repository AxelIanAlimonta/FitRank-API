using AutoMapper;
using FitRank_API.Application.DTOs.Asistencia;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.AsistenciaCasosDeUso
{
    public class ObtenerAsistenciasDetalladasPorUsuarioCasoDeUso
    {
        private readonly IAsistenciaRepositorio _asistenciaRepositorio;
        private readonly IMapper _mapper;

        public ObtenerAsistenciasDetalladasPorUsuarioCasoDeUso(
            IAsistenciaRepositorio asistenciaRepositorio,
            IMapper mapper)
        {
            _asistenciaRepositorio = asistenciaRepositorio;
            _mapper = mapper;
        }

        public async Task<List<AsistenciaDetalleUsuarioDTO>> Ejecutar(long usuarioId)
        {
            // 1️⃣ Traemos las entidades desde la capa de infraestructura
            var asistencias = await _asistenciaRepositorio.ObtenerPorUsuarioAsync(usuarioId);

            // 2️⃣ Mapeamos entidades → DTOs con AutoMapper
            return _mapper.Map<List<AsistenciaDetalleUsuarioDTO>>(asistencias);
        }
    }
}
