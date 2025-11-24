using AutoMapper;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.PuntajeCasosDeUso;
using FitRank_API.Application.DTOs.PuntajeDTOs;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.PuntajeCasosDeUsoTests
{
    public class ActualizarPuntajeCasoDeUsoTests
    {
        private readonly Mock<IPuntajeRepositorio> _mockRepositorio;
        private readonly IMapper _mapper;
        private readonly ActualizarPuntajeCasoDeUso _casoDeUso;

        public ActualizarPuntajeCasoDeUsoTests()
        {
            _mockRepositorio = new Mock<IPuntajeRepositorio>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PuntajeProfile>();
            });
            _mapper = config.CreateMapper();
            _casoDeUso = new ActualizarPuntajeCasoDeUso(_mockRepositorio.Object, _mapper);
        }

        [Fact]
        public async Task DeberiaActualizarPuntajeCorrectamente()
        {
            // Arrange
            var puntajeActualizado = new ActualizarPuntajeDTO
            {
                Id = 1,
                SocioId = 1,
                Motivo = "Motivo actualizado",
                Fecha = DateTime.Now,
                Valor = 15
            };

            var puntajeEntidad = new Puntaje
            {
                Id = 1,
                SocioId = 1,
                Motivo = "Motivo actualizado",
                Fecha = puntajeActualizado.Fecha,
                Valor = 15
            };

            _mockRepositorio.Setup(r => r.ActualizarAsync(It.IsAny<Puntaje>()))
                .ReturnsAsync(puntajeEntidad);

            // Act
            var resultado = await _casoDeUso.Ejecutar(puntajeActualizado);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Id.Should().Be(1);
            resultado.Motivo.Should().Be("Motivo actualizado");
            resultado.Valor.Should().Be(15);
            _mockRepositorio.Verify(r => r.ActualizarAsync(It.IsAny<Puntaje>()), Times.Once);
        }

        [Fact]
        public async Task DeberiaRetornarNullCuandoPuntajeNoExiste()
        {
            // Arrange
            var puntajeActualizado = new ActualizarPuntajeDTO
            {
                Id = 999,
                SocioId = 1,
                Motivo = "Motivo",
                Fecha = DateTime.Now,
                Valor = 10
            };

            _mockRepositorio.Setup(r => r.ActualizarAsync(It.IsAny<Puntaje>()))
                .ReturnsAsync((Puntaje?)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(puntajeActualizado);

            // Assert
            resultado.Should().BeNull();
            _mockRepositorio.Verify(r => r.ActualizarAsync(It.IsAny<Puntaje>()), Times.Once);
        }
    }
}
