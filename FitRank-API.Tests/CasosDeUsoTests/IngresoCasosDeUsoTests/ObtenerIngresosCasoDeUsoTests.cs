using AutoMapper;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.Ingreso;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.IngresoCasosDeUsoTests
{
    public class ObtenerIngresosCasoDeUsoTests
    {
        private readonly Mock<IIngresoRepositorio> _mockRepositorio;
        private readonly IMapper _mapper;
        private readonly ObtenerIngresosCasoDeUso _casoDeUso;

        public ObtenerIngresosCasoDeUsoTests()
        {
            _mockRepositorio = new Mock<IIngresoRepositorio>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<IngresoMappingProfile>();
            });
            _mapper = config.CreateMapper();
            _casoDeUso = new ObtenerIngresosCasoDeUso(_mockRepositorio.Object, _mapper);
        }

        [Fact]
        public async Task DeberiaRetornarTodosLosIngresos()
        {
            // Arrange
            var ingresos = new List<Ingreso>
            {
                new Ingreso { Id = 1, GimnasioId = 1, Monto = 1000, MetodoPago = "Efectivo" },
                new Ingreso { Id = 2, GimnasioId = 1, Monto = 1500, MetodoPago = "MercadoPago" }
            };

            _mockRepositorio.Setup(r => r.ObtenerTodosAsync())
                .ReturnsAsync(ingresos);

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
        public async Task DeberiaRetornarListaVaciaCuandoNoHayIngresos()
        {
            // Arrange
            var ingresosVacios = new List<Ingreso>();

            _mockRepositorio.Setup(r => r.ObtenerTodosAsync())
                .ReturnsAsync(ingresosVacios);

            // Act
            var resultado = await _casoDeUso.Ejecutar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
            _mockRepositorio.Verify(r => r.ObtenerTodosAsync(), Times.Once);
        }
    }
}
