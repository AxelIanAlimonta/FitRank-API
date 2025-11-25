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

        [Fact]
        public async Task DebeMapearCorrectamenteTodosLosCamposDeCadaPuntaje()
        {
            // Arrange
            var fecha1 = new DateTime(2024, 1, 10);
            var fecha2 = new DateTime(2024, 2, 15);
            var puntajes = new List<Puntaje>
            {
                new Puntaje { Id = 10, SocioId = 100, Motivo = "Motivo A", Fecha = fecha1, Valor = 20 },
                new Puntaje { Id = 20, SocioId = 200, Motivo = "Motivo B", Fecha = fecha2, Valor = 30 }
            };

            _mockRepositorio.Setup(r => r.ObtenerTodasAsync())
                .ReturnsAsync(puntajes);

            // Act
            var resultado = await _casoDeUso.Ejecutar();

            // Assert
            var lista = resultado.ToList();
            lista.Should().HaveCount(2);
            lista[0].Id.Should().Be(10);
            lista[0].SocioId.Should().Be(100);
            lista[0].Motivo.Should().Be("Motivo A");
            lista[0].Fecha.Should().Be(fecha1);
            lista[0].Valor.Should().Be(20);
            lista[1].Id.Should().Be(20);
            lista[1].SocioId.Should().Be(200);
            lista[1].Motivo.Should().Be("Motivo B");
            lista[1].Fecha.Should().Be(fecha2);
            lista[1].Valor.Should().Be(30);
        }

        [Fact]
        public async Task DebeLlamarRepositorioUnaVez()
        {
            // Arrange
            var puntajes = new List<Puntaje>();

            _mockRepositorio.Setup(r => r.ObtenerTodasAsync())
                .ReturnsAsync(puntajes);

            // Act
            await _casoDeUso.Ejecutar();

            // Assert
            _mockRepositorio.Verify(r => r.ObtenerTodasAsync(), Times.Once);
        }

        [Fact]
        public async Task DeberiaRetornarPuntajesEnElMismoOrdenDelRepositorio()
        {
            // Arrange
            var puntajes = new List<Puntaje>
            {
                new Puntaje { Id = 3, SocioId = 1, Motivo = "Tercero", Fecha = DateTime.Now, Valor = 30 },
                new Puntaje { Id = 1, SocioId = 2, Motivo = "Primero", Fecha = DateTime.Now, Valor = 10 },
                new Puntaje { Id = 2, SocioId = 3, Motivo = "Segundo", Fecha = DateTime.Now, Valor = 20 }
            };

            _mockRepositorio.Setup(r => r.ObtenerTodasAsync())
                .ReturnsAsync(puntajes);

            // Act
            var resultado = await _casoDeUso.Ejecutar();

            // Assert
            var lista = resultado.ToList();
            lista[0].Id.Should().Be(3);
            lista[1].Id.Should().Be(1);
            lista[2].Id.Should().Be(2);
        }

        [Fact]
        public async Task DeberiaRetornarListaConMultiplesPuntajes()
        {
            // Arrange
            var puntajes = new List<Puntaje>();
            for (int i = 1; i <= 5; i++)
            {
                puntajes.Add(new Puntaje
                {
                    Id = i,
                    SocioId = i * 10,
                    Motivo = $"Motivo {i}",
                    Fecha = DateTime.Now.AddDays(-i),
                    Valor = i * 5
                });
            }

            _mockRepositorio.Setup(r => r.ObtenerTodasAsync())
                .ReturnsAsync(puntajes);

            // Act
            var resultado = await _casoDeUso.Ejecutar();

            // Assert
            resultado.Should().HaveCount(5);
            resultado.ElementAt(0).Valor.Should().Be(5);
            resultado.ElementAt(4).Valor.Should().Be(25);
        }
    }
}
