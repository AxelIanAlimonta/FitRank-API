using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SolicitudCasosDeUso
{
    public class RechazarSolicitudCasoDeUso
    {
        private readonly ISolicitudRutinaProfesorRepositorio _repositorio;

        public RechazarSolicitudCasoDeUso(ISolicitudRutinaProfesorRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public virtual async Task<bool> EjecutarAsync(long solicitudId, long profesorId, string? mensaje)
        {
            var solicitud = await _repositorio.ObtenerPorIdAsync(solicitudId);
            if (solicitud == null || solicitud.Estado != EstadoSolicitud.Pendiente)
                return false;

            solicitud.Estado = EstadoSolicitud.Rechazada;
            solicitud.ProfesorId = profesorId;
            solicitud.MensajeProfesor = mensaje;
            solicitud.FechaResolucion = DateTime.UtcNow;

            await _repositorio.ActualizarAsync(solicitud);
            return true;
        }
    }

}
