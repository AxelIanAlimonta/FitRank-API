using FitRank_API.Application.CasosDeUso.NotificacionCasosDeUso;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace FitRank_API.Tests.CasosDeUsoTests.NotificacionCasosDeUsoTests
{
    public class MarcarNotificacionLeidaCasoDeUsoTests
    {
        private readonly Mock<INotificacionRepositorio> _mockRepo;
        private readonly MarcarNotificacionLeidaCasoDeUso _casoDeUso;

        public MarcarNotificacionLeidaCasoDeUsoTests()
        {
            _mockRepo = new Mock<INotificacionRepositorio>();
            _casoDeUso = new MarcarNotificacionLeidaCasoDeUso(_mockRepo.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeMarcarNotificacionComoLeida()
        {
            // Arrange
            var notificacion = new Notificacion
            {
                Id = 1,
                UsuarioEmisorId = 1,
                UsuarioReceptorId = 5,
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
            var resultado = await _casoDeUso.Ejecutar(5, 1);

            // Assert
            resultado.Should().BeTrue();
            notificacion.Leido.Should().BeTrue();
            _mockRepo.Verify(r => r.ActualizarAsync(It.Is<Notificacion>(n => 
                n.Id == 1 && n.Leido == true)), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarFalseSiNotificacionNoExiste()
        {
            // Arrange
            _mockRepo.Setup(r => r.ObtenerPorIdAsync(999))
                .ReturnsAsync((Notificacion?)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(5, 999);

            // Assert
            resultado.Should().BeFalse();
            _mockRepo.Verify(r => r.ActualizarAsync(It.IsAny<Notificacion>()), Times.Never);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarFalseSiUsuarioNoEsReceptor()
        {
            // Arrange
            var notificacion = new Notificacion
            {
                Id = 1,
                UsuarioEmisorId = 1,
                UsuarioReceptorId = 5,
                Titulo = "Test",
                Mensaje = "Test",
                Leido = false,
                Activa = true
            };

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(1))
                .ReturnsAsync(notificacion);

            // Act
            var resultado = await _casoDeUso.Ejecutar(99, 1); // Usuario 99 no es el receptor

            // Assert
            resultado.Should().BeFalse();
            notificacion.Leido.Should().BeFalse(); // No debe haberse modificado
            _mockRepo.Verify(r => r.ActualizarAsync(It.IsAny<Notificacion>()), Times.Never);
        }

        [Fact]
        public async Task Ejecutar_DebePermitirMarcarNotificacionYaLeida()
        {
            // Arrange
            var notificacion = new Notificacion
            {
                Id = 1,
                UsuarioEmisorId = 1,
                UsuarioReceptorId = 5,
                Titulo = "Test",
                Mensaje = "Test",
                Leido = true, // Ya está leída
                Activa = true
            };

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(1))
                .ReturnsAsync(notificacion);
            _mockRepo.Setup(r => r.ActualizarAsync(It.IsAny<Notificacion>()))
                .ReturnsAsync((Notificacion n) => n);

            // Act
            var resultado = await _casoDeUso.Ejecutar(5, 1);

            // Assert
            resultado.Should().BeTrue();
            notificacion.Leido.Should().BeTrue();
            _mockRepo.Verify(r => r.ActualizarAsync(It.IsAny<Notificacion>()), Times.Once);
        }
    }
}
