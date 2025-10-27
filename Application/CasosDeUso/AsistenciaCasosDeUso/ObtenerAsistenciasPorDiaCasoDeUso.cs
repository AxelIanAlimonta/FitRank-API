using FitRank_API.Application.DTOs.Asistencia;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.AsistenciaCasosDeUso
{
    public class ObtenerAsistenciasPorDiaCasoDeUso
    {
        private readonly IAsistenciaRepositorio _asistenciaRepositorio;

        public ObtenerAsistenciasPorDiaCasoDeUso(IAsistenciaRepositorio asistenciaRepositorio)
        {
            _asistenciaRepositorio = asistenciaRepositorio;
        }

        public async Task<List<AsistenciaPorDiaDTO>> Ejecutar(int gimnasioId, DateTime? desde = null, DateTime? hasta = null)
        {
            return await _asistenciaRepositorio.ObtenerConteoPorDiaAsync(gimnasioId, desde, hasta);
        }
    }
}
