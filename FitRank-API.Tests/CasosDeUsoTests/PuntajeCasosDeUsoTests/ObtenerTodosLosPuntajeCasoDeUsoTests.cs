using AutoMapper;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.PuntajeCasosDeUso;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.PuntajeCasosDeUsoTests
{
    public class ObtenerTodosLosPuntajeCasoDeUsoTests
    {
        private readonly Mock<IPuntajeRepositorio> _mockRepositorio;
        private readonly IMapper _mapper;
        private readonly ObtenerTodosLosPuntajeCasoDeUso _casoDeUso;

        public ObtenerTodosLosPuntajeCasoDeUsoTests()
        {
            _mockRepositorio = new Mock<IPuntajeRepositorio>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PuntajeProfile>();
            });
            _mapper = config.CreateMapper();
            _casoDeUso = new ObtenerTodosLosPuntajeCasoDeUso(_mockRepositorio.Object, _mapper);
        }

        [Fact]
        public async Task DeberiaRetornarTodosLosPuntajes()
        {
            // Arrange
            var puntajes = new List<Puntaje>
            {
                new Puntaje { Id = 1, SocioId = 1, Motivo = "Motivo 1", Fecha = DateTime.Now, Valor = 10 },
                new Puntaje { Id = 2, SocioId = 2, Motivo = "Motivo 2", Fecha = DateTime.Now, Valor = 15 }
            };

            _mockRepositorio.Setup(r => r.ObtenerTodasAsync())
                .ReturnsAsync(puntajes);

            // Act
            var resultado = await _casoDeUso.Ejecutar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.First().Id.Should().Be(1);
            resultado.Last().Id.Should().Be(2);
            _mockRepositorio.Verify(r => r.ObtenerTodasAsync(), Times.Once);
        }

        [Fact]
        public async Task DeberiaRetornarListaVaciaCuandoNoHayPuntajes()
        {
            // Arrange
            var puntajesVacios = new List<Puntaje>();

            _mockRepositorio.Setup(r => r.ObtenerTodasAsync())
                .ReturnsAsync(puntajesVacios);

            // Act
            var resultado = await _casoDeUso.Ejecutar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
            _mockRepositorio.Verify(r => r.ObtenerTodasAsync(), Times.Once);
        }
    }
}
