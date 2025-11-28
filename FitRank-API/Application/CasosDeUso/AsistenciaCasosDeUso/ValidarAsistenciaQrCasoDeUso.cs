using FitRank_API.Domain.Interfaces;
using FitRank_API.Application.DTOs.Asistencia;
using FitRank_API.Application.DTOs.UsuarioDTOs;
using FitRank_API.Application.DTOs.QR;
using FitRank_API.Application.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;


namespace FitRank_API.Application.CasosDeUso.AsistenciaCasosDeUso
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


        public virtual async Task<QrValidationResponseDTO> Ejecutar(QrValidationDTO dto, int? adminId)
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

                if (userIdClaim == null)
                    return new QrValidationResponseDTO { Valido = false, Mensaje = "QR inválido." };

                var userId = int.Parse(userIdClaim.Value);
                var validoHasta = DateTime.Parse(validoHastaClaim!.Value);
                var qrGymId = int.Parse(gymIdClaim!.Value);

                var user = await _usuarioRepositorio.ObtenerPorIdAsync(userId);
                if (user == null)
                    return new QrValidationResponseDTO { Valido = false, Mensaje = "Usuario no encontrado" };

                if (user.CuotaPagadaHasta < DateTime.Now)
                    return new QrValidationResponseDTO { Valido = false, Mensaje = "Cuota expirada" };

                var asistenciaHoy = await _asistenciaRepositorio.ObtenerPorUsuarioYFechaAsync(user.Id, DateTime.Today);

                if (asistenciaHoy == null)
                {
                    var nueva = new FitRank_API.Domain.Entities.Asistencia
                    {
                        UsuarioId = user.Id,
                        Fecha = DateTime.Today,
                        Presente = true,
                        HoraEntrada = DateTime.Now,
                        GimnasioId = qrGymId,
                        Observaciones = "Ingreso por QR"
                    };

                    await _asistenciaRepositorio.AgregarAsync(nueva);
                    await _hub.Clients.Group($"gimnasio-{qrGymId}")
    .SendAsync("OcupacionActualizada", new
    {
        tipo = "entrada",
        usuarioId = user.Id,
        nombre = $"{user.Nombre} {user.Apellido}",
        foto = user.FotoDePerfil,  // solo s
        fecha = DateTime.Now
    });


                    return new QrValidationResponseDTO
                    {
                        Valido = true,
                        Mensaje = "✅ Acceso permitido — entrada registrada",
                        AsistenciaId = (int)nueva.Id,
                        UsuarioId = userId,
                        User = new UsuarioAuthDTO
                        {
                            Id = user.Id,
                            Nombre = user.Nombre,
                            Apellidos = user.Apellido,
                            Email = user.Email,
                            Rol = user.Rol
                        }

                    };
                }
                else if (asistenciaHoy.Presente && asistenciaHoy.HoraSalida == null)
                {
                    asistenciaHoy.Presente = false;
                    asistenciaHoy.HoraSalida = DateTime.Now;
                    await _asistenciaRepositorio.ActualizarAsync(asistenciaHoy);
                    await _hub.Clients.Group($"gimnasio-{qrGymId}")
    .SendAsync("OcupacionActualizada", new
    {
        tipo = "salida",
        usuarioId = user.Id,
        nombre = $"{user.Nombre} {user.Apellido}",
        foto = user.FotoDePerfil,
        fecha = DateTime.Now
    });

                    return new QrValidationResponseDTO
                    {
                        Valido = true,
                        Mensaje = "👋 Salida registrada correctamente",
                        AsistenciaId = (int)asistenciaHoy.Id
                    };
                }
                if (!asistenciaHoy.Presente && asistenciaHoy.HoraSalida != null)
                {
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

                    await _hub.Clients.Group($"gimnasio-{qrGymId}").SendAsync("OcupacionActualizada", new
                    {
                        tipo = "entrada",
                        usuarioId = user.Id,
                        nombre = $"{user.Nombre} {user.Apellido}",
                        foto = user.FotoDePerfil,
                        fecha = DateTime.Now
                    });

                    return new QrValidationResponseDTO
                    {
                        Valido = true,
                        Mensaje = "Nueva entrada registrada",
                        AsistenciaId = (int)nuevaEntrada.Id
                    };
                }

                return new QrValidationResponseDTO { Valido = false, Mensaje = "Ya registraste tu salida hoy." };
            }
            catch (Exception ex)
            {
                return new QrValidationResponseDTO { Valido = false, Mensaje = "Error: " + ex.Message };
            }
        }
    }
    }

