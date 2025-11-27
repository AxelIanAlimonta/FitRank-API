using AutoMapper;
using FitRank_API.Application.CasosDeUso.SerieCasosDeUso;
using FitRank_API.Application.DTOs.SerieDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.SerieCasosDeUsoTests
{
    public class AgregarSerieCasoDeUsoTests
    {
        private readonly Mock<ISerieRepositorio> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly AgregarSerieCasoDeUso _casoDeUso;

        public AgregarSerieCasoDeUsoTests()
        {
            _mockRepo = new Mock<ISerieRepositorio>();
            _mockMapper = new Mock<IMapper>();
            _casoDeUso = new AgregarSerieCasoDeUso(_mockRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeCrearSerieCorrectamente()
        {
            // Arrange
            var dto = new AgregarSerieDTO { Repeticiones = 10, Peso = 50 };
            var serie = new Serie { Id = 1, Repeticiones = 10, Peso = 50 };
            var serieDTO = new ObtenerSerieDTO { Id = 1, Repeticiones = 10, Peso = 50 };

            _mockMapper.Setup(m => m.Map<Serie>(dto)).Returns(serie);
            _mockRepo.Setup(r => r.AgregarAsync(serie)).ReturnsAsync(serie);
            _mockMapper.Setup(m => m.Map<ObtenerSerieDTO>(serie)).Returns(serieDTO);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(1);
            resultado.Repeticiones.Should().Be(10);
        }

        [Fact]
        public async Task Ejecutar_DebeMapearDTOAEntidad()
        {
            // Arrange
            var dto = new AgregarSerieDTO { Repeticiones = 5, Peso = 100 };
            var serie = new Serie { Repeticiones = 5, Peso = 100 };

            _mockMapper.Setup(m => m.Map<Serie>(dto)).Returns(serie);
            _mockRepo.Setup(r => r.AgregarAsync(It.IsAny<Serie>())).ReturnsAsync(serie);
            _mockMapper.Setup(m => m.Map<ObtenerSerieDTO>(It.IsAny<Serie>())).Returns(new ObtenerSerieDTO());

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            _mockMapper.Verify(m => m.Map<Serie>(dto), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeLlamarRepositorioAgregar()
        {
            // Arrange
            var dto = new AgregarSerieDTO();
            var serie = new Serie();

            _mockMapper.Setup(m => m.Map<Serie>(dto)).Returns(serie);
            _mockRepo.Setup(r => r.AgregarAsync(serie)).ReturnsAsync(serie);
            _mockMapper.Setup(m => m.Map<ObtenerSerieDTO>(serie)).Returns(new ObtenerSerieDTO());

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            _mockRepo.Verify(r => r.AgregarAsync(serie), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeMapearEntidadADTO()
        {
            // Arrange
            var dto = new AgregarSerieDTO();
            var serie = new Serie { Id = 5 };

            _mockMapper.Setup(m => m.Map<Serie>(dto)).Returns(serie);
            _mockRepo.Setup(r => r.AgregarAsync(serie)).ReturnsAsync(serie);
            _mockMapper.Setup(m => m.Map<ObtenerSerieDTO>(serie)).Returns(new ObtenerSerieDTO { Id = 5 });

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            _mockMapper.Verify(m => m.Map<ObtenerSerieDTO>(serie), Times.Once);
        }
    }
}
