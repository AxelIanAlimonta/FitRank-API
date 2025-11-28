using AutoMapper;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.Ingreso;
using FitRank_API.Application.DTOs.IngresoDTOs;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using Moq;
using Xunit;

namespace FitRank_API.Tests.CasosDeUsoTests.IngresoCasosDeUsoTests
{
    public class AgregarIngresoCasoDeUsoTests
    {
        private readonly Mock<IIngresoRepositorio> _mockRepositorio;
        private readonly IMapper _mapper;
        private readonly AgregarIngresoCasoDeUso _casoDeUso;

        public AgregarIngresoCasoDeUsoTests()
        {
            _mockRepositorio = new Mock<IIngresoRepositorio>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<IngresoMappingProfile>();
            });
            _mapper = config.CreateMapper();
            _casoDeUso = new AgregarIngresoCasoDeUso(_mockRepositorio.Object, _mapper);
        }

        [Fact]
        public async Task DeberiaAgregarIngresoCorrectamente()
        {
            // Arrange
            var dto = new AgregarIngresoDTO
            {
                GimnasioId = 1,
                Monto = 1500,
                MetodoPago = "MercadoPago",
                Observaciones = "Mensualidad"
            };

            _mockRepositorio.Setup(r => r.AgregarAsync(It.IsAny<Ingreso>()))
                .Returns(Task.CompletedTask);
            _mockRepositorio.Setup(r => r.GuardarCambiosAsync())
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.GimnasioId.Should().Be(1);
            resultado.Monto.Should().Be(1500);
            resultado.MetodoPago.Should().Be("MercadoPago");
            resultado.Confirmado.Should().BeTrue();
            _mockRepositorio.Verify(r => r.AgregarAsync(It.IsAny<Ingreso>()), Times.Once);
            _mockRepositorio.Verify(r => r.GuardarCambiosAsync(), Times.Once);
        }

        [Fact]
        public async Task DeberiaEstablecerConfirmadoEnTrue()
        {
            // Arrange
            var dto = new AgregarIngresoDTO
            {
                GimnasioId = 2,
                Monto = 2000,
                MetodoPago = "Efectivo"
            };

            _mockRepositorio.Setup(r => r.AgregarAsync(It.IsAny<Ingreso>()))
                .Returns(Task.CompletedTask);
            _mockRepositorio.Setup(r => r.GuardarCambiosAsync())
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Confirmado.Should().BeTrue();
        }

        [Fact]
        public async Task DebeLlamarRepositorioConIngresoCorrecto()
        {
            // Arrange
            var dto = new AgregarIngresoDTO
            {
                GimnasioId = 3,
                Monto = 3000,
                MetodoPago = "Transferencia",
                Observaciones = "Anual"
            };

            Ingreso? ingresoCapturado = null;
            _mockRepositorio.Setup(r => r.AgregarAsync(It.IsAny<Ingreso>()))
                .Callback<Ingreso>(i => ingresoCapturado = i)
                .Returns(Task.CompletedTask);
            _mockRepositorio.Setup(r => r.GuardarCambiosAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            ingresoCapturado.Should().NotBeNull();
            ingresoCapturado!.GimnasioId.Should().Be(3);
            ingresoCapturado.Monto.Should().Be(3000);
            ingresoCapturado.MetodoPago.Should().Be("Transferencia");
            ingresoCapturado.Confirmado.Should().BeTrue();
        }

        [Fact]
        public async Task DebeGuardarCambiosDespuesDeAgregar()
        {
            // Arrange
            var dto = new AgregarIngresoDTO
            {
                GimnasioId = 1,
                Monto = 1000,
                MetodoPago = "Efectivo"
            };

            var llamadas = new List<string>();
            _mockRepositorio.Setup(r => r.AgregarAsync(It.IsAny<Ingreso>()))
                .Callback(() => llamadas.Add("Agregar"))
                .Returns(Task.CompletedTask);
            _mockRepositorio.Setup(r => r.GuardarCambiosAsync())
                .Callback(() => llamadas.Add("GuardarCambios"))
                .Returns(Task.CompletedTask);

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            llamadas.Should().HaveCount(2);
            llamadas[0].Should().Be("Agregar");
            llamadas[1].Should().Be("GuardarCambios");
        }

        [Fact]
        public async Task DeberiaMapearTodosLosCamposCorrectamente()
        {
            // Arrange
            var dto = new AgregarIngresoDTO
            {
                GimnasioId = 5,
                Monto = 5500,
                MetodoPago = "Tarjeta",
                Observaciones = "Semestral",
                UsuarioId = 10
            };

            _mockRepositorio.Setup(r => r.AgregarAsync(It.IsAny<Ingreso>()))
                .Returns(Task.CompletedTask);
            _mockRepositorio.Setup(r => r.GuardarCambiosAsync())
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.GimnasioId.Should().Be(5);
            resultado.Monto.Should().Be(5500);
            resultado.MetodoPago.Should().Be("Tarjeta");
        }
    }
}
