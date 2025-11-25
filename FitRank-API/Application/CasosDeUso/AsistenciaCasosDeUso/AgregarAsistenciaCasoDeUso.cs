using AutoMapper;
using FitRank_API.Application.DTOs.Asistencia;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.AsistenciaCasosDeUso
{
    public class AgregarAsistenciaCasoDeUso
    {
        private readonly IAsistenciaRepositorio _asistenciaRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IMapper _mapper;

        public AgregarAsistenciaCasoDeUso(
            IAsistenciaRepositorio asistenciaRepositorio,
            IUsuarioRepositorio usuarioRepositorio,
            IMapper mapper)
        {
            _asistenciaRepositorio = asistenciaRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
            _mapper = mapper;
        }

        public virtual async Task<AsistenciaResponseDTO> Ejecutar(AgregarAsistenciaDTO dto)
        {
            var usuario = await _usuarioRepositorio.ObtenerPorIdAsync(dto.UsuarioId);
            if (usuario == null)
            {
                return new AsistenciaResponseDTO
                {
                    Success = false,
                    Mensaje = "Usuario no encontrado."
                };
            }

            var asistencia = _mapper.Map<FitRank_API.Domain.Entities.Asistencia>(dto);

            asistencia.Fecha = DateTime.UtcNow.Date;
            asistencia.HoraEntrada = DateTime.UtcNow;
            asistencia.Presente = true;

            await _asistenciaRepositorio.AgregarAsync(asistencia);

          
            var response = _mapper.Map<AsistenciaResponseDTO>(asistencia);
            response.Success = true;
            response.Mensaje = "Asistencia registrada correctamente.";
            response.NombreUsuario = $"{usuario.Nombre} {usuario.Apellido}";

            return response;
        }
    }
}
