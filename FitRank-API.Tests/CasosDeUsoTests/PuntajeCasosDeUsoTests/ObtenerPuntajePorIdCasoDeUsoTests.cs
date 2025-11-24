using AutoMapper;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.PuntajeCasosDeUso;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
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
    }
}
