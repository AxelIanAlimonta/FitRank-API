using FitRank_API.Application.CasosDeUso.SocioCasoDeUso;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.SocioCasosDeUsoTests
{
    public class CambiarParticipacionRankingCasoDeUsoTests
    {
        private readonly Mock<ISocioRepositorio> _mockRepo;
        private readonly CambiarParticipacionRankingCasoDeUso _casoDeUso;

        public CambiarParticipacionRankingCasoDeUsoTests()
        {
            _mockRepo = new Mock<ISocioRepositorio>();
            _casoDeUso = new CambiarParticipacionRankingCasoDeUso(_mockRepo.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeCambiarParticipacionATrue()
        {
            // Arrange
            var socioId = 1L;
            var participa = true;
            _mockRepo.Setup(r => r.CambiarParticipacionRankingAsync(socioId, participa)).ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId, participa);

            // Assert
            resultado.Should().BeTrue();
            _mockRepo.Verify(r => r.CambiarParticipacionRankingAsync(socioId, participa), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeCambiarParticipacionAFalse()
        {
            // Arrange
            var socioId = 1L;
            var participa = false;
            _mockRepo.Setup(r => r.CambiarParticipacionRankingAsync(socioId, participa)).ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId, participa);

            // Assert
            resultado.Should().BeTrue();
            _mockRepo.Verify(r => r.CambiarParticipacionRankingAsync(socioId, participa), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarFalseSiRepositorioFalla()
        {
            // Arrange
            var socioId = 999L;
            _mockRepo.Setup(r => r.CambiarParticipacionRankingAsync(socioId, true)).ReturnsAsync(false);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId, true);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task Ejecutar_DebeLlamarRepositorioConParametrosCorrectos()
        {
            // Arrange
            var socioId = 15L;
            var participa = true;
            _mockRepo.Setup(r => r.CambiarParticipacionRankingAsync(socioId, participa)).ReturnsAsync(true);

            // Act
            await _casoDeUso.Ejecutar(socioId, participa);

            // Assert
            _mockRepo.Verify(r => r.CambiarParticipacionRankingAsync(15L, true), Times.Once);
        }
    }
}
