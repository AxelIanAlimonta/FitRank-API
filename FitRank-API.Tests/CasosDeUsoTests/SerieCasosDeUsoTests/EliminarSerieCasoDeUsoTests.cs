using FitRank_API.Application.CasosDeUso.SerieCasosDeUso;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.SerieCasosDeUsoTests
{
    public class EliminarSerieCasoDeUsoTests
    {
        private readonly Mock<ISerieRepositorio> _mockRepo;
        private readonly EliminarSerieCasoDeUso _casoDeUso;

        public EliminarSerieCasoDeUsoTests()
        {
            _mockRepo = new Mock<ISerieRepositorio>();
            _casoDeUso = new EliminarSerieCasoDeUso(_mockRepo.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarTrueSiEliminacionExitosa()
        {
            // Arrange
            _mockRepo.Setup(r => r.EliminarAsync(1)).ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarFalseSiSerieNoExiste()
        {
            // Arrange
            _mockRepo.Setup(r => r.EliminarAsync(99)).ReturnsAsync(false);

            // Act
            var resultado = await _casoDeUso.Ejecutar(99);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task Ejecutar_DebeLlamarRepositorioEliminar()
        {
            // Arrange
            _mockRepo.Setup(r => r.EliminarAsync(5)).ReturnsAsync(true);

            // Act
            await _casoDeUso.Ejecutar(5);

            // Assert
            _mockRepo.Verify(r => r.EliminarAsync(5), Times.Once);
        }
    }
}
