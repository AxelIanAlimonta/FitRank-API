using FitRank_API.Application.DTOs.AmistadDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.AmistadCasosDeUso
{
    public class ObtenerSolicitudesPendientesCasoDeUso
    {
        private readonly IAmistadRepositorio _amistadRepositorio;

        public ObtenerSolicitudesPendientesCasoDeUso(IAmistadRepositorio amistadRepositorio)
        {
            _amistadRepositorio = amistadRepositorio;
        }

        public virtual async Task<List<SolicitudAmistadDTO>> Ejecutar(int socioId)
        {
            var solicitudes = await _amistadRepositorio.ObtenerSolicitudesPendientesAsync(socioId);

            var result = solicitudes.Select(a => new SolicitudAmistadDTO
            {
                AmistadId = a.Id,
                RemitenteId = a.SolicitanteId,
                RemitenteNombreUsuario = a.Solicitante.NombreUsuario,
                RemitenteNombre = a.Solicitante.Nombre,
                RemitentePuntaje = (a.Solicitante as Socio)?.Puntaje ?? 0
            }).ToList();

            return result;
        }
    }
}
