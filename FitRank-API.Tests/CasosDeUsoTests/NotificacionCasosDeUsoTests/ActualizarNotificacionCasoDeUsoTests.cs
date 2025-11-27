using FitRank_API.Application.CasosDeUso.NotificacionCasosDeUso;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace FitRank_API.Tests.CasosDeUsoTests.NotificacionCasosDeUsoTests
{
    public class ActualizarNotificacionCasoDeUsoTests
    {
        private readonly Mock<INotificacionRepositorio> _mockRepo;
        private readonly ActualizarNotificacionCasoDeUso _casoDeUso;

        public ActualizarNotificacionCasoDeUsoTests()
        {
            _mockRepo = new Mock<INotificacionRepositorio>();
            _casoDeUso = new ActualizarNotificacionCasoDeUso(_mockRepo.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeActualizarEstadoLeida()
        {
            // Arrange
            var notificacion = new Notificacion
            {
                Id = 1,
                UsuarioEmisorId = 1,
                UsuarioReceptorId = 2,
                Titulo = "Test",
                Mensaje = "Test",
                Leido = false,
                Activa = true
            };

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(1))
                .ReturnsAsync(notificacion);
            _mockRepo.Setup(r => r.ActualizarAsync(It.IsAny<Notificacion>()))
                .ReturnsAsync((Notificacion n) => n);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, leida: true);

            // Assert
            resultado.Should().BeTrue();
            notificacion.Leido.Should().BeTrue();
            _mockRepo.Verify(r => r.ActualizarAsync(It.Is<Notificacion>(n => n.Leido == true)), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeActualizarEstadoActiva()
        {
            // Arrange
            var notificacion = new Notificacion
            {
                Id = 1,
                UsuarioEmisorId = 1,
                UsuarioReceptorId = 2,
                Titulo = "Test",
                Mensaje = "Test",
                Leido = false,
                Activa = true
            };

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(1))
                .ReturnsAsync(notificacion);
            _mockRepo.Setup(r => r.ActualizarAsync(It.IsAny<Notificacion>()))
                .ReturnsAsync((Notificacion n) => n);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, activa: false);

            // Assert
            resultado.Should().BeTrue();
            notificacion.Activa.Should().BeFalse();
            _mockRepo.Verify(r => r.ActualizarAsync(It.Is<Notificacion>(n => n.Activa == false)), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeActualizarAmbosEstados()
        {
            // Arrange
            var notificacion = new Notificacion
            {
                Id = 1,
                UsuarioEmisorId = 1,
                UsuarioReceptorId = 2,
                Titulo = "Test",
                Mensaje = "Test",
                Leido = false,
                Activa = true
            };

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(1))
                .ReturnsAsync(notificacion);
            _mockRepo.Setup(r => r.ActualizarAsync(It.IsAny<Notificacion>()))
                .ReturnsAsync((Notificacion n) => n);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, leida: true, activa: false);

            // Assert
            resultado.Should().BeTrue();
            notificacion.Leido.Should().BeTrue();
            notificacion.Activa.Should().BeFalse();
            _mockRepo.Verify(r => r.ActualizarAsync(It.Is<Notificacion>(n => 
                n.Leido == true && n.Activa == false)), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarFalseSiNotificacionNoExiste()
        {
            // Arrange
            _mockRepo.Setup(r => r.ObtenerPorIdAsync(999))
                .ReturnsAsync((Notificacion?)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(999, leida: true);

            // Assert
            resultado.Should().BeFalse();
            _mockRepo.Verify(r => r.ActualizarAsync(It.IsAny<Notificacion>()), Times.Never);
        }

        [Fact]
        public async Task Ejecutar_NoDebeModificarSiNoSeProporcionanValores()
        {
            // Arrange
            var notificacion = new Notificacion
            {
                Id = 1,
                UsuarioEmisorId = 1,
                UsuarioReceptorId = 2,
                Titulo = "Test",
                Mensaje = "Test",
                Leido = false,
                Activa = true
            };

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(1))
                .ReturnsAsync(notificacion);
            _mockRepo.Setup(r => r.ActualizarAsync(It.IsAny<Notificacion>()))
                .ReturnsAsync((Notificacion n) => n);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado.Should().BeTrue();
            notificacion.Leido.Should().BeFalse();
            notificacion.Activa.Should().BeTrue();
            _mockRepo.Verify(r => r.ActualizarAsync(It.IsAny<Notificacion>()), Times.Once);
        }
    }
}
