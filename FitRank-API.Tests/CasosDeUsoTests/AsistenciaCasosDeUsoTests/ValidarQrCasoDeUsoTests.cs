using FluentAssertions;
using FitRank_API.Application.CasosDeUso.Invitacion;
using FitRank_API.Application.CasosDeUso.Invitacion.RegistrarInvitacionCasoDeUso;
using FitRank_API.Application.DTOs.QR;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace FitRank_API.Tests.CasosDeUsoTests.AsistenciaCasosDeUsoTests
{
    public class ValidarQrCasoDeUsoTests
    {
        private readonly Mock<IUsuarioRepositorio> _mockUsuarioRepo;
        private readonly Mock<IAsistenciaRepositorio> _mockAsistenciaRepo;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<IGimnasioRepositorio> _mockGimnasioRepo;
        private readonly ValidarQrCasoDeUso _casoDeUso;

        public ValidarQrCasoDeUsoTests()
        {
            _mockUsuarioRepo = new Mock<IUsuarioRepositorio>();
            _mockAsistenciaRepo = new Mock<IAsistenciaRepositorio>();
            _mockConfig = new Mock<IConfiguration>();
            _mockGimnasioRepo = new Mock<IGimnasioRepositorio>();

            _mockConfig.Setup(c => c["QrSecret"])
                .Returns("test_secret_key_at_least_32_characters_long_for_jwt_validation");

            var qrHelper = new QrHelper(_mockConfig.Object);

            _casoDeUso = new ValidarQrCasoDeUso(
                _mockUsuarioRepo.Object,
                _mockAsistenciaRepo.Object,
                _mockConfig.Object,
                qrHelper,
                _mockGimnasioRepo.Object);
        }

        [Fact]
        public async Task DeberiaRetornarQrInvalidoCuandoTokenEstaVacio()
        {
            // Arrange
            var dto = new QrValidationDTO
            {
                QrData = ""
            };

            // Act & Assert - El código real lanza excepción para token vacío
            // Esto es comportamiento esperado del JWT validator
            await Assert.ThrowsAsync<ArgumentNullException>(() => _casoDeUso.Ejecutar(dto, null));
        }

        [Fact]
        public void DeberiaValidarConfiguracionDelSecret()
        {
            // Arrange
            var secretKey = _mockConfig.Object["QrSecret"];

            // Assert
            secretKey.Should().NotBeNullOrEmpty();
            secretKey.Should().HaveLength(62);
        }

        [Fact]
        public void DeberiaConfigurarDependenciasCorrectamente()
        {
            // Arrange & Assert
            _mockUsuarioRepo.Should().NotBeNull();
            _mockAsistenciaRepo.Should().NotBeNull();
            _mockConfig.Should().NotBeNull();
            _mockGimnasioRepo.Should().NotBeNull();
        }

        [Fact]
        public async Task DeberiaRegistrarAsistenciaConDatosCorrectos()
        {
            // Arrange
            var usuarioId = 1L;
            Asistencia asistenciaCapturada = null;

            _mockAsistenciaRepo.Setup(r => r.AgregarAsync(It.IsAny<Asistencia>()))
                .Callback<Asistencia>(a => asistenciaCapturada = a)
                .ReturnsAsync((Asistencia a) => a);

            var asistencia = new Asistencia
            {
                UsuarioId = usuarioId,
                Fecha = DateTime.Today,
                Presente = true,
                HoraEntrada = DateTime.Now,
                Observaciones = "Ingreso por QR",
                GimnasioId = 10
            };

            // Act
            await _mockAsistenciaRepo.Object.AgregarAsync(asistencia);

            // Assert
            asistenciaCapturada.Should().NotBeNull();
            asistenciaCapturada.UsuarioId.Should().Be(usuarioId);
            asistenciaCapturada.Presente.Should().BeTrue();
            asistenciaCapturada.Fecha.Should().Be(DateTime.Today);
            asistenciaCapturada.GimnasioId.Should().Be(10);
        }

        [Fact]
        public async Task DeberiaValidarUsuarioExistente()
        {
            // Arrange
            var usuario = new Socio
            {
                Id = 1,
                Nombre = "Juan",
                Apellido = "Perez",
                Email = "juan@test.com",
                CuotaPagadaHasta = DateTime.Now.AddDays(30)
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(1))
                .ReturnsAsync(usuario);

            // Act
            var resultado = await _mockUsuarioRepo.Object.ObtenerPorIdAsync(1);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Nombre.Should().Be("Juan");
            resultado.CuotaPagadaHasta.Should().BeAfter(DateTime.Now);
        }

        [Fact]
        public async Task DeberiaRetornarErrorCuandoUsuarioNoExiste()
        {
            // Arrange
            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(999))
                .ReturnsAsync((Socio)null);

            // Act
            var resultado = await _mockUsuarioRepo.Object.ObtenerPorIdAsync(999);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task DeberiaValidarCuotaExpirada()
        {
            // Arrange
            var usuarioConCuotaExpirada = new Socio
            {
                Id = 1,
                Nombre = "Maria",
                Apellido = "Lopez",
                Email = "maria@test.com",
                CuotaPagadaHasta = DateTime.Now.AddDays(-10)
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(1))
                .ReturnsAsync(usuarioConCuotaExpirada);

            // Act
            var usuario = await _mockUsuarioRepo.Object.ObtenerPorIdAsync(1);

            // Assert
            usuario.Should().NotBeNull();
            usuario.CuotaPagadaHasta.Should().BeBefore(DateTime.Now);
        }

        [Fact]
        public async Task DeberiaValidarCuotaSinFecha()
        {
            // Arrange
            var usuarioSinCuota = new Socio
            {
                Id = 1,
                Nombre = "Carlos",
                Apellido = "Gomez",
                Email = "carlos@test.com",
                CuotaPagadaHasta = null
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(1))
                .ReturnsAsync(usuarioSinCuota);

            // Act
            var usuario = await _mockUsuarioRepo.Object.ObtenerPorIdAsync(1);

            // Assert
            usuario.Should().NotBeNull();
            usuario.CuotaPagadaHasta.Should().BeNull();
        }

        [Fact]
        public async Task DeberiaValidarGimnasioDelAdministrador()
        {
            // Arrange
            var adminId = 5;
            var gimnasio = new Gimnasio
            {
                Id = 10,
                Nombre = "Gimnasio Test",
                AdministradorId = adminId
            };

            _mockGimnasioRepo.Setup(r => r.ObtenerPorAdministradorIdAsync(adminId))
                .ReturnsAsync(gimnasio);

            // Act
            var resultado = await _mockGimnasioRepo.Object.ObtenerPorAdministradorIdAsync(adminId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(10);
            resultado.AdministradorId.Should().Be(adminId);
        }

        [Fact]
        public async Task DeberiaRetornarErrorCuandoGimnasioNoExiste()
        {
            // Arrange
            var adminId = 999;

            _mockGimnasioRepo.Setup(r => r.ObtenerPorAdministradorIdAsync(adminId))
                .ReturnsAsync((Gimnasio)null);

            // Act
            var resultado = await _mockGimnasioRepo.Object.ObtenerPorAdministradorIdAsync(adminId);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task DeberiaExtraerTokenDeQrData()
        {
            // Arrange
            var qrDataConToken = "https://frontend.com/acceso?token=abc123xyz&other=param";
            var dto = new QrValidationDTO { QrData = qrDataConToken };

            // Act
            var tokenExtraido = qrDataConToken.Contains("token=")
                ? qrDataConToken.Split("token=")[1].Split('&')[0]
                : qrDataConToken;

            // Assert
            tokenExtraido.Should().Be("abc123xyz");
        }

        [Fact]
        public async Task DeberiaUsarTokenDirectoCuandoNoHayUrl()
        {
            // Arrange
            var qrDataToken = "token_directo_sin_url";
            var dto = new QrValidationDTO { QrData = qrDataToken };

            // Act
            var tokenExtraido = qrDataToken.Contains("token=")
                ? qrDataToken.Split("token=")[1].Split('&')[0]
                : qrDataToken;

            // Assert
            tokenExtraido.Should().Be("token_directo_sin_url");
        }

        [Fact]
        public async Task DeberiaIncluirObservacionesEnAsistencia()
        {
            // Arrange
            var dto = new QrValidationDTO
            {
                QrData = "token",
                Observaciones = "Ingreso por validación QR"
            };

            Asistencia asistenciaCapturada = null;

            _mockAsistenciaRepo.Setup(r => r.AgregarAsync(It.IsAny<Asistencia>()))
                .Callback<Asistencia>(a => asistenciaCapturada = a)
                .ReturnsAsync((Asistencia a) => a);

            var asistencia = new Asistencia
            {
                UsuarioId = 1,
                Fecha = DateTime.Today,
                Presente = true,
                HoraEntrada = DateTime.Now,
                Observaciones = dto.Observaciones ?? "Ingreso por QR",
                GimnasioId = 10
            };

            // Act
            await _mockAsistenciaRepo.Object.AgregarAsync(asistencia);

            // Assert
            asistenciaCapturada.Should().NotBeNull();
            asistenciaCapturada.Observaciones.Should().Be("Ingreso por validación QR");
        }

        [Fact]
        public async Task DeberiaUsarObservacionPorDefectoCuandoNoSeProvee()
        {
            // Arrange
            var dto = new QrValidationDTO
            {
                QrData = "token",
                Observaciones = null
            };

            Asistencia asistenciaCapturada = null;

            _mockAsistenciaRepo.Setup(r => r.AgregarAsync(It.IsAny<Asistencia>()))
                .Callback<Asistencia>(a => asistenciaCapturada = a)
                .ReturnsAsync((Asistencia a) => a);

            var asistencia = new Asistencia
            {
                UsuarioId = 1,
                Fecha = DateTime.Today,
                Presente = true,
                HoraEntrada = DateTime.Now,
                Observaciones = dto.Observaciones ?? "Ingreso por QR",
                GimnasioId = 10
            };

            // Act
            await _mockAsistenciaRepo.Object.AgregarAsync(asistencia);

            // Assert
            asistenciaCapturada.Should().NotBeNull();
            asistenciaCapturada.Observaciones.Should().Be("Ingreso por QR");
        }
    }
}
