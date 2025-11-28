using FitRank_API.Application.CasosDeUso.NotificacionCasosDeUso;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace FitRank_API.Tests.CasosDeUsoTests.NotificacionCasosDeUsoTests
{
    public class EnviarNotificacionIndividualCasoDeUsoTests
    {
        private readonly Mock<INotificacionRepositorio> _mockRepo;
        private readonly EnviarNotificacionIndividualCasoDeUso _casoDeUso;

        public EnviarNotificacionIndividualCasoDeUsoTests()
        {
            _mockRepo = new Mock<INotificacionRepositorio>();
            _casoDeUso = new EnviarNotificacionIndividualCasoDeUso(_mockRepo.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeCrearYEnviarNotificacionIndividual()
        {
            // Arrange
            _mockRepo.Setup(r => r.AgregarAsync(It.IsAny<Notificacion>()))
                .ReturnsAsync((Notificacion n) =>
                {
                    n.Id = 100;
                    return n;
                });

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 5, "Título Test", "Mensaje Test");

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(100);
            resultado.UsuarioEmisorId.Should().Be(1);
            resultado.UsuarioReceptorId.Should().Be(5);
            resultado.Titulo.Should().Be("Título Test");
            resultado.Mensaje.Should().Be("Mensaje Test");

            _mockRepo.Verify(r => r.AgregarAsync(It.Is<Notificacion>(n =>
                n.UsuarioEmisorId == 1 &&
                n.UsuarioReceptorId == 5 &&
                n.Titulo == "Título Test" &&
                n.Mensaje == "Mensaje Test"
            )), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeCrearNotificacionConValoresPorDefecto()
        {
            // Arrange
            Notificacion? notificacionCapturada = null;
            _mockRepo.Setup(r => r.AgregarAsync(It.IsAny<Notificacion>()))
                .ReturnsAsync((Notificacion n) =>
                {
                    notificacionCapturada = n;
                    n.Id = 50;
                    return n;
                });

            // Act
            await _casoDeUso.Ejecutar(10, 20, "Título", "Mensaje");

            // Assert
            notificacionCapturada.Should().NotBeNull();
            notificacionCapturada!.UsuarioEmisorId.Should().Be(10);
            notificacionCapturada.UsuarioReceptorId.Should().Be(20);
            notificacionCapturada.Titulo.Should().Be("Título");
            notificacionCapturada.Mensaje.Should().Be("Mensaje");
        }

        [Fact]
        public async Task Ejecutar_DebePermitirEnviarMensajesVacios()
        {
            // Arrange
            _mockRepo.Setup(r => r.AgregarAsync(It.IsAny<Notificacion>()))
                .ReturnsAsync((Notificacion n) =>
                {
                    n.Id = 1;
                    return n;
                });

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 2, string.Empty, string.Empty);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Titulo.Should().Be(string.Empty);
            resultado.Mensaje.Should().Be(string.Empty);
            _mockRepo.Verify(r => r.AgregarAsync(It.IsAny<Notificacion>()), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebePermitirEnviarMensajesLargos()
        {
            // Arrange
            var mensajeLargo = new string('A', 1000);
            _mockRepo.Setup(r => r.AgregarAsync(It.IsAny<Notificacion>()))
                .ReturnsAsync((Notificacion n) =>
                {
                    n.Id = 1;
                    return n;
                });

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 2, "Título", mensajeLargo);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Mensaje.Should().Be(mensajeLargo);
            resultado.Mensaje.Length.Should().Be(1000);
        }
    }
}
