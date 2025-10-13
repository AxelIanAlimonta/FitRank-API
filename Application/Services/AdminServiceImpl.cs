using FitRank_API.Application.DTOs.Auth.invitacion;
using FitRank_API.Application.DTOs.Auth;
using FitRank_API.Application.DTOs.Qr;
using FitRank_API.Application.DTOs.Usuario;
using FitRank_API.Application.Interfaces;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.IdentityModel.Tokens;
using SendGrid.Helpers.Mail;
using System.Drawing.Imaging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using SendGrid;
using SendGrid.Helpers.Mail.Model;

namespace FitRank_API.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly FitRankDbContext _context;
        private readonly IConfiguration _config;
        private readonly string _qrSecret;
        private readonly ISendGridClient _sendGridClient;

        public AdminService(FitRankDbContext context, IConfiguration config, ISendGridClient sendGridClient)
        {
            _context = context;
            _config = config;
            
            _qrSecret = _config["QrSecret"] ?? "default_qr_secret_please_change";
            _sendGridClient = sendGridClient;
        }

        // En AdminService.cs – Reemplaza el método GenerarInvitacionAsync completo
        public async Task<InvitacionResponseDto> GenerarInvitacionAsync(GenerarInvitacionDto dto, int adminId)
        {
            // 1. Generar token de activación (GUID temporal, expira en 24h)
            var tokenActivacion = Guid.NewGuid().ToString("N");

            // 2. Crear invitación primero
            var invitacion = new Invitacion
            {
                GymId = adminId,
                Email = dto.Email,
                DatosPrellenados = JsonSerializer.Serialize(new
                {
                    nombre = dto.Nombre,
                    apellidos = dto.Apellidos,
                    dni = dto.Dni,
                    telefono = dto.Telefono
                }),
                MetodoPago = dto.MetodoPago ?? "Efectivo",  // Asume efectivo
                CreadaEn = DateTime.Now,
                ExpiraEn = DateTime.Now.AddHours(24),
                Estado = "Pagado"  // Para efectivo, asume pagado
            };

            var dias = dto.Periodo == "Yearly" ? 365 : 30;
            invitacion.CuotaPagadaHasta = DateTime.Now.AddDays(dias);

            _context.Invitaciones.Add(invitacion);
            await _context.SaveChangesAsync();  // Guarda para obtener ID

            // 3. Crear usuario temporal (con token de activación, NO activado)
            var user = new Usuario
            {
                nombre = dto.Nombre,
                apellidos = dto.Apellidos,
                dni = dto.Dni,
                telefono = dto.Telefono,
                correo = dto.Email,
                email = dto.Email,
                username = "user" + Guid.NewGuid().ToString("N").Substring(0, 6),  // Temporal
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString("N")),  // Password temporal (no usable directamente)
                Rol = "User",
                estado = "Activo",
                EsActivado = false,  // Nuevo: no activado hasta cambio
                CuotaPagadaHasta = invitacion.CuotaPagadaHasta,
                TokenRecuperacion = tokenActivacion,
                TokenExpira = DateTime.Now.AddHours(24)  // Expira en 24h
            };

            _context.Usuarios.Add(user);
            await _context.SaveChangesAsync();

            // Vincular invitación con usuario
            invitacion.UsuarioId = user.id;
            await _context.SaveChangesAsync();

            // 4. Generar token para QR (JWT para pase, como antes)
            var tokenInvitacion = GenerarQrToken(invitacion);  // Tu método existente
            var qrData = $"{_config["FrontendUrl"] ?? "http://localhost:4200"}/invitacion?token={tokenInvitacion}";
            var qrImageBase64 = await GenerarQrImage(qrData);  // Tu método, retorna "data:image/png;base64,..."

            // 5. Enviar email con QR (inline) y botón de activación
            try
            {
                var linkActivacion = $"{_config["FrontendUrl"] ?? "http://localhost:4200"}/activar-cuenta?token={tokenActivacion}";

                var from = new EmailAddress(_config["Email:From"] ?? "noreply@fitrank.com", "FitRank Admin");
                var to = new EmailAddress(dto.Email);

                var plainTextContent = $@"
¡Hola {dto.Nombre} {dto.Apellidos}!

Te invitamos a unirte a FitRank. Tu pase QR está adjunto (descárgalo para tu celu).

Enlace QR: {qrData}

Datos:
- Nombre: {dto.Nombre}
- Apellidos: {dto.Apellidos}
- DNI: {dto.Dni}
- Teléfono: {dto.Telefono}
- Método: {dto.MetodoPago} (pagado hasta {invitacion.CuotaPagadaHasta?.ToShortDateString()})

Para activar tu cuenta y elegir tu contraseña (por seguridad), haz clic aquí: {linkActivacion}

¡Te esperamos! Equipo FitRank.";

                var htmlContent = $@"
<h2>¡Hola {dto.Nombre} {dto.Apellidos}!</h2>
<p>Te invitamos a unirte a FitRank. Tu pase QR vence el <strong>{invitacion.CuotaPagadaHasta?.ToShortDateString()}</strong>.</p>
<img src='data:image/png;base64,{qrImageBase64.Split(',')[1]}' alt='QR Invitación' style='max-width: 300px; border: 1px solid #ccc;'>
<p><a href='{qrData}' style='color: #007bff;'>O usa este enlace directo al QR</a></p>
<ul>
  <li><strong>Nombre:</strong> {dto.Nombre}</li>
  <li><strong>Apellidos:</strong> {dto.Apellidos}</li>
  <li><strong>DNI:</strong> {dto.Dni}</li>
  <li><strong>Email:</strong> {dto.Email}</li>
  <li><strong>Teléfono:</strong> {dto.Telefono}</li>
  <li><strong>Método de pago:</strong> {dto.MetodoPago} ({dto.Monto} {dto.Periodo})</li>
</ul>
<p>🔒 Por seguridad, activa tu cuenta y elige una contraseña nueva:</p>
<p style='text-align:center; margin: 20px 0;'>
  <a href='{linkActivacion}' 
     style='background-color:#6a1b9a; color:white; padding:12px 22px; text-decoration:none; border-radius:6px; font-weight:bold;'>
     Activar mi cuenta e Iniciar Sesión
  </a>
</p>
<p>Si el botón no funciona, copia este enlace: <a href='{linkActivacion}'>{linkActivacion}</a></p>
<p>Este enlace expira en 24 horas. ¡Te esperamos!<br>Equipo FitRank</p>";

                var msg = MailHelper.CreateSingleEmail(from, to, "Invitación a FitRank - Activa tu cuenta", plainTextContent, htmlContent);

                // Adjunta QR como attachment (opcional, ya que está inline en HTML)
                var qrBytes = Convert.FromBase64String(qrImageBase64.Split(',')[1]);
                msg.AddAttachment("qr_invitacion.png", Convert.ToBase64String(qrBytes), "image/png");

                var response = await _sendGridClient.SendEmailAsync(msg);
                if (response.StatusCode != System.Net.HttpStatusCode.Accepted && response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine($"Error enviando email a {dto.Email}: {response.StatusCode} - {await response.Body.ReadAsStringAsync()}");
                    // No rompe el flujo: el usuario y token ya se crearon
                }
                else
                {
                    Console.WriteLine($"Email enviado exitosamente a {dto.Email}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción enviando email a {dto.Email}: {ex.Message}");  // Log, no falla el método
            }

            return new InvitacionResponseDto
            {
                Success = true,
                QrImage = qrImageBase64,
                TokenInvitacion = tokenInvitacion,
                Mensaje = "Invitación generada y email enviado. Socio active cuenta para login. Cuota pagada hasta " + invitacion.CuotaPagadaHasta?.ToShortDateString(),
                InvitacionId = invitacion.Id
            };
        }

        
        
        public async Task<InvitacionResponseDto> FallbackEfectivoAsync(FallbackEfectivoDto dto, int adminId)
        {
            var invitacion = await _context.Invitaciones
                .FirstOrDefaultAsync(i => i.Id == dto.InvitacionId && i.Estado == "Pendiente");

            if (invitacion == null)
                return new InvitacionResponseDto { Success = false, Mensaje = "Invitación no encontrada" };

            // Cambia a efectivo y set pagado
            invitacion.Estado = "FallbackEfectivo";
            invitacion.MetodoPago = "Efectivo";
            invitacion.CuotaPagadaHasta = DateTime.Now.AddDays(30);  // Default mensual

            // Regenera QR
            var tokenInvitacion = GenerarQrToken(invitacion);
            var qrData = $"{_config["FrontendUrl"] ?? "http://localhost:4200"}/invitacion?token={tokenInvitacion}";
            var qrImage = await GenerarQrImage(qrData);

            await _context.SaveChangesAsync();

            return new InvitacionResponseDto
            {
                Success = true,
                QrImage = qrImage,
                TokenInvitacion = tokenInvitacion,
                Mensaje = "Fallback a efectivo confirmado. Cuota pagada hasta 30 días.",
                InvitacionId = invitacion.Id
            };
        }

        // 3. ValidarQrAsync: Valida QR de acceso, crea Asistencia
        public async Task<QrValidationResponseDto> ValidarQrAsync(QrValidationDto dto, int? adminId)
        {
            try
            {
                // Parsea token del QR data (ej. "https://...token=eyJ...")
                var tokenStr = dto.QrData.Contains("token=") ? dto.QrData.Split("token=")[1].Split('&')[0] : dto.QrData;

                // Valida JWT QR
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_qrSecret);
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

                // Extrae claims
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId");
                var validoHastaClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "validoHasta");
                var gymIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "gymId");

                if (userIdClaim == null || validoHastaClaim == null)
                    return new QrValidationResponseDto { Valido = false, Mensaje = "QR malformado" };

                var userId = int.Parse(userIdClaim.Value);
                var validoHasta = DateTime.Parse(validoHastaClaim.Value);
                var qrGymId = gymIdClaim != null ? int.Parse(gymIdClaim.Value) : 1;

                // Busca usuario
                var user = await _context.Usuarios.FindAsync(userId);
                if (user == null)
                    return new QrValidationResponseDto { Valido = false, Mensaje = "Usuario no encontrado" };

                // Chequea cuota
                if (!user.CuotaPagadaHasta.HasValue || user.CuotaPagadaHasta < DateTime.Now || validoHasta < DateTime.Now)
                    return new QrValidationResponseDto { Valido = false, Mensaje = "Cuota expirada o QR inválido" };

                // Chequea gymId (opcional para single-gym)
                if (adminId.HasValue && qrGymId != adminId.Value)
                    return new QrValidationResponseDto { Valido = false, Mensaje = "QR no válido para este gym" };

                // Crea Asistencia
                var asistencia = new Asistencia
                {
                    UsuarioId = userId,
                    Fecha = DateTime.Today,
                    Presente = true,
                    HoraEntrada = DateTime.Now,
                    HoraSalida = null,
                    Observaciones = dto.Observaciones ?? "Ingreso por QR",
                    GymId = adminId ?? 1  // ID admin o default
                };
                _context.Asistencias.Add(asistencia);
                await _context.SaveChangesAsync();

                // Mapea user DTO
                var userDto = new UsuarioAuthDto
                {
                    Id = user.id,
                    Nombre = user.nombre,
                    Apellidos = user.apellidos,
                    Correo = user.correo,
                    Username = user.username,
                    Rol = user.Rol ?? "User",
                    CuotaPagadaHasta = user.CuotaPagadaHasta,
                    TieneCuotaPagada = true  // Ya chequeado
                };

                return new QrValidationResponseDto
                {
                    Valido = true,
                    Mensaje = "Acceso permitido",
                    User = userDto,
                    AsistenciaId = asistencia.Id
                };
            }
            catch (SecurityTokenExpiredException)
            {
                return new QrValidationResponseDto { Valido = false, Mensaje = "QR expirado" };
            }
            catch (Exception ex)
            {
                return new QrValidationResponseDto { Valido = false, Mensaje = "Error en validación: " + ex.Message };
            }
        }

        // 4. EnviarEmailQrAsync: Envía QR por email (opcional – comenta si no quieres SendGrid)
        public async Task<EmailResponseDto> EnviarEmailQrAsync(EmailDto dto)
        {
            // Si no tienes SendGrid, retorna success falso o comenta
            try
            {
                var user = await _context.Usuarios.FindAsync(dto.UsuarioId);
                if (user == null)
                    return new EmailResponseDto { Success = false, Mensaje = "Usuario no encontrado" };

                var qrData = $"{_config["FrontendUrl"] ?? "http://localhost:4200"}/perfil?token={user.QrToken}";
                var vencimiento = user.CuotaPagadaHasta?.ToShortDateString() ?? "No definido";

                var msg = new SendGridMessage
                {
                    From = new EmailAddress(_config["Email:From"] ?? "noreply@fitrank.com", "FitRank"),
                    Subject = "Tu QR de Acceso FitRank",
                    PlainTextContent = $"Bienvenido, {user.nombre}. Tu QR vence el {vencimiento}. Accede aquí: {qrData}",
                    HtmlContent = $@"
                        <p>¡Bienvenido, {user.nombre} {user.apellidos}!</p>
                        <p>Tu QR de acceso vence el <strong>{vencimiento}</strong>.</p>
                        <p>Visualízalo en: <a href='{qrData}'>Ver mi QR</a></p>"
                };
                msg.AddTo(new EmailAddress(dto.EmailDestinatario ?? user.correo));

                var response = await _sendGridClient.SendEmailAsync(msg);
                if (response.StatusCode == System.Net.HttpStatusCode.Accepted || response.StatusCode == System.Net.HttpStatusCode.OK)
                    return new EmailResponseDto { Success = true, Mensaje = "Email enviado" };

                return new EmailResponseDto { Success = false, Mensaje = "Error enviando email" };
            }
            catch (Exception ex)
            {
                return new EmailResponseDto { Success = false, Mensaje = "Error: " + ex.Message };
            }
        }

        // Helper 1: GenerarQrToken (JWT para QR)
        public string GenerarQrToken(Invitacion invitacion)
        {
            var claims = new[]
            {
                new Claim("userId", "0"),  // Pre-register; set post-register
                new Claim("gymId", invitacion.GymId.ToString()),
                new Claim("validoHasta", invitacion.CuotaPagadaHasta?.ToString("o") ?? DateTime.Now.AddDays(30).ToString("o"))
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_qrSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: invitacion.CuotaPagadaHasta ?? DateTime.Now.AddDays(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Helper 2: GenerarQrImage (Base64 PNG)
        public async Task<string> GenerarQrImage(string data)
        {
            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new QRCode(qrCodeData);
            using var qrBitmap = qrCode.GetGraphic(20);  // Tamaño 20
            using var ms = new MemoryStream();
            qrBitmap.Save(ms, ImageFormat.Png);
            var base64 = Convert.ToBase64String(ms.ToArray());
            return $"data:image/png;base64,{base64}";
        }

        string IAdminService.GenerarQrImage(string data)
        {
            throw new NotImplementedException();
        }
    }
}