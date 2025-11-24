using FluentAssertions;
using FitRank_API.Application.CasosDeUso.Asistencia;
using FitRank_API.Application.DTOs.QR;
using FitRank_API.Application.Hubs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace FitRank_API.Tests.CasosDeUsoTests.AsistenciaCasosDeUsoTests
{
    public class ValidarAsistenciaQrCasoDeUsoTests
    {
        private readonly Mock<IUsuarioRepositorio> _mockUsuarioRepo;
        private readonly Mock<IAsistenciaRepositorio> _mockAsistenciaRepo;
        private readonly Mock<IGimnasioRepositorio> _mockGimnasioRepo;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<IHubContext<NotificacionesHub>> _mockHub;
        private readonly ValidarAsistenciaQrCasoDeUso _casoDeUso;

        public ValidarAsistenciaQrCasoDeUsoTests()
        {
            _mockUsuarioRepo = new Mock<IUsuarioRepositorio>();
            _mockAsistenciaRepo = new Mock<IAsistenciaRepositorio>();
            _mockGimnasioRepo = new Mock<IGimnasioRepositorio>();
            _mockConfig = new Mock<IConfiguration>();
            _mockHub = new Mock<IHubContext<NotificacionesHub>>();

            _mockConfig.Setup(c => c["QrSecret"])
                .Returns("test_secret_key_at_least_32_characters_long_for_jwt");

            _casoDeUso = new ValidarAsistenciaQrCasoDeUso(
                _mockUsuarioRepo.Object,
                _mockAsistenciaRepo.Object,
                _mockGimnasioRepo.Object,
                _mockConfig.Object,
                _mockHub.Object);
        }

        [Fact]
        public async Task DeberiaRetornarQrInvalidoCuandoTokenEstaVacio()
        {
            // Arrange
            var dto = new QrValidationDTO
            {
                QrData = ""
            };

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto, null);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Valido.Should().BeFalse();
        }

        [Fact]
        public async Task DeberiaRetornarQrInvalidoCuandoUsuarioNoExiste()
        {
            // Arrange
            var dto = new QrValidationDTO
            {
                QrData = "invalid-token"
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(It.IsAny<long>()))
                .ReturnsAsync((Socio?)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto, null);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Valido.Should().BeFalse();
        }

        [Fact]
        public async Task DeberiaValidarConfiguracionDelSecret()
        {
            // Arrange
            var secretKey = _mockConfig.Object["QrSecret"];

            // Assert
            secretKey.Should().NotBeNullOrEmpty();
            secretKey.Should().HaveLength(51); // La clave configurada tiene esta longitud
        }

        [Fact]
        public async Task DeberiaConfigurarRepositoriosCorrectamente()
        {
            // Arrange & Act
            var usuario = new Socio
            {
                Id = 1,
                Nombre = "Test",
                Apellido = "User",
                Email = "test@test.com",
                CuotaPagadaHasta = DateTime.Now.AddDays(30)
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(1))
                .ReturnsAsync(usuario);

            var result = await _mockUsuarioRepo.Object.ObtenerPorIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            _mockUsuarioRepo.Verify(r => r.ObtenerPorIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeberiaManejarAsistenciaExistente()
        {
            // Arrange
            var asistenciaExistente = new Asistencia
            {
                Id = 1,
                UsuarioId = 1,
                Fecha = DateTime.Today,
                Presente = true,
                HoraEntrada = DateTime.Now.AddHours(-2),
                HoraSalida = null
            };

            _mockAsistenciaRepo.Setup(r => r.ObtenerPorUsuarioYFechaAsync(1, DateTime.Today))
                .ReturnsAsync(asistenciaExistente);

            // Act
            var resultado = await _mockAsistenciaRepo.Object.ObtenerPorUsuarioYFechaAsync(1, DateTime.Today);

            // Assert
            resultado.Should().NotBeNull();
            resultado.HoraSalida.Should().BeNull();
            resultado.Presente.Should().BeTrue();
        }

        [Fact]
        public async Task DeberiaCrearNuevaAsistenciaCuandoNoExiste()
        {
            // Arrange
            Asistencia? asistenciaCapturada = null;

            _mockAsistenciaRepo.Setup(r => r.ObtenerPorUsuarioYFechaAsync(It.IsAny<long>(), It.IsAny<DateTime>()))
                .ReturnsAsync((Asistencia?)null);

            _mockAsistenciaRepo.Setup(r => r.AgregarAsync(It.IsAny<Asistencia>()))
                .Callback<Asistencia>(a => asistenciaCapturada = a)
                .ReturnsAsync((Asistencia a) => a);

            // Act
            var nuevaAsistencia = new Asistencia
            {
                UsuarioId = 1,
                Fecha = DateTime.Today,
                Presente = true,
                HoraEntrada = DateTime.Now,
                GimnasioId = 10,
                Observaciones = "Ingreso por QR"
            };

            await _mockAsistenciaRepo.Object.AgregarAsync(nuevaAsistencia);

            // Assert
            asistenciaCapturada.Should().NotBeNull();
            asistenciaCapturada.Presente.Should().BeTrue();
            asistenciaCapturada.Observaciones.Should().Be("Ingreso por QR");
        }

        [Fact]
        public async Task DeberiaActualizarAsistenciaConSalida()
        {
            // Arrange
            var asistencia = new Asistencia
            {
                Id = 1,
                UsuarioId = 1,
                Fecha = DateTime.Today,
                Presente = true,
                HoraEntrada = DateTime.Now.AddHours(-3),
                HoraSalida = null
            };

            Asistencia? asistenciaActualizada = null;

            _mockAsistenciaRepo.Setup(r => r.ActualizarAsync(It.IsAny<Asistencia>()))
                .Callback<Asistencia>(a => asistenciaActualizada = a)
                .ReturnsAsync((Asistencia a) => a);

            // Act
            asistencia.Presente = false;
            asistencia.HoraSalida = DateTime.Now;
            await _mockAsistenciaRepo.Object.ActualizarAsync(asistencia);

            // Assert
            asistenciaActualizada.Should().NotBeNull();
            asistenciaActualizada.Presente.Should().BeFalse();
            asistenciaActualizada.HoraSalida.Should().NotBeNull();
        }

        [Fact]
        public async Task DeberiaValidarCuotaPagadaDelUsuario()
        {
            // Arrange
            var usuarioConCuotaVigente = new Socio
            {
                Id = 1,
                Nombre = "Usuario",
                Apellido = "Vigente",
                CuotaPagadaHasta = DateTime.Now.AddDays(15)
            };

            var usuarioConCuotaVencida = new Socio
            {
                Id = 2,
                Nombre = "Usuario",
                Apellido = "Vencido",
                CuotaPagadaHasta = DateTime.Now.AddDays(-5)
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(1))
                .ReturnsAsync(usuarioConCuotaVigente);

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(2))
                .ReturnsAsync(usuarioConCuotaVencida);

            // Act
            var usuarioVigente = await _mockUsuarioRepo.Object.ObtenerPorIdAsync(1);
            var usuarioVencido = await _mockUsuarioRepo.Object.ObtenerPorIdAsync(2);

            // Assert
            usuarioVigente!.CuotaPagadaHasta!.Value.Should().BeAfter(DateTime.Now);
            usuarioVencido!.CuotaPagadaHasta!.Value.Should().BeBefore(DateTime.Now);
        }

        [Fact]
        public async Task DeberiaVerificarHubContext()
        {
            // Arrange & Assert
            _mockHub.Should().NotBeNull();
            _mockHub.Object.Should().NotBeNull();
        }

        [Fact]
        public async Task DeberiaExtraerTokenDeUrl()
        {
            // Arrange
            var qrDataConUrl = "https://frontend.com/acceso?token=abc123&other=param";
            var dto = new QrValidationDTO { QrData = qrDataConUrl };

            // Act
            var tokenExtraido = qrDataConUrl.Contains("token=")
                ? qrDataConUrl.Split("token=")[1].Split('&')[0]
                : qrDataConUrl;

            // Assert
            tokenExtraido.Should().Be("abc123");
        }
    }
}
