using AutoMapper;
using FitRank_API.Application.CasosDeUso.SerieCasosDeUso;
using FitRank_API.Application.DTOs.SerieDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.SerieCasosDeUsoTests
{
    public class ObtenerSeriesCasoDeUsoTests
    {
        private readonly Mock<ISerieRepositorio> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ObtenerSeriesCasoDeUso _casoDeUso;

        public ObtenerSeriesCasoDeUsoTests()
        {
            _mockRepo = new Mock<ISerieRepositorio>();
            _mockMapper = new Mock<IMapper>();
            _casoDeUso = new ObtenerSeriesCasoDeUso(_mockRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarListaVaciaSiNoHaySeries()
        {
            // Arrange
            _mockRepo.Setup(r => r.ObtenerTodasAsync()).ReturnsAsync(new List<Serie>());
            _mockMapper.Setup(m => m.Map<IEnumerable<ObtenerSerieDTO>>(It.IsAny<IEnumerable<Serie>>()))
                .Returns(new List<ObtenerSerieDTO>());

            // Act
            var resultado = await _casoDeUso.Ejecutar();

            // Assert
            resultado.Should().BeEmpty();
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarTodasLasSeries()
        {
            // Arrange
            var series = new List<Serie>
            {
                new Serie { Id = 1, Repeticiones = 10 },
                new Serie { Id = 2, Repeticiones = 15 }
            };

            var seriesDTO = new List<ObtenerSerieDTO>
            {
                new ObtenerSerieDTO { Id = 1, Repeticiones = 10 },
                new ObtenerSerieDTO { Id = 2, Repeticiones = 15 }
            };

            _mockRepo.Setup(r => r.ObtenerTodasAsync()).ReturnsAsync(series);
            _mockMapper.Setup(m => m.Map<IEnumerable<ObtenerSerieDTO>>(series)).Returns(seriesDTO);

            // Act
            var resultado = await _casoDeUso.Ejecutar();

            // Assert
            resultado.Should().HaveCount(2);
        }

        [Fact]
        public async Task Ejecutar_DebeLlamarRepositorioObtenerTodas()
        {
            // Arrange
            _mockRepo.Setup(r => r.ObtenerTodasAsync()).ReturnsAsync(new List<Serie>());
            _mockMapper.Setup(m => m.Map<IEnumerable<ObtenerSerieDTO>>(It.IsAny<IEnumerable<Serie>>()))
                .Returns(new List<ObtenerSerieDTO>());

            // Act
            await _casoDeUso.Ejecutar();

            // Assert
            _mockRepo.Verify(r => r.ObtenerTodasAsync(), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeMapearCorrectamente()
        {
            // Arrange
            var series = new List<Serie> { new Serie { Id = 1 } };
            _mockRepo.Setup(r => r.ObtenerTodasAsync()).ReturnsAsync(series);
            _mockMapper.Setup(m => m.Map<IEnumerable<ObtenerSerieDTO>>(series))
                .Returns(new List<ObtenerSerieDTO> { new ObtenerSerieDTO { Id = 1 } });

            // Act
            await _casoDeUso.Ejecutar();

            // Assert
            _mockMapper.Verify(m => m.Map<IEnumerable<ObtenerSerieDTO>>(series), Times.Once);
        }
    }
}
