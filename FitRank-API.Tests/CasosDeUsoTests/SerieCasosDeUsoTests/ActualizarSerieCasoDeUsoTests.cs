using AutoMapper;
using FitRank_API.Application.CasosDeUso.SerieCasosDeUso;
using FitRank_API.Application.DTOs.SerieDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.SerieCasosDeUsoTests
{
    public class ActualizarSerieCasoDeUsoTests
    {
        private readonly Mock<ISerieRepositorio> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ActualizarSerieCasoDeUso _casoDeUso;

        public ActualizarSerieCasoDeUsoTests()
        {
            _mockRepo = new Mock<ISerieRepositorio>();
            _mockMapper = new Mock<IMapper>();
            _casoDeUso = new ActualizarSerieCasoDeUso(_mockRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarNullSiSerieNoExiste()
        {
            // Arrange
            var dto = new ActualizarSerieDTO { Id = 1 };
            _mockRepo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync((Serie?)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task Ejecutar_DebeActualizarSerieCorrectamente()
        {
            // Arrange
            var dto = new ActualizarSerieDTO { Id = 1, Repeticiones = 15, Peso = 75 };
            var serieExistente = new Serie { Id = 1, Repeticiones = 10, Peso = 50 };
            var serieActualizada = new Serie { Id = 1, Repeticiones = 15, Peso = 75 };
            var serieDTO = new ObtenerSerieDTO { Id = 1, Repeticiones = 15, Peso = 75 };

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(serieExistente);
            _mockMapper.Setup(m => m.Map(dto, serieExistente)).Returns(serieExistente);
            _mockRepo.Setup(r => r.ActualizarAsync(serieExistente)).ReturnsAsync(serieActualizada);
            _mockMapper.Setup(m => m.Map<ObtenerSerieDTO>(serieActualizada)).Returns(serieDTO);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Repeticiones.Should().Be(15);
            resultado.Peso.Should().Be(75);
        }

        [Fact]
        public async Task Ejecutar_DebeMapearDTOASerie()
        {
            // Arrange
            var dto = new ActualizarSerieDTO { Id = 2, Repeticiones = 12 };
            var serie = new Serie { Id = 2 };

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(2)).ReturnsAsync(serie);
            _mockMapper.Setup(m => m.Map(dto, serie)).Returns(serie);
            _mockRepo.Setup(r => r.ActualizarAsync(serie)).ReturnsAsync(serie);
            _mockMapper.Setup(m => m.Map<ObtenerSerieDTO>(serie)).Returns(new ObtenerSerieDTO());

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            _mockMapper.Verify(m => m.Map(dto, serie), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeLlamarRepositorioActualizar()
        {
            // Arrange
            var dto = new ActualizarSerieDTO { Id = 3 };
            var serie = new Serie { Id = 3 };

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(3)).ReturnsAsync(serie);
            _mockMapper.Setup(m => m.Map(dto, serie)).Returns(serie);
            _mockRepo.Setup(r => r.ActualizarAsync(serie)).ReturnsAsync(serie);
            _mockMapper.Setup(m => m.Map<ObtenerSerieDTO>(serie)).Returns(new ObtenerSerieDTO());

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            _mockRepo.Verify(r => r.ActualizarAsync(serie), Times.Once);
        }
    }
}
