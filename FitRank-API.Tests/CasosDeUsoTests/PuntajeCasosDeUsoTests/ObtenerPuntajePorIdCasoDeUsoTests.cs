using AutoMapper;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.PuntajeCasosDeUso;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.PuntajeCasosDeUsoTests
{
    public class ObtenerPuntajePorIdCasoDeUsoTests
    {
        private readonly Mock<IPuntajeRepositorio> _mockRepositorio;
        private readonly IMapper _mapper;
        private readonly ObtenerPuntajePorIdCasoDeUso _casoDeUso;

        public ObtenerPuntajePorIdCasoDeUsoTests()
        {
            _mockRepositorio = new Mock<IPuntajeRepositorio>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PuntajeProfile>();
            });
            _mapper = config.CreateMapper();
            _casoDeUso = new ObtenerPuntajePorIdCasoDeUso(_mockRepositorio.Object, _mapper);
        }

        [Fact]
        public async Task DeberiaRetornarPuntajeCuandoExiste()
        {
            // Arrange
            long puntajeId = 1;
            var puntajeExistente = new Puntaje
            {
                Id = 1,
                SocioId = 1,
                Motivo = "Asistencia perfecta",
                Fecha = DateTime.Now,
                Valor = 10
            };

            _mockRepositorio.Setup(r => r.ObtenerPorIdAsync(puntajeId))
                .ReturnsAsync(puntajeExistente);

            // Act
            var resultado = await _casoDeUso.Ejecutar(puntajeId);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Id.Should().Be(1);
            resultado.SocioId.Should().Be(1);
            resultado.Motivo.Should().Be("Asistencia perfecta");
            resultado.Valor.Should().Be(10);
            _mockRepositorio.Verify(r => r.ObtenerPorIdAsync(puntajeId), Times.Once);
        }

        [Fact]
        public async Task DeberiaRetornarNullCuandoPuntajeNoExiste()
        {
            // Arrange
            long puntajeId = 999;
            _mockRepositorio.Setup(r => r.ObtenerPorIdAsync(puntajeId))
                .ReturnsAsync((Puntaje?)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(puntajeId);

            // Assert
            resultado.Should().BeNull();
            _mockRepositorio.Verify(r => r.ObtenerPorIdAsync(puntajeId), Times.Once);
        }

        [Fact]
        public async Task DebeMapearCorrectamenteTodosLosCampos()
        {
            // Arrange
            long puntajeId = 50;
            var fecha = new DateTime(2024, 7, 20, 16, 30, 0);
            var puntaje = new Puntaje
            {
                Id = 50,
                SocioId = 250,
                Motivo = "Logro importante",
                Fecha = fecha,
                Valor = 35
            };

            _mockRepositorio.Setup(r => r.ObtenerPorIdAsync(puntajeId))
                .ReturnsAsync(puntaje);

            // Act
            var resultado = await _casoDeUso.Ejecutar(puntajeId);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Id.Should().Be(50);
            resultado.SocioId.Should().Be(250);
            resultado.Motivo.Should().Be("Logro importante");
            resultado.Fecha.Should().Be(fecha);
            resultado.Valor.Should().Be(35);
        }

        [Fact]
        public async Task DebeLlamarRepositorioConIdCorrecto()
        {
            // Arrange
            long puntajeId = 777;

            _mockRepositorio.Setup(r => r.ObtenerPorIdAsync(puntajeId))
                .ReturnsAsync((Puntaje?)null);

            // Act
            await _casoDeUso.Ejecutar(puntajeId);

            // Assert
            _mockRepositorio.Verify(r => r.ObtenerPorIdAsync(puntajeId), Times.Once);
        }

        [Fact]
        public async Task DeberiaRetornarPuntajesConDiferentesValores()
        {
            // Arrange
            var puntaje1 = new Puntaje { Id = 1, SocioId = 1, Motivo = "Bajo", Fecha = DateTime.Now, Valor = 5 };
            var puntaje2 = new Puntaje { Id = 2, SocioId = 2, Motivo = "Alto", Fecha = DateTime.Now, Valor = 100 };

            _mockRepositorio.Setup(r => r.ObtenerPorIdAsync(1))
                .ReturnsAsync(puntaje1);

            _mockRepositorio.Setup(r => r.ObtenerPorIdAsync(2))
                .ReturnsAsync(puntaje2);

            // Act
            var resultado1 = await _casoDeUso.Ejecutar(1);
            var resultado2 = await _casoDeUso.Ejecutar(2);

            // Assert
            resultado1!.Valor.Should().Be(5);
            resultado2!.Valor.Should().Be(100);
        }

        [Fact]
        public async Task DeberiaRetornarTipoObtenerPuntajeDTO()
        {
            // Arrange
            long puntajeId = 1;
            var puntaje = new Puntaje
            {
                Id = 1,
                SocioId = 1,
                Motivo = "Test",
                Fecha = DateTime.Now,
                Valor = 10
            };

            _mockRepositorio.Setup(r => r.ObtenerPorIdAsync(puntajeId))
                .ReturnsAsync(puntaje);

            // Act
            var resultado = await _casoDeUso.Ejecutar(puntajeId);

            // Assert
            resultado.Should().BeOfType<FitRank_API.Application.DTOs.PuntajeDTOs.ObtenerPuntajeDTO>();
        }
    }
}
