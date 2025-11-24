using AutoMapper;
using FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.RutinaCasosDeUsoTests
{
    public class AgregarRutinaCasoDeUsoTests
    {
        private readonly Mock<IRutinaRepositorio> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly AgregarRutinaCasoDeUso _casoDeUso;

        public AgregarRutinaCasoDeUsoTests()
        {
            _mockRepo = new Mock<IRutinaRepositorio>();
            _mockMapper = new Mock<IMapper>();
            _casoDeUso = new AgregarRutinaCasoDeUso(_mockRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeCrearRutinaCorrectamente()
        {
            // Arrange
            var dto = new AgregarRutinaDTO { Nombre = "Rutina Test", Descripcion = "Test" };
            var rutina = new Rutina { Id = 1, Nombre = "Rutina Test" };
            var rutinaDTO = new ObtenerRutinaDTO { Id = 1, Nombre = "Rutina Test" };

            _mockMapper.Setup(m => m.Map<Rutina>(dto)).Returns(rutina);
            _mockRepo.Setup(r => r.AgregarAsync(rutina)).ReturnsAsync(rutina);
            _mockMapper.Setup(m => m.Map<ObtenerRutinaDTO>(rutina)).Returns(rutinaDTO);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(1);
            resultado.Nombre.Should().Be("Rutina Test");
        }

        [Fact]
        public async Task Ejecutar_DebeMapearDTOAEntidad()
        {
            // Arrange
            var dto = new AgregarRutinaDTO { Nombre = "Test" };
            var rutina = new Rutina();

            _mockMapper.Setup(m => m.Map<Rutina>(dto)).Returns(rutina);
            _mockRepo.Setup(r => r.AgregarAsync(It.IsAny<Rutina>())).ReturnsAsync(rutina);
            _mockMapper.Setup(m => m.Map<ObtenerRutinaDTO>(It.IsAny<Rutina>())).Returns(new ObtenerRutinaDTO());

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            _mockMapper.Verify(m => m.Map<Rutina>(dto), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeLlamarRepositorioAgregar()
        {
            // Arrange
            var dto = new AgregarRutinaDTO();
            var rutina = new Rutina();

            _mockMapper.Setup(m => m.Map<Rutina>(dto)).Returns(rutina);
            _mockRepo.Setup(r => r.AgregarAsync(rutina)).ReturnsAsync(rutina);
            _mockMapper.Setup(m => m.Map<ObtenerRutinaDTO>(rutina)).Returns(new ObtenerRutinaDTO());

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            _mockRepo.Verify(r => r.AgregarAsync(rutina), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeMapearEntidadADTO()
        {
            // Arrange
            var dto = new AgregarRutinaDTO();
            var rutina = new Rutina { Id = 5 };

            _mockMapper.Setup(m => m.Map<Rutina>(dto)).Returns(rutina);
            _mockRepo.Setup(r => r.AgregarAsync(rutina)).ReturnsAsync(rutina);
            _mockMapper.Setup(m => m.Map<ObtenerRutinaDTO>(rutina)).Returns(new ObtenerRutinaDTO { Id = 5 });

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            _mockMapper.Verify(m => m.Map<ObtenerRutinaDTO>(rutina), Times.Once);
        }
    }
}
