using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SolicitudCasosDeUso
{
    public class FinalizarSolicitudCasoDeUso
    {
        private readonly ISolicitudRutinaProfesorRepositorio _repositorio;

        public FinalizarSolicitudCasoDeUso(ISolicitudRutinaProfesorRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public virtual async Task<bool> EjecutarAsync(long solicitudId, long rutinaId, string? mensaje)
        {
            var solicitud = await _repositorio.ObtenerPorIdAsync(solicitudId);
            if (solicitud == null || solicitud.Estado != EstadoSolicitud.TomadaPorProfesor)
                return false;

            solicitud.Estado = EstadoSolicitud.Finalizada;
            solicitud.RutinaId = rutinaId;
            solicitud.FechaResolucion = DateTime.UtcNow;
            solicitud.MensajeProfesor = mensaje;

            await _repositorio.ActualizarAsync(solicitud);
            return true;
        }
    }

}
