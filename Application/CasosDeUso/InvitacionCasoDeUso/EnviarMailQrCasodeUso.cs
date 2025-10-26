using FitRank_API.Application.DTOs.QR;
using FitRank_API.Infrastructure.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;
using FitRank_API.Domain.Entities;
using FitRank_API.Application.DTOs.UsuarioDTOs;

namespace FitRank_API.Application.CasosDeUso.Invitacion
{
    public class EnviarEmailQrCasoDeUso
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly ISendGridClient _sendGridClient;
        private readonly IConfiguration _config;

        public EnviarEmailQrCasoDeUso(
            IUsuarioRepositorio usuarioRepositorio,
            ISendGridClient sendGridClient,
            IConfiguration config)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _sendGridClient = sendGridClient;
            _config = config;
        }

        public async Task<EmailResponseDTO> Ejecutar(EmailDTO dto)
        {
            try
            {
                // 1️⃣ Buscar usuario
                var user = await _usuarioRepositorio.ObtenerPorIdAsync(dto.UsuarioId);
                if (user == null)
                    return new EmailResponseDTO
                    {
                        Success = false,
                        Mensaje = "Usuario no encontrado"
                    };

               
                var qrData = $"{_config["FrontendUrl"] ?? "http://localhost:4200"}/perfil?token={user.QrToken}";
                var vencimiento = user.CuotaPagadaHasta?.ToShortDateString() ?? "No definido";

              
                var msg = new SendGridMessage
                {
                    From = new EmailAddress(_config["Email:From"] ?? "noreply@fitrank.com", "FitRank"),
                    Subject = "Tu QR de Acceso FitRank",
                    PlainTextContent = $"Bienvenido, {user.Nombre}. Tu QR vence el {vencimiento}. Accede aquí: {qrData}",
                    HtmlContent = $@"
                        <p>¡Bienvenido, {user.Nombre} {user.Apellido}!</p>
                        <p>Tu QR de acceso vence el <strong>{vencimiento}</strong>.</p>
                        <p>Visualízalo en: <a href='{qrData}'>Ver mi QR</a></p>"
                };

                msg.AddTo(new EmailAddress(dto.EmailDestinatario ?? user.Email));

                // 4️⃣ Enviar email
                var response = await _sendGridClient.SendEmailAsync(msg);
                if (response.StatusCode == System.Net.HttpStatusCode.Accepted ||
                    response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return new EmailResponseDTO { Success = true, Mensaje = "Email enviado correctamente" };
                }

                return new EmailResponseDTO
                {
                    Success = false,
                    Mensaje = $"Error enviando email: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new EmailResponseDTO
                {
                    Success = false,
                    Mensaje = "Error inesperado: " + ex.Message
                };
            }
        }
    }
}

