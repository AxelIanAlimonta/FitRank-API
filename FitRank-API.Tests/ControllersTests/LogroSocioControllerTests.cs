using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using FitRank_API.Application.DTOs.LogroSocioDTOs;
using AutoMapper;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Application.CasosDeUso.LogroSocioCasosDeUso;
using FitRank_API.Presentacion.Controllers;

namespace FitRank_API.tests.ControllersTests;

public class LogroSocioControllerTests
{
    private readonly LogrosSocioController _controller;
    private readonly Mock<ObtenerLogrosSocioCasoDeUso> _mockObtenerLogrosSocio;
    private readonly Mock<ObtenerLogrosDisponiblesPorSocioCasoDeUso> _mockObtenerLogrosDisponibles;

    public LogroSocioControllerTests()
    {
        var mockLogroSocioRepositorio = new Mock<ILogroSocioRepositorio>();
        var mockLogroGimnasioRepositorio = new Mock<ILogroGimnasioRepositorio>();
        var mockMapper = new Mock<IMapper>();

        _mockObtenerLogrosSocio = new Mock<ObtenerLogrosSocioCasoDeUso>(
            mockLogroSocioRepositorio.Object, mockMapper.Object);
        _mockObtenerLogrosDisponibles = new Mock<ObtenerLogrosDisponiblesPorSocioCasoDeUso>(
            mockLogroGimnasioRepositorio.Object, mockLogroSocioRepositorio.Object, mockMapper.Object);

        _controller = new LogrosSocioController(
            _mockObtenerLogrosSocio.Object,
            _mockObtenerLogrosDisponibles.Object
        );
    }

    #region ObtenerLogrosObtenidosPorSocio Tests

    [Fact]
    public async Task ObtenerLogrosObtenidosPorSocio_RetornaOkResult_ConListaCompleta()
    {
        // Arrange
        int socioId = 1;
        int gimnasioId = 10;
        var listaLogros = new List<LogroSocioDTO>
        {
            new LogroSocioDTO
            {
                LogroId = 1,
                Nombre = "Logro 1",
                NombreClave = "logro_1",
                FechaOtorgado = DateTime.Now
            },
            new LogroSocioDTO
            {
                LogroId = 2,
                Nombre = "Logro 2",
                NombreClave = "logro_2",
                FechaOtorgado = DateTime.Now.AddDays(-5)
            }
        };

        _mockObtenerLogrosSocio.Setup(caso => caso.Ejecutar(socioId, gimnasioId))
            .ReturnsAsync(listaLogros);

        // Act
        var resultado = await _controller.ObtenerLogrosObtenidosPorSocio(socioId, gimnasioId);

        // Assert
        var okResult = resultado.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaLogros);
    }

    [Fact]
    public async Task ObtenerLogrosObtenidosPorSocio_RetornaOkResult_ConListaVacia()
    {
        // Arrange
        int socioId = 1;
        int gimnasioId = 10;
        _mockObtenerLogrosSocio.Setup(caso => caso.Ejecutar(socioId, gimnasioId))
            .ReturnsAsync(new List<LogroSocioDTO>());

        // Act
        var resultado = await _controller.ObtenerLogrosObtenidosPorSocio(socioId, gimnasioId);

        // Assert
        var okResult = resultado.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerLogrosObtenidosPorSocio_SocioIdCero_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.ObtenerLogrosObtenidosPorSocio(0, 10);

        // Assert
        var badRequestResult = resultado.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerLogrosObtenidosPorSocio_SocioIdNegativo_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.ObtenerLogrosObtenidosPorSocio(-5, 10);

        // Assert
        var badRequestResult = resultado.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerLogrosObtenidosPorSocio_GimnasioIdCero_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.ObtenerLogrosObtenidosPorSocio(1, 0);

        // Assert
        var badRequestResult = resultado.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerLogrosObtenidosPorSocio_GimnasioIdNegativo_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.ObtenerLogrosObtenidosPorSocio(1, -3);

        // Assert
        var badRequestResult = resultado.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerLogrosObtenidosPorSocio_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockObtenerLogrosSocio.Setup(caso => caso.Ejecutar(1, 1))
            .ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.ObtenerLogrosObtenidosPorSocio(1, 1);

        // Assert
        var statusCodeResult = resultado.Result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerLogrosDisponiblesPorSocio Tests

    [Fact]
    public async Task ObtenerLogrosDisponiblesPorSocio_RetornaOkResult_ConListaCompleta()
    {
        // Arrange
        int socioId = 1;
        int gimnasioId = 10;
        var listaLogros = new List<LogroDisponibleDTO>
        {
            new LogroDisponibleDTO
            {
                LogroId = 3,
                Nombre = "Logro 3",
                NombreClave = "logro_3"
            },
            new LogroDisponibleDTO
            {
                LogroId = 4,
                Nombre = "Logro 4",
                NombreClave = "logro_4"
            }
        };

        _mockObtenerLogrosDisponibles.Setup(caso => caso.Ejecutar(socioId, gimnasioId))
            .ReturnsAsync(listaLogros);

        // Act
        var resultado = await _controller.ObtenerLogrosDisponiblesPorSocio(socioId, gimnasioId);

        // Assert
        var okResult = resultado.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaLogros);
    }

    [Fact]
    public async Task ObtenerLogrosDisponiblesPorSocio_RetornaOkResult_ConListaVacia()
    {
        // Arrange
        int socioId = 1;
        int gimnasioId = 10;
        _mockObtenerLogrosDisponibles.Setup(caso => caso.Ejecutar(socioId, gimnasioId))
            .ReturnsAsync(new List<LogroDisponibleDTO>());

        // Act
        var resultado = await _controller.ObtenerLogrosDisponiblesPorSocio(socioId, gimnasioId);

        // Assert
        var okResult = resultado.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerLogrosDisponiblesPorSocio_SocioIdCero_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.ObtenerLogrosDisponiblesPorSocio(0, 10);

        // Assert
        var badRequestResult = resultado.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerLogrosDisponiblesPorSocio_SocioIdNegativo_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.ObtenerLogrosDisponiblesPorSocio(-7, 10);

        // Assert
        var badRequestResult = resultado.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerLogrosDisponiblesPorSocio_GimnasioIdCero_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.ObtenerLogrosDisponiblesPorSocio(1, 0);

        // Assert
        var badRequestResult = resultado.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerLogrosDisponiblesPorSocio_GimnasioIdNegativo_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.ObtenerLogrosDisponiblesPorSocio(1, -2);

        // Assert
        var badRequestResult = resultado.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerLogrosDisponiblesPorSocio_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockObtenerLogrosDisponibles.Setup(caso => caso.Ejecutar(1, 1))
            .ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.ObtenerLogrosDisponiblesPorSocio(1, 1);

        // Assert
        var statusCodeResult = resultado.Result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion
}
