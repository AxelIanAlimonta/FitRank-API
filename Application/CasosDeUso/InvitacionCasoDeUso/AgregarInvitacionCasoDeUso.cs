
using global::FitRank_API.Application.CasosDeUso.Invitacion;
using global::FitRank_API.Application.DTOs.Invitacion;
using global::FitRank_API.Domain.Entities;
using global::FitRank_API.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;

namespace FitRank_API.Application.CasosDeUso.Invitacion
{
    public class AgregarInvitacionCasoDeUso
    {
        private readonly IInvitacionRepositorio _invitacionRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IConfiguration _config;
        private readonly ISendGridClient _sendGridClient;
        private readonly QrHelper _qrHelper;
        private readonly IGimnasioRepositorio _gimnasioRepositorio;

        public AgregarInvitacionCasoDeUso(
                  IInvitacionRepositorio invitacionRepositorio,
                  IUsuarioRepositorio usuarioRepositorio,
                  IConfiguration config,
                  ISendGridClient sendGridClient,
                  QrHelper qrHelper,
                  IGimnasioRepositorio gimnasioRepositorio)
        {
            _invitacionRepositorio = invitacionRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
            _config = config;
            _sendGridClient = sendGridClient;
            _qrHelper = qrHelper;
            _gimnasioRepositorio = gimnasioRepositorio;
        }

        public async Task<InvitacionResponseDTO> Ejecutar(GenerarInvitacionDTO dto, int adminId)
        {

            var tokenActivacion = Guid.NewGuid().ToString("N");

            Domain.Entities.Invitacion invitacion = await GenerarNuevaInvitacionAsync(dto, adminId);

            Socio socio = await RegistrarSocioInvitacion(dto, tokenActivacion, invitacion);

            invitacion.UsuarioId = (int?)socio.Id;
            await _invitacionRepositorio.ActualizarAsync(invitacion);

            (string tokenInvitacion, string qrImage) = await ProcesarInvitacionQrAsync(dto, tokenActivacion, invitacion);

            return GenerarInvitacionRespuesta(dto, invitacion, tokenInvitacion, qrImage);
        }

        private static InvitacionResponseDTO GenerarInvitacionRespuesta(GenerarInvitacionDTO dto, Domain.Entities.Invitacion invitacion, string tokenInvitacion, string qrImage)
        {
            return new InvitacionResponseDTO
            {
                Success = true,
                InvitacionId = (int)invitacion.Id,
                TokenInvitacion = tokenInvitacion,
                QrImage = qrImage,
                Mensaje = $"Invitación generada y enviada a {dto.Email}"
            };
        }

        private async Task<(string tokenInvitacion, string qrImage)> ProcesarInvitacionQrAsync(GenerarInvitacionDTO dto, string tokenActivacion, Domain.Entities.Invitacion invitacion)
        {
            var tokenInvitacion = _qrHelper.GenerarQrToken(invitacion);
            var qrData = $"{_config["FrontendUrl"]}/invitacion?token={tokenInvitacion}";
            var qrImage = await _qrHelper.GenerarQrImage(qrData);


            var linkActivacion = $"{_config["FrontendUrl"]}/activar-cuenta?token={tokenActivacion}";
            var from = new EmailAddress(_config["Email:From"], "FitRank");
            var to = new EmailAddress(dto.Email);

            var html = $@"
                <h3>¡Hola {dto.Nombre}!</h3>
                <p>Tu invitación vence el {invitacion.CuotaPagadaHasta?.ToShortDateString()}.</p>
                <img src='data:image/png;base64,{qrImage.Split(',')[1]}' alt='QR' width='200'/>
                <p><a href='{linkActivacion}'>Activar cuenta y crear contraseña</a></p>";

            var msg = MailHelper.CreateSingleEmail(from, to, "Invitación a FitRank", html, html);
            await _sendGridClient.SendEmailAsync(msg);
            return (tokenInvitacion, qrImage);
        }

        private async Task<Socio> RegistrarSocioInvitacion(GenerarInvitacionDTO dto, string tokenActivacion, Domain.Entities.Invitacion invitacion)
        {
            var socio = new Socio
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellidos,
                Dni = dto.Dni,
                Telefono = dto.Telefono,
                Email = dto.Email,
                NombreUsuario = "socio_" + Guid.NewGuid().ToString("N").Substring(0, 6),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString("N")),
                Rol = "Socio",
                Estado = "Activo",
                EsActivado = false,
                Nivel = "Beginner",
                Altura = 0,
                Peso = 0,
                CuotaPagadaHasta = invitacion.CuotaPagadaHasta,
                TokenRecuperacion = tokenActivacion,
                TokenExpira = DateTime.Now.AddHours(24)
            };

            await _usuarioRepositorio.AgregarAsync(socio);
            return socio;
        }

        private async Task<Domain.Entities.Invitacion> GenerarNuevaInvitacionAsync(GenerarInvitacionDTO dto, long adminId)
        {
            var gimnasio = await _gimnasioRepositorio.ObtenerPorAdministradorIdAsync(adminId);

            if (gimnasio == null)
                throw new Exception("No se encontró un gimnasio asociado al administrador.");
            var invitacion = new Domain.Entities.Invitacion
            {
                GimnasioId = gimnasio.Id,
                Email = dto.Email,
                DatosPrellenados = JsonSerializer.Serialize(new
                {
                    nombre = dto.Nombre,
                    apellidos = dto.Apellidos,
                    dni = dto.Dni,
                    telefono = dto.Telefono
                }),
                MetodoPago = dto.MetodoPago ?? "Efectivo",
                CreadaEn = DateTime.Now,
                ExpiraEn = DateTime.Now.AddHours(24),
                Estado = "Pagado",
                CuotaPagadaHasta = dto.Periodo == "Yearly"
                    ? DateTime.Now.AddYears(1)
                    : DateTime.Now.AddMonths(1)
            };

            await _invitacionRepositorio.AgregarAsync(invitacion);
            return invitacion;
        }
    }
}







