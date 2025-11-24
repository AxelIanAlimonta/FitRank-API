using AutoMapper;
using FitRank_API.Application.DTOs.Asistencia;
using FitRank_API.Application.DTOs.SocioDTOs;
using FitRank_API.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Application.CasosDeUso.AsistenciaCasosDeUso
{
    public class ObtenerAsistenciasDetalladasPorUsuarioCasoDeUso
    {
        private readonly IAsistenciaRepositorio _asistenciaRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IMapper _mapper;

        public ObtenerAsistenciasDetalladasPorUsuarioCasoDeUso(
            IAsistenciaRepositorio asistenciaRepositorio,
            IUsuarioRepositorio usuarioRepositorio,
            IMapper mapper)
        {
            _asistenciaRepositorio = asistenciaRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
            _mapper = mapper;
        }

        public virtual async Task<DetalleUsuarioAsistenciaRespuestaDTO> Ejecutar(long usuarioId)
        {
            var socio = await _usuarioRepositorio.ObtenerSocioConGimnasioPorIdAsync(usuarioId);


            if (socio == null)
            {
                return new DetalleUsuarioAsistenciaRespuestaDTO
                {
                    Exito = false,
                    Mensaje = "No se encontró el socio solicitado."
                };
            }

            // 🔹 Obtener asistencias
            var asistencias = await _asistenciaRepositorio.ObtenerPorUsuarioAsync(usuarioId);

            // 🔹 Mapear a DTOs
            var asistenciasDto = _mapper.Map<List<AsistenciaDetalleUsuarioDTO>>(asistencias);

            var socioDto = _mapper.Map<SocioDTO>(socio);


            return new DetalleUsuarioAsistenciaRespuestaDTO
            {
                Exito = true,
                Mensaje = "Detalle obtenido correctamente.",
                Socio = socioDto,
                Asistencias = asistenciasDto
            };
        }
    }
}

