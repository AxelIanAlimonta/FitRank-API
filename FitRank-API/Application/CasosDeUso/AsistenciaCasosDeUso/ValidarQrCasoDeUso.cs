using FitRank_API.Application.CasosDeUso.Invitacion;
using FitRank_API.Application.DTOs.Asistencia;
using FitRank_API.Application.DTOs.QR;
using FitRank_API.Application.DTOs.UsuarioDTOs;
using FitRank_API.Application.Helpers;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace FitRank_API.Application.CasosDeUso.Invitacion.RegistrarInvitacionCasoDeUso
{
    public class ValidarQrCasoDeUso
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IAsistenciaRepositorio _asistenciaRepositorio;
        private readonly IConfiguration _config;
        private readonly QrHelper _qrHelper;
        private readonly IGimnasioRepositorio _gimnasioRepositorio;

   public ValidarQrCasoDeUso(IUsuarioRepositorio usuarioRepositorio,
            IAsistenciaRepositorio asistenciaRepositorio,
            IConfiguration config,
            QrHelper qrHelper,
            IGimnasioRepositorio gimnasioRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _asistenciaRepositorio = asistenciaRepositorio;
            _config = config;
            _qrHelper = qrHelper;
            _gimnasioRepositorio = gimnasioRepositorio;
        }

        public async Task<QrValidationResponseDTO> Ejecutar(QrValidationDTO dto, int? adminId)
        {
            try
            {

                var tokenStr = dto.QrData.Contains("token=")
                    ? dto.QrData.Split("token=")[1].Split('&')[0]
                    : dto.QrData;

                var key = Encoding.UTF8.GetBytes(_config["QrSecret"] ?? "default_qr_secret_please_change");
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParams = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true
                };

                var principal = tokenHandler.ValidateToken(tokenStr, validationParams, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;

            
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId");
                var validoHastaClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "validoHasta");
                var gymIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "gymId");

                if (userIdClaim == null || validoHastaClaim == null)
                    return new QrValidationResponseDTO { Valido = false, Mensaje = "QR malformado" };

                var userId = int.Parse(userIdClaim.Value);
                var validoHasta = DateTime.Parse(validoHastaClaim.Value);
                var qrGymId = gymIdClaim != null ? int.Parse(gymIdClaim.Value) : 1;

             
                var user = await _usuarioRepositorio.ObtenerPorIdAsync(userId);
                if (user == null)
                    return new QrValidationResponseDTO { Valido = false, Mensaje = "Usuario no encontrado" };

              
                if (!user.CuotaPagadaHasta.HasValue || user.CuotaPagadaHasta < DateTime.Now || validoHasta < DateTime.Now)
                    return new QrValidationResponseDTO { Valido = false, Mensaje = "Cuota expirada o QR inválido" };


                
                long gimnasioId = 0;
                if (adminId.HasValue)
                {
                    var gimnasio = await _gimnasioRepositorio.ObtenerPorAdministradorIdAsync(adminId.Value);
                    if (gimnasio == null)
                        return new QrValidationResponseDTO { Valido = false, Mensaje = "No se encontró gimnasio asociado al administrador." };

                    gimnasioId = gimnasio.Id;

                    if (qrGymId != gimnasio.Id)
                        return new QrValidationResponseDTO { Valido = false, Mensaje = "QR no válido para este gimnasio." };
                }


                var asistencia = new Asistencia
                {
                    UsuarioId = userId,
                    Fecha = DateTime.Today,
                    Presente = true,
                    HoraEntrada = DateTime.Now,
                    Observaciones = dto.Observaciones ?? "Ingreso por QR",
                    GimnasioId = gimnasioId
                };

                await _asistenciaRepositorio.AgregarAsync(asistencia);

              
                var userDto = new UsuarioAuthDTO
                {
                    Id = user.Id,
                    Nombre = user.Nombre,
                    Apellidos = user.Apellido,
                    Email = user.Email,
                    NombreUsuario = user.NombreUsuario,
                    Rol = user.Rol ?? "Socio",
                    CuotaPagadaHasta = user.CuotaPagadaHasta,
                    TieneCuotaPagada = true
                };

               
                return new QrValidationResponseDTO
                {
                    Valido = true,
                    Mensaje = "Acceso permitido",
                    User = userDto,
                    AsistenciaId = (int?)asistencia.Id
                };
            }
            catch (SecurityTokenExpiredException)
            {
                return new QrValidationResponseDTO { Valido = false, Mensaje = "QR expirado" };
            }

        }
    }
}

