using FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.RutinaCasosDeUsoTests
{
    public class CambiarEstadoRutinaCasoDeUsoTests
    {
        private readonly Mock<IRutinaRepositorio> _mockRepo;
        private readonly CambiarEstadoRutinaCasoDeUso _casoDeUso;

        public CambiarEstadoRutinaCasoDeUsoTests()
        {
            _mockRepo = new Mock<IRutinaRepositorio>();
            _casoDeUso = new CambiarEstadoRutinaCasoDeUso(_mockRepo.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeActivarRutinaCorrectamente()
        {
            // Arrange
            _mockRepo.Setup(r => r.CambiarEstadoRutinaAsync(1, true)).ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, true);

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact]
        public async Task Ejecutar_DebeDesactivarRutinaCorrectamente()
        {
            // Arrange
            _mockRepo.Setup(r => r.CambiarEstadoRutinaAsync(1, false)).ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, false);

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarFalseSiRutinaNoExiste()
        {
            // Arrange
            _mockRepo.Setup(r => r.CambiarEstadoRutinaAsync(99, true)).ReturnsAsync(false);

            // Act
            var resultado = await _casoDeUso.Ejecutar(99, true);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task Ejecutar_DebeLlamarRepositorioConParametrosCorrectos()
        {
            // Arrange
            _mockRepo.Setup(r => r.CambiarEstadoRutinaAsync(7, false)).ReturnsAsync(true);

            // Act
            await _casoDeUso.Ejecutar(7, false);

            // Assert
            _mockRepo.Verify(r => r.CambiarEstadoRutinaAsync(7, false), Times.Once);
        }
    }
}
