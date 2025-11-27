using AutoMapper;
using FitRank_API.Application.CasosDeUso.SerieCasosDeUso;
using FitRank_API.Application.DTOs.SerieDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.SerieCasosDeUsoTests
{
    public class ObtenerSeriePorIdCasoDeUsoTests
    {
        private readonly Mock<ISerieRepositorio> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ObtenerSeriePorIdCasoDeUso _casoDeUso;

        public ObtenerSeriePorIdCasoDeUsoTests()
        {
            _mockRepo = new Mock<ISerieRepositorio>();
            _mockMapper = new Mock<IMapper>();
            _casoDeUso = new ObtenerSeriePorIdCasoDeUso(_mockRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarNullSiSerieNoExiste()
        {
            // Arrange
            _mockRepo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync((Serie?)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarSerieCorrectamente()
        {
            // Arrange
            var serie = new Serie { Id = 1, Repeticiones = 10, Peso = 50 };
            var serieDTO = new ObtenerSerieDTO { Id = 1, Repeticiones = 10, Peso = 50 };

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(serie);
            _mockMapper.Setup(m => m.Map<ObtenerSerieDTO>(serie)).Returns(serieDTO);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Id.Should().Be(1);
            resultado.Repeticiones.Should().Be(10);
        }

        [Fact]
        public async Task Ejecutar_DebeLlamarRepositorioObtenerPorId()
        {
            // Arrange
            _mockRepo.Setup(r => r.ObtenerPorIdAsync(5)).ReturnsAsync((Serie?)null);

            // Act
            await _casoDeUso.Ejecutar(5);

            // Assert
            _mockRepo.Verify(r => r.ObtenerPorIdAsync(5), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeMapearSiSerieExiste()
        {
            // Arrange
            var serie = new Serie { Id = 2 };
            var serieDTO = new ObtenerSerieDTO { Id = 2 };

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(2)).ReturnsAsync(serie);
            _mockMapper.Setup(m => m.Map<ObtenerSerieDTO>(serie)).Returns(serieDTO);

            // Act
            await _casoDeUso.Ejecutar(2);

            // Assert
            _mockMapper.Verify(m => m.Map<ObtenerSerieDTO>(serie), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_NoDebeMapearSiSerieNoExiste()
        {
            // Arrange
            _mockRepo.Setup(r => r.ObtenerPorIdAsync(99)).ReturnsAsync((Serie?)null);

            // Act
            await _casoDeUso.Ejecutar(99);

            // Assert
            _mockMapper.Verify(m => m.Map<ObtenerSerieDTO>(It.IsAny<Serie>()), Times.Never);
        }
    }
}
