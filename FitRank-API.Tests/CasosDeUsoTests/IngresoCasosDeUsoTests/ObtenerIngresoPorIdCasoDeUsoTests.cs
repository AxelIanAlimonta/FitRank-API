using AutoMapper;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.Ingreso;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.IngresoCasosDeUsoTests
{
    public class ObtenerIngresoPorIdCasoDeUsoTests
    {
        private readonly Mock<IIngresoRepositorio> _mockRepositorio;
        private readonly IMapper _mapper;
        private readonly ObtenerIngresoPorIdCasoDeUso _casoDeUso;

        public ObtenerIngresoPorIdCasoDeUsoTests()
        {
            _mockRepositorio = new Mock<IIngresoRepositorio>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<IngresoMappingProfile>();
            });
            _mapper = config.CreateMapper();
            _casoDeUso = new ObtenerIngresoPorIdCasoDeUso(_mockRepositorio.Object, _mapper);
        }

        [Fact]
        public async Task DeberiaRetornarIngresoCuandoExiste()
        {
            // Arrange
            long ingresoId = 1;
            var ingresoExistente = new Ingreso
            {
                Id = 1,
                GimnasioId = 1,
                Monto = 1000,
                MetodoPago = "Efectivo",
                Fecha = DateTime.Now
            };

            _mockRepositorio.Setup(r => r.ObtenerPorIdAsync(ingresoId))
                .ReturnsAsync(ingresoExistente);

            // Act
            var resultado = await _casoDeUso.Ejecutar(ingresoId);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Id.Should().Be(1);
            resultado.Monto.Should().Be(1000);
            resultado.MetodoPago.Should().Be("Efectivo");
            _mockRepositorio.Verify(r => r.ObtenerPorIdAsync(ingresoId), Times.Once);
        }

        [Fact]
        public async Task DeberiaRetornarNullCuandoIngresoNoExiste()
        {
            // Arrange
            long ingresoId = 999;
            _mockRepositorio.Setup(r => r.ObtenerPorIdAsync(ingresoId))
                .ReturnsAsync((Ingreso?)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(ingresoId);

            // Assert
            resultado.Should().BeNull();
            _mockRepositorio.Verify(r => r.ObtenerPorIdAsync(ingresoId), Times.Once);
        }
    }
}
