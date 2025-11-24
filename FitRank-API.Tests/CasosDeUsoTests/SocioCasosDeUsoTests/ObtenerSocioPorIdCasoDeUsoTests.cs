using AutoMapper;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.SocioCasoDeUso;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.SocioCasosDeUsoTests
{
    public class ObtenerSocioPorIdCasoDeUsoTests
    {
        private readonly Mock<ISocioRepositorio> _mockRepositorio;
        private readonly IMapper _mapper;
        private readonly ObtenerSocioPorIdCasoDeUso _casoDeUso;

        public ObtenerSocioPorIdCasoDeUsoTests()
        {
            _mockRepositorio = new Mock<ISocioRepositorio>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SocioProfile>();
            });
            _mapper = config.CreateMapper();
            _casoDeUso = new ObtenerSocioPorIdCasoDeUso(_mockRepositorio.Object, _mapper);
        }

        [Fact]
        public async Task DeberiaRetornarSocioCuandoExiste()
        {
            // Arrange
            long socioId = 1;
            var socioExistente = new Socio
            {
                Id = 1,
                Nombre = "Juan",
                Apellido = "Pérez",
                Email = "juan@test.com",
                NombreUsuario = "juanp",
                Altura = 1.75,
                Peso = 75.0,
                Nivel = "Intermedio",
                GimnasioId = 1,
                Gimnasio = new Gimnasio { Id = 1, Nombre = "Gimnasio Test" }
            };

            _mockRepositorio.Setup(r => r.ObtenerPorIdAsync(socioId))
                .ReturnsAsync(socioExistente);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Id.Should().Be(1);
            resultado.Nombre.Should().Be("Juan");
            resultado.Apellido.Should().Be("Pérez");
            resultado.NombreUsuario.Should().Be("juanp");
            _mockRepositorio.Verify(r => r.ObtenerPorIdAsync(socioId), Times.Once);
        }

        [Fact]
        public async Task DeberiaRetornarNullCuandoSocioNoExiste()
        {
            // Arrange
            long socioId = 999;
            _mockRepositorio.Setup(r => r.ObtenerPorIdAsync(socioId))
                .ReturnsAsync((Socio?)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId);

            // Assert
            resultado.Should().BeNull();
            _mockRepositorio.Verify(r => r.ObtenerPorIdAsync(socioId), Times.Once);
        }
    }
}
