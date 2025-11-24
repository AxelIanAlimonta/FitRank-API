using FluentAssertions;
using FitRank_API.Application.CasosDeUso.PuntajeCasosDeUso;
using FitRank_API.Infrastructure.Interfaces;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.PuntajeCasosDeUsoTests
{
    public class EliminarPuntajeCasoDeUsoTests
    {
        private readonly Mock<IPuntajeRepositorio> _mockRepositorio;
        private readonly EliminarPuntajeCasoDeUso _casoDeUso;

        public EliminarPuntajeCasoDeUsoTests()
        {
            _mockRepositorio = new Mock<IPuntajeRepositorio>();
            _casoDeUso = new EliminarPuntajeCasoDeUso(_mockRepositorio.Object);
        }

        [Fact]
        public async Task DeberiaEliminarPuntajeCorrectamente()
        {
            // Arrange
            long puntajeId = 1;
            _mockRepositorio.Setup(r => r.EliminarAsync(puntajeId))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Ejecutar(puntajeId);

            // Assert
            resultado.Should().BeTrue();
            _mockRepositorio.Verify(r => r.EliminarAsync(puntajeId), Times.Once);
        }
    }
}
