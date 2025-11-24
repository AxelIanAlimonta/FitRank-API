using FitRank_API.Application.CasosDeUso.NotificacionCasosDeUso;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace FitRank_API.Tests.CasosDeUsoTests.NotificacionCasosDeUsoTests
{
    public class RetenerSocioCasoDeUsoTests
    {
        private readonly Mock<IUsuarioRepositorio> _mockUsuarioRepo;
        private readonly Mock<INotificacionRepositorio> _mockNotiRepo;
        private readonly RetenerSocioCasoDeUso _casoDeUso;

        public RetenerSocioCasoDeUsoTests()
        {
            _mockUsuarioRepo = new Mock<IUsuarioRepositorio>();
            _mockNotiRepo = new Mock<INotificacionRepositorio>();
            _casoDeUso = new RetenerSocioCasoDeUso(_mockUsuarioRepo.Object, _mockNotiRepo.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeCrearNotificacionDeRetencionParaSocio()
        {
            // Arrange
            var socio = new Socio
            {
                Id = 5,
                Nombre = "Juan",
                Email = "juan@test.com",
                GimnasioId = 10
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(5))
                .ReturnsAsync(socio);
            _mockNotiRepo.Setup(r => r.AgregarAsync(It.IsAny<Notificacion>()))
                .ReturnsAsync((Notificacion n) => { n.Id = 100; return n; });

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 5);

            // Assert
            resultado.Should().BeTrue();
            _mockNotiRepo.Verify(r => r.AgregarAsync(It.Is<Notificacion>(n =>
                n.UsuarioEmisorId == 1 &&
                n.UsuarioReceptorId == 5 &&
                n.Titulo == "Te extrañamos en FitRank" &&
                n.Mensaje.Contains("Juan") &&
                n.Activa == true &&
                n.Leido == false
            )), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeLanzarExcepcionSiSocioNoExiste()
        {
            // Arrange
            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(999))
                .ReturnsAsync((Usuario?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _casoDeUso.Ejecutar(1, 999));

            exception.Message.Should().Be("No se encontró el socio seleccionado.");
            _mockNotiRepo.Verify(r => r.AgregarAsync(It.IsAny<Notificacion>()), Times.Never);
        }

        [Fact]
        public async Task Ejecutar_DebeIncluirNombreDelSocioEnMensaje()
        {
            // Arrange
            var socio = new Socio
            {
                Id = 10,
                Nombre = "María",
                Email = "maria@test.com",
                GimnasioId = 20
            };

            Notificacion? notificacionCapturada = null;
            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(10))
                .ReturnsAsync(socio);
            _mockNotiRepo.Setup(r => r.AgregarAsync(It.IsAny<Notificacion>()))
                .ReturnsAsync((Notificacion n) =>
                {
                    notificacionCapturada = n;
                    n.Id = 1;
                    return n;
                });

            // Act
            await _casoDeUso.Ejecutar(1, 10);

            // Assert
            notificacionCapturada.Should().NotBeNull();
            notificacionCapturada!.Mensaje.Should().Contain("María");
            notificacionCapturada.Mensaje.Should().Contain("hace varios días no venís al gimnasio");
        }

        [Fact]
        public async Task Ejecutar_DebeEstablecerFechaEnvioActual()
        {
            // Arrange
            var socio = new Socio
            {
                Id = 5,
                Nombre = "Carlos",
                Email = "carlos@test.com",
                GimnasioId = 10
            };

            Notificacion? notificacionCapturada = null;
            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(5))
                .ReturnsAsync(socio);
            _mockNotiRepo.Setup(r => r.AgregarAsync(It.IsAny<Notificacion>()))
                .ReturnsAsync((Notificacion n) =>
                {
                    notificacionCapturada = n;
                    n.Id = 1;
                    return n;
                });

            var fechaAntes = DateTime.UtcNow;

            // Act
            await _casoDeUso.Ejecutar(1, 5);

            var fechaDespues = DateTime.UtcNow;

            // Assert
            notificacionCapturada.Should().NotBeNull();
            notificacionCapturada!.FechaEnvio.Should().BeAfter(fechaAntes.AddSeconds(-1));
            notificacionCapturada.FechaEnvio.Should().BeBefore(fechaDespues.AddSeconds(1));
        }

        [Fact]
        public async Task Ejecutar_DebeMarcarNotificacionComoActivaYNoLeida()
        {
            // Arrange
            var socio = new Socio
            {
                Id = 5,
                Nombre = "Luis",
                Email = "luis@test.com",
                GimnasioId = 10
            };

            Notificacion? notificacionCapturada = null;
            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(5))
                .ReturnsAsync(socio);
            _mockNotiRepo.Setup(r => r.AgregarAsync(It.IsAny<Notificacion>()))
                .ReturnsAsync((Notificacion n) =>
                {
                    notificacionCapturada = n;
                    n.Id = 1;
                    return n;
                });

            // Act
            await _casoDeUso.Ejecutar(1, 5);

            // Assert
            notificacionCapturada.Should().NotBeNull();
            notificacionCapturada!.Activa.Should().BeTrue();
            notificacionCapturada.Leido.Should().BeFalse();
        }
    }
}
