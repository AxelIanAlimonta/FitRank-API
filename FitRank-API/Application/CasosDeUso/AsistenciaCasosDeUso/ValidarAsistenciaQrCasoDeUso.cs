using FitRank_API.Application.DTOs.Asistencia;
using FitRank_API.Application.DTOs.QR;
using FitRank_API.Application.DTOs.UsuarioDTOs;
using FitRank_API.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using FitRank_API.Application.Hubs;

namespace FitRank_API.Application.CasosDeUso.Asistencia
{
    public class ValidarAsistenciaQrCasoDeUso
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IAsistenciaRepositorio _asistenciaRepositorio;
        private readonly IGimnasioRepositorio _gimnasioRepositorio;
        private readonly IConfiguration _config;
        private readonly IHubContext<NotificacionesHub> _hub;

        public ValidarAsistenciaQrCasoDeUso(
            IUsuarioRepositorio usuarioRepositorio,
            IAsistenciaRepositorio asistenciaRepositorio,
            IGimnasioRepositorio gimnasioRepositorio,
            IConfiguration config,
            IHubContext<NotificacionesHub> hub)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _asistenciaRepositorio = asistenciaRepositorio;
            _gimnasioRepositorio = gimnasioRepositorio;
            _config = config;
            _hub = hub;
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

                var principal = tokenHandler.ValidateToken(tokenStr, validationParams, out var validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;

                var userId = int.Parse(jwtToken.Claims.First(c => c.Type == "userId").Value);
                var qrGymId = int.Parse(jwtToken.Claims.First(c => c.Type == "gymId").Value);
                var validoHasta = DateTime.Parse(jwtToken.Claims.First(c => c.Type == "validoHasta").Value);

                var user = await _usuarioRepositorio.ObtenerPorIdAsync(userId);
                if (user == null)
                    return new QrValidationResponseDTO { Valido = false, Mensaje = "Usuario no encontrado" };

                if (user.CuotaPagadaHasta < DateTime.Now)
                    return new QrValidationResponseDTO { Valido = false, Mensaje = "Cuota expirada" };

                
                var ultimaHoy = await _asistenciaRepositorio.ObtenerUltimaAsistenciaHoyAsync(user.Id, qrGymId);

         
                if (ultimaHoy == null)
                {
                    var nueva = new FitRank_API.Domain.Entities.Asistencia
                    {
                        UsuarioId = user.Id,
                        GimnasioId = qrGymId,
                        Fecha = DateTime.Today,
                        HoraEntrada = DateTime.Now,
                        Presente = true,
                        Observaciones = "Ingreso por QR"
                    };

                    await _asistenciaRepositorio.AgregarAsync(nueva);

                    await EnviarSignalREvento("entrada", user, qrGymId);

                    return new QrValidationResponseDTO
                    {
                        Valido = true,
                        Mensaje = "Entrada registrada",
                        AsistenciaId = (int)nueva.Id
                    };
                }

                if (ultimaHoy.HoraSalida == null)
                {
                    ultimaHoy.HoraSalida = DateTime.Now;
                    ultimaHoy.Presente = false;

                    await _asistenciaRepositorio.ActualizarAsync(ultimaHoy);
                    await EnviarSignalREvento("salida", user, qrGymId);

                    return new QrValidationResponseDTO
                    {
                        Valido = true,
                        Mensaje = "Salida registrada",
                        AsistenciaId = (int)ultimaHoy.Id
                    };
                }

             
                var nuevaEntrada = new FitRank_API.Domain.Entities.Asistencia
                {
                    UsuarioId = user.Id,
                    GimnasioId = qrGymId,
                    Fecha = DateTime.Today,
                    HoraEntrada = DateTime.Now,
                    Presente = true,
                    Observaciones = "Re-ingreso por QR"
                };

                await _asistenciaRepositorio.AgregarAsync(nuevaEntrada);
                await EnviarSignalREvento("entrada", user, qrGymId);

                return new QrValidationResponseDTO
                {
                    Valido = true,
                    Mensaje = "Nueva entrada registrada",
                    AsistenciaId = (int)nuevaEntrada.Id
                };
            }
            catch (Exception ex)
            {
                return new QrValidationResponseDTO
                {
                    Valido = false,
                    Mensaje = "Error QR: " + ex.Message
                };
            }
        }

        private async Task EnviarSignalREvento(string tipo, dynamic user, long gymId)
        {
            await _hub.Clients.Group($"gimnasio-{gymId}").SendAsync("OcupacionActualizada", new
            {
                tipo = tipo,
                usuarioId = user.Id,
                nombre = $"{user.Nombre} {user.Apellido}",
                foto = user.FotoDePerfil,
                fecha = DateTime.Now
            });
        }
    }
}
