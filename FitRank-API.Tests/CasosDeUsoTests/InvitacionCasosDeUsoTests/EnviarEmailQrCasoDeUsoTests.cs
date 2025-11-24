using FitRank_API.Application.CasosDeUso.Invitacion;
using FitRank_API.Application.DTOs.UsuarioDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using Xunit;
using FluentAssertions;

namespace FitRank_API.Tests.CasosDeUsoTests.InvitacionCasosDeUsoTests
{
    public class EnviarEmailQrCasoDeUsoTests
    {
        private readonly Mock<IUsuarioRepositorio> _mockUsuarioRepo;
        private readonly Mock<ISendGridClient> _mockSendGridClient;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly EnviarEmailQrCasoDeUso _casoDeUso;

        public EnviarEmailQrCasoDeUsoTests()
        {
            _mockUsuarioRepo = new Mock<IUsuarioRepositorio>();
            _mockSendGridClient = new Mock<ISendGridClient>();
            _mockConfig = new Mock<IConfiguration>();

            _mockConfig.Setup(c => c["FrontendUrl"]).Returns("https://fitrank.com");
            _mockConfig.Setup(c => c["Email:From"]).Returns("noreply@fitrank.com");

            _casoDeUso = new EnviarEmailQrCasoDeUso(
                _mockUsuarioRepo.Object,
                _mockSendGridClient.Object,
                _mockConfig.Object);
        }

        [Fact]
        public async Task DeberiaEnviarEmailCorrectamenteConUsuarioValido()
        {
            // Arrange
            var dto = new EmailDTO
            {
                UsuarioId = 1,
                EmailDestinatario = "user@test.com"
            };

            var usuario = new Socio
            {
                Id = 1,
                Nombre = "Juan",
                Apellido = "Perez",
                Email = "juan@test.com",
                QrToken = "abc123token",
                CuotaPagadaHasta = DateTime.Now.AddMonths(1)
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(dto.UsuarioId))
                .ReturnsAsync(usuario);

            _mockSendGridClient.Setup(s => s.SendEmailAsync(
                It.IsAny<SendGridMessage>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SendGrid.Response(HttpStatusCode.Accepted, null, null));

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Success.Should().BeTrue();
            resultado.Mensaje.Should().Be("Email enviado correctamente");
        }

        [Fact]
        public async Task DeberiaUsarEmailDelUsuarioCuandoDestinatarioEsNulo()
        {
            // Arrange
            var dto = new EmailDTO
            {
                UsuarioId = 1,
                EmailDestinatario = null
            };

            var usuario = new Socio
            {
                Id = 1,
                Nombre = "Maria",
                Apellido = "Lopez",
                Email = "maria@test.com",
                QrToken = "xyz789token",
                CuotaPagadaHasta = DateTime.Now.AddDays(15)
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(dto.UsuarioId))
                .ReturnsAsync(usuario);

            _mockSendGridClient.Setup(s => s.SendEmailAsync(
                It.IsAny<SendGridMessage>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SendGrid.Response(HttpStatusCode.OK, null, null));

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Success.Should().BeTrue();

            _mockSendGridClient.Verify(s => s.SendEmailAsync(
                It.Is<SendGridMessage>(m => 
                    m.Personalizations[0].Tos[0].Email == usuario.Email),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeberiaRetornarErrorCuandoUsuarioNoExiste()
        {
            // Arrange
            var dto = new EmailDTO
            {
                UsuarioId = 999,
                EmailDestinatario = "test@test.com"
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(dto.UsuarioId))
                .ReturnsAsync((Socio)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Success.Should().BeFalse();
            resultado.Mensaje.Should().Be("Usuario no encontrado");

            _mockSendGridClient.Verify(s => s.SendEmailAsync(
                It.IsAny<SendGridMessage>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeberiaRetornarErrorCuandoSendGridFalla()
        {
            // Arrange
            var dto = new EmailDTO
            {
                UsuarioId = 1,
                EmailDestinatario = "user@test.com"
            };

            var usuario = new Socio
            {
                Id = 1,
                Nombre = "Carlos",
                Apellido = "Gomez",
                Email = "carlos@test.com",
                QrToken = "token456",
                CuotaPagadaHasta = DateTime.Now.AddMonths(1)
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(dto.UsuarioId))
                .ReturnsAsync(usuario);

            _mockSendGridClient.Setup(s => s.SendEmailAsync(
                It.IsAny<SendGridMessage>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SendGrid.Response(HttpStatusCode.BadRequest, null, null));

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Success.Should().BeFalse();
            resultado.Mensaje.Should().Contain("Error enviando email");
        }

        [Fact]
        public async Task DeberiaIncluirVencimientoEnEmail()
        {
            // Arrange
            var fechaVencimiento = DateTime.Now.AddMonths(1);
            var dto = new EmailDTO
            {
                UsuarioId = 1,
                EmailDestinatario = "user@test.com"
            };

            var usuario = new Socio
            {
                Id = 1,
                Nombre = "Ana",
                Apellido = "Martinez",
                Email = "ana@test.com",
                QrToken = "tokenABC",
                CuotaPagadaHasta = fechaVencimiento
            };

            string capturedHtmlContent = null;

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(dto.UsuarioId))
                .ReturnsAsync(usuario);

            _mockSendGridClient.Setup(s => s.SendEmailAsync(
                It.IsAny<SendGridMessage>(),
                It.IsAny<CancellationToken>()))
                .Callback<SendGridMessage, CancellationToken>((msg, ct) => capturedHtmlContent = msg.HtmlContent)
                .ReturnsAsync(new SendGrid.Response(HttpStatusCode.Accepted, null, null));

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Success.Should().BeTrue();
            capturedHtmlContent.Should().NotBeNull();
            capturedHtmlContent.Should().Contain(fechaVencimiento.ToShortDateString());
            capturedHtmlContent.Should().Contain(usuario.Nombre);
            capturedHtmlContent.Should().Contain(usuario.Apellido);
        }

        [Fact]
        public async Task DeberiaUsarUrlPorDefectoCuandoFrontendUrlNoEstaConfigurado()
        {
            // Arrange
            _mockConfig.Setup(c => c["FrontendUrl"]).Returns((string)null);

            var dto = new EmailDTO
            {
                UsuarioId = 1,
                EmailDestinatario = "user@test.com"
            };

            var usuario = new Socio
            {
                Id = 1,
                Nombre = "Pedro",
                Apellido = "Sanchez",
                Email = "pedro@test.com",
                QrToken = "token999",
                CuotaPagadaHasta = DateTime.Now.AddMonths(1)
            };

            string capturedPlainText = null;

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(dto.UsuarioId))
                .ReturnsAsync(usuario);

            _mockSendGridClient.Setup(s => s.SendEmailAsync(
                It.IsAny<SendGridMessage>(),
                It.IsAny<CancellationToken>()))
                .Callback<SendGridMessage, CancellationToken>((msg, ct) => capturedPlainText = msg.PlainTextContent)
                .ReturnsAsync(new SendGrid.Response(HttpStatusCode.OK, null, null));

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Success.Should().BeTrue();
            capturedPlainText.Should().Contain("http://localhost:4200/perfil?token=");
        }

        [Fact]
        public async Task DeberiaManejarExcepcionYRetornarError()
        {
            // Arrange
            var dto = new EmailDTO
            {
                UsuarioId = 1,
                EmailDestinatario = "user@test.com"
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(dto.UsuarioId))
                .ThrowsAsync(new Exception("Error de base de datos"));

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Success.Should().BeFalse();
            resultado.Mensaje.Should().Contain("Error inesperado");
            resultado.Mensaje.Should().Contain("Error de base de datos");
        }
    }
}
