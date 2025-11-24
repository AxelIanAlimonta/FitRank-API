using FluentAssertions;
using FitRank_API.Application.CasosDeUso.SocioCasoDeUso;
using FitRank_API.Infrastructure.Interfaces;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.SocioCasosDeUsoTests
{
    public class EliminarSocioCasoDeUsoTests
    {
        private readonly Mock<ISocioRepositorio> _mockRepositorio;
        private readonly EliminarSocioCasoDeUso _casoDeUso;

        public EliminarSocioCasoDeUsoTests()
        {
            _mockRepositorio = new Mock<ISocioRepositorio>();
            _casoDeUso = new EliminarSocioCasoDeUso(_mockRepositorio.Object);
        }

        [Fact]
        public async Task DeberiaEliminarSocioCorrectamente()
        {
            // Arrange
            long socioId = 1;
            _mockRepositorio.Setup(r => r.EliminarAsync(socioId))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId);

            // Assert
            resultado.Should().BeTrue();
            _mockRepositorio.Verify(r => r.EliminarAsync(socioId), Times.Once);
        }
    }
}
