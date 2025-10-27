using FitRank_API.Application.DTOs.Asistencia;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.AsistenciaCasosDeUso
{
    public class ObtenerAsistenciasDetalladasPorUsuarioCasoDeUso
    {
        private readonly IAsistenciaRepositorio _asistenciaRepositorio;

        public ObtenerAsistenciasDetalladasPorUsuarioCasoDeUso(IAsistenciaRepositorio asistenciaRepositorio)
        {
            _asistenciaRepositorio = asistenciaRepositorio;
        }

        public async Task<List<AsistenciaDetalleUsuarioDTO>> Ejecutar(int usuarioId)
        {
            return await _asistenciaRepositorio.ObtenerAsistenciasDetalladasPorUsuarioAsync(usuarioId);
        }
    }
}
