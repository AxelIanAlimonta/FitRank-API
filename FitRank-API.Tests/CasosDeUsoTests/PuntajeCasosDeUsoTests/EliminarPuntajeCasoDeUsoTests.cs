using FluentAssertions;
using FitRank_API.Application.CasosDeUso.PuntajeCasosDeUso;
using FitRank_API.Domain.Interfaces;
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

        [Fact]
        public async Task DeberiaRetornarFalseCuandoPuntajeNoExiste()
        {
            // Arrange
            long puntajeId = 999;
            _mockRepositorio.Setup(r => r.EliminarAsync(puntajeId))
                .ReturnsAsync(false);

            // Act
            var resultado = await _casoDeUso.Ejecutar(puntajeId);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task DebeLlamarRepositorioConIdCorrecto()
        {
            // Arrange
            long puntajeId = 456;

            _mockRepositorio.Setup(r => r.EliminarAsync(puntajeId))
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Ejecutar(puntajeId);

            // Assert
            _mockRepositorio.Verify(r => r.EliminarAsync(puntajeId), Times.Once);
        }

        [Fact]
        public async Task DebeRetornarResultadoDelRepositorio()
        {
            // Arrange
            long puntajeId1 = 1;
            long puntajeId2 = 2;

            _mockRepositorio.Setup(r => r.EliminarAsync(puntajeId1))
                .ReturnsAsync(true);

            _mockRepositorio.Setup(r => r.EliminarAsync(puntajeId2))
                .ReturnsAsync(false);

            // Act
            var resultado1 = await _casoDeUso.Ejecutar(puntajeId1);
            var resultado2 = await _casoDeUso.Ejecutar(puntajeId2);

            // Assert
            resultado1.Should().BeTrue();
            resultado2.Should().BeFalse();
        }

        [Fact]
        public async Task DeberiaEliminarPuntajesConDiferentesIds()
        {
            // Arrange
            long[] ids = { 1L, 100L, 999L };

            foreach (var id in ids)
            {
                _mockRepositorio.Setup(r => r.EliminarAsync(id))
                    .ReturnsAsync(true);
            }

            // Act & Assert
            foreach (var id in ids)
            {
                var resultado = await _casoDeUso.Ejecutar(id);
                resultado.Should().BeTrue();
            }

            _mockRepositorio.Verify(r => r.EliminarAsync(It.IsAny<long>()), Times.Exactly(3));
        }
    }
}
