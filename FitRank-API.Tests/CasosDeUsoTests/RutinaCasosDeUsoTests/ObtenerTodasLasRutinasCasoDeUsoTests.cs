using AutoMapper;
using FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace FitRank_API.Tests.CasosDeUsoTests.RutinaCasosDeUsoTests
{
    public class ObtenerTodasLasRutinasCasoDeUsoTests
    {
        private readonly Mock<IRutinaRepositorio> _mockRepo;
        private readonly IMapper _mapper;
        private readonly ObtenerTodasLasRutinasCasoDeUso _casoDeUso;

        public ObtenerTodasLasRutinasCasoDeUsoTests()
        {
            _mockRepo = new Mock<IRutinaRepositorio>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<RutinaProfile>();
            });
            _mapper = config.CreateMapper();
            _casoDeUso = new ObtenerTodasLasRutinasCasoDeUso(_mockRepo.Object, _mapper);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarTodasLasRutinas()
        {
            // Arrange
            var rutinas = new List<Rutina>
            {
                new Rutina { Id = 1, Nombre = "Rutina 1", Descripcion = "Desc 1" },
                new Rutina { Id = 2, Nombre = "Rutina 2", Descripcion = "Desc 2" },
                new Rutina { Id = 3, Nombre = "Rutina 3", Descripcion = "Desc 3" }
            };

            _mockRepo.Setup(r => r.ObtenerTodasAsync()).ReturnsAsync(rutinas);

            // Act
            var resultado = await _casoDeUso.Ejecutar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(3);
            resultado[0].Nombre.Should().Be("Rutina 1");
            resultado[1].Nombre.Should().Be("Rutina 2");
            resultado[2].Nombre.Should().Be("Rutina 3");
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarListaVacia_CuandoNoHayRutinas()
        {
            // Arrange
            _mockRepo.Setup(r => r.ObtenerTodasAsync()).ReturnsAsync(new List<Rutina>());

            // Act
            var resultado = await _casoDeUso.Ejecutar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
        }

        [Fact]
        public async Task Ejecutar_DebeLlamarRepositorio()
        {
            // Arrange
            _mockRepo.Setup(r => r.ObtenerTodasAsync()).ReturnsAsync(new List<Rutina>());

            // Act
            await _casoDeUso.Ejecutar();

            // Assert
            _mockRepo.Verify(r => r.ObtenerTodasAsync(), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeMapearCorrectamenteLasPropiedades()
        {
            // Arrange
            var rutinas = new List<Rutina>
            {
                new Rutina
                {
                    Id = 10,
                    Nombre = "Rutina Test",
                    Descripcion = "Descripción Test",
                    TipoCreacion = "Manual",
                    Activa = true,
                    Favorita = false,
                    SocioId = 5,
                    UsuarioId = 100
                }
            };

            _mockRepo.Setup(r => r.ObtenerTodasAsync()).ReturnsAsync(rutinas);

            // Act
            var resultado = await _casoDeUso.Ejecutar();

            // Assert
            resultado.First().Id.Should().Be(10);
            resultado.First().Nombre.Should().Be("Rutina Test");
            resultado.First().Descripcion.Should().Be("Descripción Test");
            resultado.First().TipoCreacion.Should().Be("Manual");
            resultado.First().Activa.Should().BeTrue();
            resultado.First().Favorita.Should().BeFalse();
            resultado.First().SocioId.Should().Be(5);
            resultado.First().UsuarioId.Should().Be(100);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarListaDeObtenerRutinaDTO()
        {
            // Arrange
            var rutinas = new List<Rutina>
            {
                new Rutina { Id = 1, Nombre = "Test" }
            };

            _mockRepo.Setup(r => r.ObtenerTodasAsync()).ReturnsAsync(rutinas);

            // Act
            var resultado = await _casoDeUso.Ejecutar();

            // Assert
            resultado.Should().AllBeOfType<ObtenerRutinaDTO>();
        }

        [Fact]
        public async Task Ejecutar_DebeManejarVariasRutinas()
        {
            // Arrange
            var rutinas = new List<Rutina>();
            for (int i = 1; i <= 10; i++)
            {
                rutinas.Add(new Rutina
                {
                    Id = i,
                    Nombre = $"Rutina {i}",
                    Descripcion = $"Descripción {i}"
                });
            }

            _mockRepo.Setup(r => r.ObtenerTodasAsync()).ReturnsAsync(rutinas);

            // Act
            var resultado = await _casoDeUso.Ejecutar();

            // Assert
            resultado.Should().HaveCount(10);
            resultado.Select(r => r.Id).Should().ContainInOrder(Enumerable.Range(1, 10).Select(i => (long)i));
        }
    }
}
