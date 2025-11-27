using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SolicitudCasosDeUso
{
    public class TomarSolicitudCasoDeUso
    {
        private readonly ISolicitudRutinaProfesorRepositorio _repositorio;

        public TomarSolicitudCasoDeUso(ISolicitudRutinaProfesorRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public virtual async Task<bool> EjecutarAsync(long solicitudId, long profesorId)
        {
            var solicitud = await _repositorio.ObtenerPorIdAsync(solicitudId);
            if (solicitud == null || solicitud.Estado != EstadoSolicitud.Pendiente)
                return false;

            solicitud.Estado = EstadoSolicitud.TomadaPorProfesor;
            solicitud.ProfesorId = profesorId;
            await _repositorio.ActualizarAsync(solicitud);
            return true;
        }
    }

}
