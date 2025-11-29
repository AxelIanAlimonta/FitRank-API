using AutoMapper;
using FitRank_API.Application.DTOs.Invitacion;
using FitRank_API.Application.DTOs.UsuarioDTOs;
using FitRank_API.Application.Interfaces;
using FitRank_API.Domain.Interfaces;
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
        private readonly IPasswordService _passwordService;

        public AgregarUsuarioConInvitacionCasoDeUso(
            IUsuarioRepositorio usuarioRepo,
            IInvitacionRepositorio invitacionRepo,
            IMapper mapper,
            GenerarTokenCasoDeUso generarToken,
            IPasswordService passwordService)
        {
            _usuarioRepo = usuarioRepo;
            _invitacionRepo = invitacionRepo;
            _mapper = mapper;
            _generarToken = generarToken;
            _passwordService = passwordService;
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
            
            invitacion.UsuarioId = socio.Id;
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
                PasswordHash = _passwordService.HashPassword(dto.Password),
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

            var socioRegistrado = await _usuarioRepo.AgregarAsync(socio);
            return socioRegistrado as Socio ?? socio;
        }
    }
}

