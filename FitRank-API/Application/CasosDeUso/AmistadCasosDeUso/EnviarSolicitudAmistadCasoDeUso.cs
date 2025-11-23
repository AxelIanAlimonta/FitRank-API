using FitRank_API.Application.DTOs.AmistadDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Enums;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.AmistadCasosDeUso
{
    public class EnviarSolicitudAmistadCasoDeUso
    {
        private readonly IAmistadRepositorio _amistadRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        public EnviarSolicitudAmistadCasoDeUso(IAmistadRepositorio amistadRepositorio, IUsuarioRepositorio usuarioRepositorio)
        {
            _amistadRepositorio = amistadRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
        }

        public async Task<AmistadDTO> Ejecutar(EnviarSolicitudAmistadDTO dto)
        {
            if (dto.SolicitanteId == dto.DestinatarioId)
            {
                return new AmistadDTO
                {
                    Completado = false,
                    Mensaje = "No podés enviarte solicitud a vos mismo."
                };
            }

            var solicitante = await _usuarioRepositorio.ObtenerPorIdAsync(dto.SolicitanteId);
            var destinatario = await _usuarioRepositorio.ObtenerPorIdAsync(dto.DestinatarioId);

            if (solicitante == null || destinatario == null)
            {
                return new AmistadDTO
                {
                    Completado = false,
                    Mensaje = "Alguno de los usuarios no existe."
                };
            }

            var socioId1 = Math.Min(dto.SolicitanteId, dto.DestinatarioId);
            var socioId2 = Math.Max(dto.SolicitanteId, dto.DestinatarioId);

            var existente = await _amistadRepositorio.ObtenerPorIdDeSociosAsync(socioId1, socioId2);

            if (existente != null)
            {
                if (existente.Estado == EstadoAmistad.Aceptado)
                {
                    return new AmistadDTO
                    {
                        Completado = false,
                        Mensaje = "Ya son amigos."
                    };
                }

                if (existente.Estado == EstadoAmistad.Pendiente)
                {
                    return new AmistadDTO
                    {
                        Completado = false,
                        Mensaje = "Ya existe una solicitud pendiente."
                    };
                }
            }

            var ahora = DateTime.UtcNow;

            var amistad = new Amistad
            {
                SocioId1 = socioId1,
                SocioId2 = socioId2,
                SolicitanteId = dto.SolicitanteId,
                Estado = EstadoAmistad.Pendiente,
                FechaCreacion = ahora,
                FechaActualizacion = ahora
            };

            amistad = await _amistadRepositorio.CrearAsync(amistad);

            return new AmistadDTO
            {
                Completado = true,
                Mensaje = "Solicitud de amistad enviada correctamente.",
                AmistadId = amistad.Id,
                SocioId1 = amistad.SocioId1,
                SocioId2 = amistad.SocioId2,
                SolicitanteId = amistad.SolicitanteId,
                Estado = amistad.Estado.ToString()
            };
        }

    }
}
