using AutoMapper;
using FitRank_API.Application.DTOs.Invitacion;
using FitRank_API.Application.DTOs.UsuarioDTOs;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Application.Helpers;
using System.Text.Json;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.CasosDeUso.UsuarioCasosDeUso
{
    public class AgregarUsuarioConInvitacionCasoDeUso
    {
        private readonly IUsuarioRepositorio _usuarioRepo;
        private readonly IInvitacionRepositorio _invitacionRepo;
        private readonly IMapper _mapper;
        private readonly GenerarTokenCasoDeUso _generarToken;
       

        public AgregarUsuarioConInvitacionCasoDeUso(
            IUsuarioRepositorio usuarioRepo,
            IInvitacionRepositorio invitacionRepo,
            IMapper mapper,
            GenerarTokenCasoDeUso generarToken)
        {
            _usuarioRepo = usuarioRepo;
            _invitacionRepo = invitacionRepo;
            _mapper = mapper;
            _generarToken = generarToken;
        }

        public virtual async Task<AuthResponseDTO?> Ejecutar(RegisterInvitacionDTO dto)
        {
            int? invitacionId = TokenInvitacionHelper.ParseIdFromJwt(dto.TokenInvitacion)
                                ?? TokenInvitacionHelper.ParseIdFromTokenSimple(dto.TokenInvitacion);

            if (invitacionId == null)
                return null;

            var invitacion = await _invitacionRepo.ObtenerPorIdAsync(invitacionId.Value);
            if (invitacion == null || invitacion.UsuarioId != null)
                return null;

            var datosPre = JsonSerializer.Deserialize<Dictionary<string, object>>(invitacion.DatosPrellenados)
                        ?? new Dictionary<string, object>();

            Socio socio = await RegistrarSocioConInvitacion(dto, invitacion, datosPre);

            invitacion.UsuarioId = (int?)socio.Id;
            invitacion.Estado = "Usada";
            await _invitacionRepo.ActualizarAsync(invitacion);


            var token = _generarToken.Ejecutar(socio);

            var socioDto = _mapper.Map<UsuarioAuthDTO>(socio);

            return new AuthResponseDTO
            {
                Token = token,
                User = socioDto
            };
        }

        private async Task<Socio> RegistrarSocioConInvitacion(RegisterInvitacionDTO dto, Domain.Entities.Invitacion invitacion, Dictionary<string, object> datosPre)
        {
            var socio = new Socio
            {
                Nombre = datosPre.GetValueOrDefault("nombre", dto.NombreUsuario)?.ToString() ?? "",
                Apellido = datosPre.GetValueOrDefault("apellidos", "")?.ToString() ?? "",
                Dni = int.Parse(datosPre.GetValueOrDefault("dni", "0")?.ToString() ?? "0"),
                Telefono = datosPre.GetValueOrDefault("telefono", "")?.ToString(),
                Email = invitacion.Email,
                NombreUsuario = dto.NombreUsuario,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Rol = "User",
                Estado = "Activo",
                FechaNacimiento = dto.FechaNacimiento,
                CuotaPagadaHasta = invitacion.CuotaPagadaHasta,
                EsActivado = true,
            

                FechaRegistro = DateTime.UtcNow,
                Nivel = "Inicial",
                Peso = 0,
                Altura = 0,
                Puntaje = 0,
             
                GimnasioId = invitacion.GimnasioId
            };


            await _usuarioRepo.AgregarAsync(socio);
            return socio;
        }
    }
}

