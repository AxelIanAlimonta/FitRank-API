using AutoMapper;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.SocioCasoDeUso;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.SocioCasosDeUsoTests
{
    public class ObtenerSociosCasoDeUsoTests
    {
        private readonly Mock<ISocioRepositorio> _mockRepositorio;
        private readonly IMapper _mapper;
        private readonly ObtenerSociosCasoDeUso _casoDeUso;

        public ObtenerSociosCasoDeUsoTests()
        {
            _mockRepositorio = new Mock<ISocioRepositorio>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SocioProfile>();
            });
            _mapper = config.CreateMapper();
            _casoDeUso = new ObtenerSociosCasoDeUso(_mockRepositorio.Object, _mapper);
        }

        [Fact]
        public async Task DeberiaRetornarTodosLosSocios()
        {
            // Arrange
            var socios = new List<Socio>
            {
                new Socio
                {
                    Id = 1,
                    Nombre = "Juan",
                    Apellido = "Pérez",
                    Email = "juan@test.com",
                    NombreUsuario = "juanp",
                    GimnasioId = 1,
                    Gimnasio = new Gimnasio { Id = 1, Nombre = "Gimnasio Test" }
                },
                new Socio
                {
                    Id = 2,
                    Nombre = "María",
                    Apellido = "García",
                    Email = "maria@test.com",
                    NombreUsuario = "mariag",
                    GimnasioId = 1,
                    Gimnasio = new Gimnasio { Id = 1, Nombre = "Gimnasio Test" }
                }
            };

            _mockRepositorio.Setup(r => r.ObtenerTodosAsync())
                .ReturnsAsync(socios);

            // Act
            var resultado = await _casoDeUso.Ejecutar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.First().Id.Should().Be(1);
            resultado.Last().Id.Should().Be(2);
            _mockRepositorio.Verify(r => r.ObtenerTodosAsync(), Times.Once);
        }

        [Fact]
        public async Task DeberiaRetornarListaVaciaCuandoNoHaySocios()
        {
            // Arrange
            var sociosVacios = new List<Socio>();

            _mockRepositorio.Setup(r => r.ObtenerTodosAsync())
                .ReturnsAsync(sociosVacios);

            // Act
            var resultado = await _casoDeUso.Ejecutar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
            _mockRepositorio.Verify(r => r.ObtenerTodosAsync(), Times.Once);
        }
    }
}
