using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SolicitudCasosDeUso
{
    public class TerminarSolicitudCasoDeUso
    {
        private readonly ISolicitudRutinaProfesorRepositorio _repositorio;

        public TerminarSolicitudCasoDeUso(ISolicitudRutinaProfesorRepositorio repositorio)
        {
            _repositorio = repositorio;
        }
        public virtual async Task<bool> EjecutarAsync(long solicitudId)
        {
            var solicitud = await _repositorio.ObtenerPorIdAsync(solicitudId);
            if (solicitud == null)
                return false;

            solicitud.Estado = EstadoSolicitud.Finalizada;
            solicitud.FechaResolucion = DateTime.UtcNow;

            await _repositorio.ActualizarAsync(solicitud);
            return true;
        }
    }
}
