

using FitRank_API.Application.DTOs.ProfesorDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.ProfesorCasosDeUso
{
    public class ObtenerEstadisticasProfesoresCasoDeUso
    {
        private readonly ISolicitudRutinaProfesorRepositorio _solicitudRepo;

        public ObtenerEstadisticasProfesoresCasoDeUso(ISolicitudRutinaProfesorRepositorio solicitudRepo)
        {
            _solicitudRepo = solicitudRepo;
        }

        public async Task<EstadisticasProfesoresDTO> Ejecutar()
        {
            var (topSolicitado, topPendientes, topCumplidor, topValorado)
                = await _solicitudRepo.ObtenerEstadisticasProfesoresAsync();

            return new EstadisticasProfesoresDTO
            {
                TopSolicitado = topSolicitado != null ? new TopSolicitadoDTO
                {
                    NombreProfesor = $"{topSolicitado.Nombre} {topSolicitado.Apellido}",
                    CantidadSolicitudes = topSolicitado.Solicitudes?.Count ?? 0
                } : null,

                TopPendientes = topPendientes != null ? new TopPendientesDTO
                {
                    NombreProfesor = $"{topPendientes.Nombre} {topPendientes.Apellido}",
                    Pendientes = topPendientes.Solicitudes?.Count(s => s.Estado == EstadoSolicitud.Pendiente) ?? 0
                } : null,

                TopCumplidor = topCumplidor != null ? new TopCumplidorDTO
                {
                    NombreProfesor = $"{topCumplidor.Nombre} {topCumplidor.Apellido}",
                    Completadas = topCumplidor.Solicitudes?
                        .Count(s => s.Estado == EstadoSolicitud.TomadaPorProfesor || s.Estado == EstadoSolicitud.Rechazada) ?? 0
                } : null,

                TopValorado = topValorado != null ? new TopValoradaDTO
                {
                    NombreProfesor = $"{topValorado.Value.profesor?.Nombre} {topValorado.Value.profesor?.Apellido}",
                    PromedioValoracion = topValorado.Value.promedio
                } : null
            };
        }
    }
}
