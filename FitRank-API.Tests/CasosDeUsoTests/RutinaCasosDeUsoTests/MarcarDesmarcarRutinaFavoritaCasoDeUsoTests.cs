using FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.RutinaCasosDeUsoTests
{
    public class MarcarDesmarcarRutinaFavoritaCasoDeUsoTests
    {
        private readonly Mock<IRutinaRepositorio> _mockRepo;
        private readonly MarcarDesmarcarRutinaFavoritaCasoDeUso _casoDeUso;

        public MarcarDesmarcarRutinaFavoritaCasoDeUsoTests()
        {
            _mockRepo = new Mock<IRutinaRepositorio>();
            _casoDeUso = new MarcarDesmarcarRutinaFavoritaCasoDeUso(_mockRepo.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeMarcarComoFavoritaCorrectamente()
        {
            // Arrange
            _mockRepo.Setup(r => r.MarcarFavoritaAsync(1, true)).ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, true);

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact]
        public async Task Ejecutar_DebeDesmarcarFavoritaCorrectamente()
        {
            // Arrange
            _mockRepo.Setup(r => r.MarcarFavoritaAsync(1, false)).ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, false);

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarFalseSiRutinaNoExiste()
        {
            // Arrange
            _mockRepo.Setup(r => r.MarcarFavoritaAsync(99, true)).ReturnsAsync(false);

            // Act
            var resultado = await _casoDeUso.Ejecutar(99, true);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task Ejecutar_DebeLlamarRepositorioConParametrosCorrectos()
        {
            // Arrange
            _mockRepo.Setup(r => r.MarcarFavoritaAsync(5, true)).ReturnsAsync(true);

            // Act
            await _casoDeUso.Ejecutar(5, true);

            // Assert
            _mockRepo.Verify(r => r.MarcarFavoritaAsync(5, true), Times.Once);
        }
    }
}
