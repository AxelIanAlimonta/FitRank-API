using AutoMapper;
using FitRank_API.Application.DTOs.Asistencia;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.AsistenciaCasosDeUso
{
    public class ObtenerAsistenciasPorDiaCasoDeUso
    {
        private readonly IAsistenciaRepositorio _asistenciaRepositorio;

        public ObtenerAsistenciasPorDiaCasoDeUso(IAsistenciaRepositorio asistenciaRepositorio)
        {
            _asistenciaRepositorio = asistenciaRepositorio;
        }

        public virtual async Task<List<AsistenciaPorDiaDTO>> Ejecutar(long gimnasioId, DateTime? desde = null, DateTime? hasta = null)
        {
            var asistencias = await _asistenciaRepositorio.ObtenerPorGimnasioYRangoAsync(gimnasioId, desde, hasta);

          
            var resultado = asistencias
                .GroupBy(a => a.Fecha.Date)
                .Select(g => new AsistenciaPorDiaDTO
                {
                    Fecha = g.Key,
                    Cantidad = g.Count()
                })
                .OrderByDescending(x => x.Fecha)
                .ToList();

            return resultado;
        }
    }
}
