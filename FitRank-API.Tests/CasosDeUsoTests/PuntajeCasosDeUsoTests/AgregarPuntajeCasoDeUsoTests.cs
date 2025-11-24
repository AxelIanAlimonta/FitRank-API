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
    public class AgregarPuntajeCasoDeUsoTests
    {
        private readonly Mock<IPuntajeRepositorio> _mockRepositorio;
        private readonly IMapper _mapper;
        private readonly AgregarPuntajeCasoDeUso _casoDeUso;

        public AgregarPuntajeCasoDeUsoTests()
        {
            _mockRepositorio = new Mock<IPuntajeRepositorio>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PuntajeProfile>();
            });
            _mapper = config.CreateMapper();
            _casoDeUso = new AgregarPuntajeCasoDeUso(_mockRepositorio.Object, _mapper);
        }

        [Fact]
        public async Task DeberiaAgregarPuntajeCorrectamente()
        {
            // Arrange
            var nuevoPuntaje = new AgregarPuntajeDTO
            {
                SocioId = 1,
                Motivo = "Asistencia perfecta",
                Fecha = DateTime.Now,
                Valor = 10
            };

            var puntajeEntidad = new Puntaje
            {
                Id = 1,
                SocioId = 1,
                Motivo = "Asistencia perfecta",
                Fecha = nuevoPuntaje.Fecha,
                Valor = 10
            };

            _mockRepositorio.Setup(r => r.AgregarAsync(It.IsAny<Puntaje>()))
                .ReturnsAsync(puntajeEntidad);

            // Act
            var resultado = await _casoDeUso.Ejecutar(nuevoPuntaje);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(1);
            resultado.SocioId.Should().Be(1);
            resultado.Motivo.Should().Be("Asistencia perfecta");
            resultado.Valor.Should().Be(10);
            _mockRepositorio.Verify(r => r.AgregarAsync(It.IsAny<Puntaje>()), Times.Once);
        }
    }
}
