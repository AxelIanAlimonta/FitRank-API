using FitRank_API.Application.CasosDeUso.AmistadCasosDeUso;
using FitRank_API.Application.DTOs.AmistadDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Enums;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.AmistadCasosDeUsoTests
{
    public class EliminarAmigoCasoDeUsoTests
    {
        private readonly Mock<IAmistadRepositorio> _mockRepo;
        private readonly EliminarAmigoCasoDeUso _casoDeUso;

        public EliminarAmigoCasoDeUsoTests()
        {
            _mockRepo = new Mock<IAmistadRepositorio>();
            _casoDeUso = new EliminarAmigoCasoDeUso(_mockRepo.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarFalseSiSocioEsIgualAAmigo()
        {
            // Arrange
            var dto = new EliminarAmigoDTO { SocioId = 5, AmigoId = 5 };

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarFalseSiAmistadNoExiste()
        {
            // Arrange
            var dto = new EliminarAmigoDTO { SocioId = 1, AmigoId = 2 };
            _mockRepo.Setup(r => r.ObtenerPorIdDeSociosAsync(1, 2)).ReturnsAsync((Amistad?)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarFalseSiAmistadNoEstaAceptada()
        {
            // Arrange
            var dto = new EliminarAmigoDTO { SocioId = 1, AmigoId = 2 };
            var amistad = new Amistad { Estado = EstadoAmistad.Pendiente };
            _mockRepo.Setup(r => r.ObtenerPorIdDeSociosAsync(1, 2)).ReturnsAsync(amistad);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task Ejecutar_DebeEliminarAmistadCorrectamente()
        {
            // Arrange
            var dto = new EliminarAmigoDTO { SocioId = 3, AmigoId = 8 };
            var amistad = new Amistad { Id = 1, SocioId1 = 3, SocioId2 = 8, Estado = EstadoAmistad.Aceptado };
            _mockRepo.Setup(r => r.ObtenerPorIdDeSociosAsync(3, 8)).ReturnsAsync(amistad);
            _mockRepo.Setup(r => r.EliminarAsync(amistad)).ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().BeTrue();
            _mockRepo.Verify(r => r.EliminarAsync(amistad), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeUsarMathMinYMaxParaOrdenarIds()
        {
            // Arrange
            var dto = new EliminarAmigoDTO { SocioId = 15, AmigoId = 3 };
            var amistad = new Amistad { Id = 1, SocioId1 = 3, SocioId2 = 15, Estado = EstadoAmistad.Aceptado };
            _mockRepo.Setup(r => r.ObtenerPorIdDeSociosAsync(3, 15)).ReturnsAsync(amistad);
            _mockRepo.Setup(r => r.EliminarAsync(amistad)).ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().BeTrue();
            _mockRepo.Verify(r => r.ObtenerPorIdDeSociosAsync(3, 15), Times.Once);
        }
    }
}
