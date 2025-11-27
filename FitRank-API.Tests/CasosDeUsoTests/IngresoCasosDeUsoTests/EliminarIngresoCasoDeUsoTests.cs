using FluentAssertions;
using FitRank_API.Application.CasosDeUso.Ingreso;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using Moq;
using Xunit;

namespace FitRank_API.Tests.CasosDeUsoTests.IngresoCasosDeUsoTests
{
    public class EliminarIngresoCasoDeUsoTests
    {
        private readonly Mock<IIngresoRepositorio> _mockRepositorio;
        private readonly EliminarIngresoCasoDeUso _casoDeUso;

        public EliminarIngresoCasoDeUsoTests()
        {
            _mockRepositorio = new Mock<IIngresoRepositorio>();
            _casoDeUso = new EliminarIngresoCasoDeUso(_mockRepositorio.Object);
        }

        [Fact]
        public async Task DeberiaEliminarIngresoCuandoExiste()
        {
            // Arrange
            long ingresoId = 1;
            var ingresoExistente = new Ingreso
            {
                Id = ingresoId,
                GimnasioId = 1,
                Monto = 1000,
                MetodoPago = "Efectivo"
            };

            _mockRepositorio.Setup(r => r.ObtenerPorIdAsync(ingresoId))
                .ReturnsAsync(ingresoExistente);
            _mockRepositorio.Setup(r => r.EliminarAsync(ingresoExistente))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _casoDeUso.Ejecutar(ingresoId);

            // Assert
            resultado.Should().BeTrue();
            _mockRepositorio.Verify(r => r.ObtenerPorIdAsync(ingresoId), Times.Once);
            _mockRepositorio.Verify(r => r.EliminarAsync(ingresoExistente), Times.Once);
        }

        [Fact]
        public async Task DeberiaRetornarFalseCuandoIngresoNoExiste()
        {
            // Arrange
            long ingresoId = 999;
            _mockRepositorio.Setup(r => r.ObtenerPorIdAsync(ingresoId))
                .ReturnsAsync((Ingreso?)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(ingresoId);

            // Assert
            resultado.Should().BeFalse();
            _mockRepositorio.Verify(r => r.ObtenerPorIdAsync(ingresoId), Times.Once);
            _mockRepositorio.Verify(r => r.EliminarAsync(It.IsAny<Ingreso>()), Times.Never);
        }

        [Fact]
        public async Task NoDeberiaLlamarEliminarCuandoIngresoNoExiste()
        {
            // Arrange
            long ingresoId = 100;
            _mockRepositorio.Setup(r => r.ObtenerPorIdAsync(ingresoId))
                .ReturnsAsync((Ingreso?)null);

            // Act
            await _casoDeUso.Ejecutar(ingresoId);

            // Assert
            _mockRepositorio.Verify(r => r.EliminarAsync(It.IsAny<Ingreso>()), Times.Never);
        }

        [Fact]
        public async Task DebeBuscarIngresoPorIdCorrecto()
        {
            // Arrange
            long ingresoId = 42;
            var ingreso = new Ingreso { Id = ingresoId };

            _mockRepositorio.Setup(r => r.ObtenerPorIdAsync(ingresoId))
                .ReturnsAsync(ingreso);
            _mockRepositorio.Setup(r => r.EliminarAsync(ingreso))
                .Returns(Task.CompletedTask);

            // Act
            await _casoDeUso.Ejecutar(ingresoId);

            // Assert
            _mockRepositorio.Verify(r => r.ObtenerPorIdAsync(ingresoId), Times.Once);
        }

        [Fact]
        public async Task DebeEliminarElIngresoCorrectoCuandoExiste()
        {
            // Arrange
            long ingresoId = 5;
            var ingresoEsperado = new Ingreso
            {
                Id = ingresoId,
                GimnasioId = 2,
                Monto = 2500
            };

            Ingreso? ingresoEliminado = null;
            _mockRepositorio.Setup(r => r.ObtenerPorIdAsync(ingresoId))
                .ReturnsAsync(ingresoEsperado);
            _mockRepositorio.Setup(r => r.EliminarAsync(It.IsAny<Ingreso>()))
                .Callback<Ingreso>(i => ingresoEliminado = i)
                .Returns(Task.CompletedTask);

            // Act
            await _casoDeUso.Ejecutar(ingresoId);

            // Assert
            ingresoEliminado.Should().NotBeNull();
            ingresoEliminado.Should().Be(ingresoEsperado);
        }
    }
}
