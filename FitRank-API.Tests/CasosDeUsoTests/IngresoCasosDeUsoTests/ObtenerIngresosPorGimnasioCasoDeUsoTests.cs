using AutoMapper;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.Ingreso;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using Moq;
using Xunit;

namespace FitRank_API.Tests.CasosDeUsoTests.IngresoCasosDeUsoTests
{
    public class ObtenerIngresosPorGimnasioCasoDeUsoTests
    {
        private readonly Mock<IIngresoRepositorio> _mockRepositorio;
        private readonly IMapper _mapper;
        private readonly ObtenerIngresosPorGimnasioCasoDeUso _casoDeUso;

        public ObtenerIngresosPorGimnasioCasoDeUsoTests()
        {
            _mockRepositorio = new Mock<IIngresoRepositorio>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<IngresoMappingProfile>();
            });
            _mapper = config.CreateMapper();
            _casoDeUso = new ObtenerIngresosPorGimnasioCasoDeUso(_mockRepositorio.Object, _mapper);
        }

        [Fact]
        public async Task DeberiaRetornarIngresosPorGimnasio()
        {
            // Arrange
            long gimnasioId = 1;
            var ingresos = new List<Ingreso>
            {
                new Ingreso { Id = 1, GimnasioId = gimnasioId, Monto = 1000, MetodoPago = "Efectivo" },
                new Ingreso { Id = 2, GimnasioId = gimnasioId, Monto = 1500, MetodoPago = "MercadoPago" },
                new Ingreso { Id = 3, GimnasioId = gimnasioId, Monto = 2000, MetodoPago = "Transferencia" }
            };

            _mockRepositorio.Setup(r => r.ObtenerPorGimnasioAsync(gimnasioId))
                .ReturnsAsync(ingresos);

            // Act
            var resultado = await _casoDeUso.Ejecutar(gimnasioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(3);
            resultado.Should().AllSatisfy(i => i.GimnasioId.Should().Be(gimnasioId));
            _mockRepositorio.Verify(r => r.ObtenerPorGimnasioAsync(gimnasioId), Times.Once);
        }

        [Fact]
        public async Task DeberiaRetornarListaVaciaCuandoNoHayIngresosParaGimnasio()
        {
            // Arrange
            long gimnasioId = 5;
            var ingresosVacios = new List<Ingreso>();

            _mockRepositorio.Setup(r => r.ObtenerPorGimnasioAsync(gimnasioId))
                .ReturnsAsync(ingresosVacios);

            // Act
            var resultado = await _casoDeUso.Ejecutar(gimnasioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
            _mockRepositorio.Verify(r => r.ObtenerPorGimnasioAsync(gimnasioId), Times.Once);
        }

        [Fact]
        public async Task DebeLlamarRepositorioConGimnasioIdCorrecto()
        {
            // Arrange
            long gimnasioId = 42;
            var ingresos = new List<Ingreso>
            {
                new Ingreso { Id = 1, GimnasioId = gimnasioId, Monto = 3000 }
            };

            _mockRepositorio.Setup(r => r.ObtenerPorGimnasioAsync(gimnasioId))
                .ReturnsAsync(ingresos);

            // Act
            await _casoDeUso.Ejecutar(gimnasioId);

            // Assert
            _mockRepositorio.Verify(r => r.ObtenerPorGimnasioAsync(gimnasioId), Times.Once);
        }

        [Fact]
        public async Task DeberiaMapearCorrectamenteTodosLosCampos()
        {
            // Arrange
            long gimnasioId = 2;
            var ingresos = new List<Ingreso>
            {
                new Ingreso
                {
                    Id = 10,
                    GimnasioId = gimnasioId,
                    Monto = 5500,
                    MetodoPago = "Tarjeta",
                    Observaciones = "Mensualidad",
                    Confirmado = true,
                    Fecha = DateTime.Now
                }
            };

            _mockRepositorio.Setup(r => r.ObtenerPorGimnasioAsync(gimnasioId))
                .ReturnsAsync(ingresos);

            // Act
            var resultado = await _casoDeUso.Ejecutar(gimnasioId);

            // Assert
            var ingreso = resultado.First();
            ingreso.Id.Should().Be(10);
            ingreso.GimnasioId.Should().Be(gimnasioId);
            ingreso.Monto.Should().Be(5500);
            ingreso.MetodoPago.Should().Be("Tarjeta");
            ingreso.Confirmado.Should().BeTrue();
        }

        [Fact]
        public async Task DeberiaRetornarMultiplesIngresosParaMismoGimnasio()
        {
            // Arrange
            long gimnasioId = 3;
            var ingresos = new List<Ingreso>
            {
                new Ingreso { Id = 1, GimnasioId = gimnasioId, Monto = 1000 },
                new Ingreso { Id = 2, GimnasioId = gimnasioId, Monto = 2000 },
                new Ingreso { Id = 3, GimnasioId = gimnasioId, Monto = 3000 },
                new Ingreso { Id = 4, GimnasioId = gimnasioId, Monto = 4000 },
                new Ingreso { Id = 5, GimnasioId = gimnasioId, Monto = 5000 }
            };

            _mockRepositorio.Setup(r => r.ObtenerPorGimnasioAsync(gimnasioId))
                .ReturnsAsync(ingresos);

            // Act
            var resultado = await _casoDeUso.Ejecutar(gimnasioId);

            // Assert
            resultado.Should().HaveCount(5);
            resultado.Select(i => i.Id).Should().ContainInOrder(1, 2, 3, 4, 5);
        }

        [Fact]
        public async Task DeberiaRetornarDTOsTipoCorrectamente()
        {
            // Arrange
            long gimnasioId = 1;
            var ingresos = new List<Ingreso>
            {
                new Ingreso { Id = 1, GimnasioId = gimnasioId, Monto = 1000 }
            };

            _mockRepositorio.Setup(r => r.ObtenerPorGimnasioAsync(gimnasioId))
                .ReturnsAsync(ingresos);

            // Act
            var resultado = await _casoDeUso.Ejecutar(gimnasioId);

            // Assert
            resultado.Should().AllBeAssignableTo<FitRank_API.Application.DTOs.IngresoDTOs.ObtenerIngresoDTO>();
        }

        [Fact]
        public async Task DeberiaRetornarIngresosOrdenadosComoVienenDelRepositorio()
        {
            // Arrange
            long gimnasioId = 8;
            var ingresos = new List<Ingreso>
            {
                new Ingreso { Id = 5, GimnasioId = gimnasioId, Monto = 500 },
                new Ingreso { Id = 3, GimnasioId = gimnasioId, Monto = 300 },
                new Ingreso { Id = 7, GimnasioId = gimnasioId, Monto = 700 }
            };

            _mockRepositorio.Setup(r => r.ObtenerPorGimnasioAsync(gimnasioId))
                .ReturnsAsync(ingresos);

            // Act
            var resultado = await _casoDeUso.Ejecutar(gimnasioId);

            // Assert
            resultado.Should().HaveCount(3);
            resultado.ElementAt(0).Id.Should().Be(5);
            resultado.ElementAt(1).Id.Should().Be(3);
            resultado.ElementAt(2).Id.Should().Be(7);
        }

        [Fact]
        public async Task DeberiaMapearIngresosConDiferentesEstadosDeConfirmacion()
        {
            // Arrange
            long gimnasioId = 9;
            var ingresos = new List<Ingreso>
            {
                new Ingreso { Id = 1, GimnasioId = gimnasioId, Monto = 1000, Confirmado = true },
                new Ingreso { Id = 2, GimnasioId = gimnasioId, Monto = 2000, Confirmado = false },
                new Ingreso { Id = 3, GimnasioId = gimnasioId, Monto = 3000, Confirmado = true }
            };

            _mockRepositorio.Setup(r => r.ObtenerPorGimnasioAsync(gimnasioId))
                .ReturnsAsync(ingresos);

            // Act
            var resultado = await _casoDeUso.Ejecutar(gimnasioId);

            // Assert
            resultado.ElementAt(0).Confirmado.Should().BeTrue();
            resultado.ElementAt(1).Confirmado.Should().BeFalse();
            resultado.ElementAt(2).Confirmado.Should().BeTrue();
        }
    }
}
