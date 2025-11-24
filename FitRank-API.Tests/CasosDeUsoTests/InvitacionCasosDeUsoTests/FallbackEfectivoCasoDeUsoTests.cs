using FitRank_API.Application.CasosDeUso.Invitacion;
using FitRank_API.Application.DTOs.Invitacion;
using FitRank_API.Application.Helpers;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using FluentAssertions;

namespace FitRank_API.Tests.CasosDeUsoTests.InvitacionCasosDeUsoTests
{
    public class FallbackEfectivoCasoDeUsoTests
    {
        private readonly Mock<IInvitacionRepositorio> _mockInvitacionRepo;
        private readonly Mock<IGimnasioRepositorio> _mockGimnasioRepo;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<QrHelper> _mockQrHelper;
        private readonly FallbackEfectivoCasoDeUso _casoDeUso;

        public FallbackEfectivoCasoDeUsoTests()
        {
            _mockInvitacionRepo = new Mock<IInvitacionRepositorio>();
            _mockGimnasioRepo = new Mock<IGimnasioRepositorio>();
            _mockConfig = new Mock<IConfiguration>();
            _mockQrHelper = new Mock<QrHelper>(Mock.Of<IConfiguration>());

            _mockConfig.Setup(c => c["FrontendUrl"]).Returns("https://fitrank.com");

            _casoDeUso = new FallbackEfectivoCasoDeUso(
                _mockInvitacionRepo.Object,
                _mockGimnasioRepo.Object,
                _mockConfig.Object,
                _mockQrHelper.Object);
        }

        [Fact]
        public async Task DeberiaConvertirInvitacionAFallbackEfectivoCorrectamente()
        {
            // Arrange
            var adminId = 1;
            var gimnasioId = 10L;
            var invitacionId = 5L;

            var dto = new FallbackEfectivoDTO
            {
                InvitacionId = invitacionId,
                ConfirmarEfectivo = true
            };

            var gimnasio = new Gimnasio { Id = gimnasioId, Nombre = "Gym Test" };
            
            var invitacion = new Invitacion
            {
                Id = invitacionId,
                Email = "test@test.com",
                Estado = "Pendiente",
                MetodoPago = "MercadoPago",
                GimnasioId = 20L,
                CreadaEn = DateTime.Now,
                ExpiraEn = DateTime.Now.AddHours(24)
            };

            Invitacion invitacionActualizada = null;

            _mockGimnasioRepo.Setup(r => r.ObtenerPorAdministradorIdAsync(adminId))
                .ReturnsAsync(gimnasio);

            _mockInvitacionRepo.Setup(r => r.ObtenerPorIdYEstadoAsync(invitacionId, "Pendiente"))
                .ReturnsAsync(invitacion);

            _mockInvitacionRepo.Setup(r => r.ActualizarAsync(It.IsAny<Invitacion>()))
                .Callback<Invitacion>(inv => invitacionActualizada = inv)
                .ReturnsAsync((Invitacion inv) => inv);

            _mockQrHelper.Setup(q => q.GenerarQrToken(It.IsAny<Invitacion>()))
                .Returns("fallback-token-123");

            _mockQrHelper.Setup(q => q.GenerarQrImage(It.IsAny<string>()))
                .ReturnsAsync("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==");

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto, adminId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Success.Should().BeTrue();
            resultado.InvitacionId.Should().Be(invitacionId);
            resultado.TokenInvitacion.Should().Be("fallback-token-123");
            resultado.QrImage.Should().Contain("data:image/png;base64");
            resultado.Mensaje.Should().Contain("Pago confirmado en efectivo");
            resultado.Mensaje.Should().Contain("30 días");

            invitacionActualizada.Should().NotBeNull();
            invitacionActualizada.Estado.Should().Be("FallbackEfectivo");
            invitacionActualizada.MetodoPago.Should().Be("Efectivo");
            invitacionActualizada.GimnasioId.Should().Be(gimnasioId);
            invitacionActualizada.CuotaPagadaHasta.Should().NotBeNull();
            invitacionActualizada.CuotaPagadaHasta.Value.Should().BeCloseTo(DateTime.Now.AddDays(30), TimeSpan.FromSeconds(5));

            _mockGimnasioRepo.Verify(r => r.ObtenerPorAdministradorIdAsync(adminId), Times.Once);
           
        }

        [Fact]
        public async Task DeberiaRetornarErrorCuandoGimnasioNoExiste()
        {
            // Arrange
            var adminId = 999;
            var dto = new FallbackEfectivoDTO
            {
                InvitacionId = 1,
                ConfirmarEfectivo = true
            };

            _mockGimnasioRepo.Setup(r => r.ObtenerPorAdministradorIdAsync(adminId))
                .ReturnsAsync((Gimnasio)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto, adminId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Success.Should().BeFalse();
            resultado.Mensaje.Should().Contain("No se encontró un gimnasio");
        }

        [Fact]
        public async Task DeberiaRetornarErrorCuandoInvitacionNoExiste()
        {
            // Arrange
            var adminId = 1;
            var gimnasioId = 10L;
            var invitacionId = 999L;

            var dto = new FallbackEfectivoDTO
            {
                InvitacionId = invitacionId,
                ConfirmarEfectivo = true
            };

            var gimnasio = new Gimnasio { Id = gimnasioId, Nombre = "Gym Test" };

            _mockGimnasioRepo.Setup(r => r.ObtenerPorAdministradorIdAsync(adminId))
                .ReturnsAsync(gimnasio);

            _mockInvitacionRepo.Setup(r => r.ObtenerPorIdYEstadoAsync(invitacionId, "Pendiente"))
                .ReturnsAsync((Invitacion)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto, adminId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Success.Should().BeFalse();
            resultado.Mensaje.Should().Contain("Invitación no encontrada o ya procesada");

            _mockInvitacionRepo.Verify(r => r.ActualizarAsync(It.IsAny<Invitacion>()), Times.Never);
        }

        [Fact]
        public async Task DeberiaRegenerarQrConUrlCorrecta()
        {
            // Arrange
            var adminId = 1;
            var invitacionId = 5L;

            var dto = new FallbackEfectivoDTO
            {
                InvitacionId = invitacionId,
                ConfirmarEfectivo = true
            };

            var gimnasio = new Gimnasio { Id = 10L, Nombre = "Gym Test" };
            var invitacion = new Invitacion
            {
                Id = invitacionId,
                Email = "test@test.com",
                Estado = "Pendiente",
                MetodoPago = "MercadoPago"
            };

            string capturedQrData = null;

            _mockGimnasioRepo.Setup(r => r.ObtenerPorAdministradorIdAsync(adminId))
                .ReturnsAsync(gimnasio);

            _mockInvitacionRepo.Setup(r => r.ObtenerPorIdYEstadoAsync(invitacionId, "Pendiente"))
                .ReturnsAsync(invitacion);

            _mockInvitacionRepo.Setup(r => r.ActualizarAsync(It.IsAny<Invitacion>()))
                .ReturnsAsync((Invitacion inv) => inv);

            _mockQrHelper.Setup(q => q.GenerarQrToken(It.IsAny<Invitacion>()))
                .Returns("token-fallback");

            _mockQrHelper.Setup(q => q.GenerarQrImage(It.IsAny<string>()))
                .Callback<string>(qrData => capturedQrData = qrData)
                .ReturnsAsync("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==");

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto, adminId);

            // Assert
            resultado.Success.Should().BeTrue();
            capturedQrData.Should().NotBeNull();
            capturedQrData.Should().Contain("https://fitrank.com/invitacion?token=token-fallback");
        }

        [Fact]
        public async Task DeberiaAsignarGimnasioIdDelAdministrador()
        {
            // Arrange
            var adminId = 1;
            var gimnasioDelAdmin = 50L;
            var gimnasioOriginal = 30L;

            var dto = new FallbackEfectivoDTO
            {
                InvitacionId = 1,
                ConfirmarEfectivo = true
            };

            var gimnasio = new Gimnasio { Id = gimnasioDelAdmin, Nombre = "Gym Admin" };
            var invitacion = new Invitacion
            {
                Id = 1,
                Email = "test@test.com",
                Estado = "Pendiente",
                GimnasioId = gimnasioOriginal
            };

            Invitacion invitacionCapturada = null;

            _mockGimnasioRepo.Setup(r => r.ObtenerPorAdministradorIdAsync(adminId))
                .ReturnsAsync(gimnasio);

            _mockInvitacionRepo.Setup(r => r.ObtenerPorIdYEstadoAsync(1, "Pendiente"))
                .ReturnsAsync(invitacion);

            _mockInvitacionRepo.Setup(r => r.ActualizarAsync(It.IsAny<Invitacion>()))
                .Callback<Invitacion>(inv => invitacionCapturada = inv)
                .ReturnsAsync((Invitacion inv) => inv);

            _mockQrHelper.Setup(q => q.GenerarQrToken(It.IsAny<Invitacion>()))
                .Returns("token");

            _mockQrHelper.Setup(q => q.GenerarQrImage(It.IsAny<string>()))
                .ReturnsAsync("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==");

            // Act
            await _casoDeUso.Ejecutar(dto, adminId);

            // Assert
            invitacionCapturada.Should().NotBeNull();
            invitacionCapturada.GimnasioId.Should().Be(gimnasioDelAdmin);
            invitacionCapturada.GimnasioId.Should().NotBe(gimnasioOriginal);
        }

        [Fact]
        public async Task DeberiaUsarUrlPorDefectoCuandoFrontendUrlNoEstaConfigurado()
        {
            // Arrange
            _mockConfig.Setup(c => c["FrontendUrl"]).Returns((string)null);

            var adminId = 1;
            var dto = new FallbackEfectivoDTO
            {
                InvitacionId = 1,
                ConfirmarEfectivo = true
            };

            var gimnasio = new Gimnasio { Id = 10L, Nombre = "Gym Test" };
            var invitacion = new Invitacion
            {
                Id = 1,
                Email = "test@test.com",
                Estado = "Pendiente"
            };

            string capturedQrData = null;

            _mockGimnasioRepo.Setup(r => r.ObtenerPorAdministradorIdAsync(adminId))
                .ReturnsAsync(gimnasio);

            _mockInvitacionRepo.Setup(r => r.ObtenerPorIdYEstadoAsync(1, "Pendiente"))
                .ReturnsAsync(invitacion);

            _mockInvitacionRepo.Setup(r => r.ActualizarAsync(It.IsAny<Invitacion>()))
                .ReturnsAsync((Invitacion inv) => inv);

            _mockQrHelper.Setup(q => q.GenerarQrToken(It.IsAny<Invitacion>()))
                .Returns("token");

            _mockQrHelper.Setup(q => q.GenerarQrImage(It.IsAny<string>()))
                .Callback<string>(qrData => capturedQrData = qrData)
                .ReturnsAsync("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==");

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto, adminId);

            // Assert
            resultado.Success.Should().BeTrue();
            capturedQrData.Should().Contain("http://localhost:4200/invitacion?token=");
        }
    }
}
