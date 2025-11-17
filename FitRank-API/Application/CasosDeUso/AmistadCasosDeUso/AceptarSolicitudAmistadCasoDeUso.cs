using FitRank_API.Application.DTOs.AmistadDTOs;
using FitRank_API.Domain.Enums;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.AmistadCasosDeUso
{
    public class AceptarSolicitudAmistadCasoDeUso
    {
        private readonly IAmistadRepositorio _amistadRepositorio;

        public AceptarSolicitudAmistadCasoDeUso(IAmistadRepositorio amistadRepositorio)
        {
            _amistadRepositorio = amistadRepositorio;
        }

        public async Task<AmistadDTO> Ejecutar(AceptarSolicitudAmistadDTO dto)
        {
            var amistad = await _amistadRepositorio.ObtenerPorIdAsync(dto.AmistadId);

            if (amistad == null)
            {
                return new AmistadDTO
                {
                    Completado = false,
                    Mensaje = "Solicitud no encontrada."
                };
            }

            var esParte = amistad.SocioId1 == dto.SocioId || amistad.SocioId2 == dto.SocioId;
            if (!esParte)
            {
                return new AmistadDTO
                {
                    Completado = false,
                    Mensaje = "No podés aceptar esta solicitud."
                };
            }

            if (amistad.Estado != EstadoAmistad.Pendiente)
            {
                return new AmistadDTO
                {
                    Completado = false,
                    Mensaje = "La solicitud no está pendiente."
                };
            }

            if (amistad.SolicitanteId == dto.SocioId)
            {
                return new AmistadDTO
                {
                    Completado = false,
                    Mensaje = "No podés aceptar tu propia solicitud."
                };
            }

            amistad.Estado = EstadoAmistad.Aceptado;
            amistad.FechaActualizacion = DateTime.UtcNow;

            await _amistadRepositorio.ActualizarAsync(amistad);

            return new AmistadDTO
            {
                Completado = true,
                Mensaje = "Solicitud aceptada correctamente.",
                AmistadId = amistad.Id,
                SocioId1 = amistad.SocioId1,
                SocioId2 = amistad.SocioId2,
                SolicitanteId = amistad.SolicitanteId,
                Estado = amistad.Estado.ToString()
            };
        }
    }
}
