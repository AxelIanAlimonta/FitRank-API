using FitRank_API.Application.DTOs.SolicitudDTO;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface ISolicitudRutinaProfesorRepositorio
    {
        Task AgregarAsync(SolicitudRutinaProfesor solicitud);
        Task<SolicitudRutinaProfesor?> ObtenerPorIdAsync(long id);
        Task<List<SolicitudRutinaProfesorDTO>> ObtenerPendientesAsync();
        Task<List<SolicitudRutinaProfesorDTO>> ObtenerPorProfesorAsync(long profesorId);
        Task<List<SolicitudRutinaProfesorDTO>> ObtenerPorSocioAsync(long socioId);
        Task ActualizarAsync(SolicitudRutinaProfesor solicitud);
        Task<Profesor?> ObtenerProfesorMasSolicitadoAsync();
        Task<Profesor?> ObtenerProfesorConMasPendientesAsync();
        Task<Profesor?> ObtenerProfesorMasCumplidorAsync();
        Task<(Profesor?, double?)?> ObtenerProfesorMejorPromedioValoracionesAsync();

    }
}
