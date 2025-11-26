using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using FitRank_API.Controllers;
using FitRank_API.Application.DTOs.LogroSocioDTOs;
using AutoMapper;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Application.CasosDeUso.LogroSocioCasosDeUso;

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

        _mockObtenerLogrosSocio = new Mock<ObtenerLogrosSocioCasoDeUso>(mockLogroSocioRepositorio.Object, mockMapper.Object);
        _mockObtenerLogrosDisponibles = new Mock<ObtenerLogrosDisponiblesPorSocioCasoDeUso>(mockLogroGimnasioRepositorio.Object, mockLogroSocioRepositorio.Object, mockMapper.Object);

        _controller = new LogrosSocioController(
            _mockObtenerLogrosSocio.Object,
            _mockObtenerLogrosDisponibles.Object
        );
    }

    //ObtenerLogrosObtenidosPorSocio_RetornaOkResult_ConListaCompleta
    [Fact]
    public async Task ObtenerLogrosObtenidosPorSocio_RetornaOkResult_ConListaCompleta()
    {
        // Arrange
        int socioId = 1;
        int gimnasioId = 10;
        var listaLogros = new List<LogroSocioDTO>
        {
            new LogroSocioDTO { LogroId = 1, Nombre = "Logro 1", NombreClave = "logro_1", Descripcion = "Desc 1", Imagen = "img1.png", FechaOtorgado = DateTime.Now },
            new LogroSocioDTO { LogroId = 2, Nombre = "Logro 2", NombreClave = "logro_2", Descripcion = "Desc 2", Imagen = "img2.png", FechaOtorgado = DateTime.Now.AddDays(-5) }
        };

        _mockObtenerLogrosSocio
            .Setup(caso => caso.Ejecutar(socioId, gimnasioId))
            .ReturnsAsync(listaLogros);

        // Act
        var resultado = await _controller.ObtenerLogrosObtenidosPorSocio(socioId, gimnasioId);

        // Assert
        var okResult = resultado.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaLogros);
    }

    //ObtenerLogrosObtenidosPorSocio_RetornaOkResult_ConListaVacia
    [Fact]
    public async Task ObtenerLogrosObtenidosPorSocio_RetornaOkResult_ConListaVacia()
    {
        // Arrange
        int socioId = 1;
        int gimnasioId = 10;
        var listaLogros = new List<LogroSocioDTO>();

        _mockObtenerLogrosSocio
            .Setup(caso => caso.Ejecutar(socioId, gimnasioId))
            .ReturnsAsync(listaLogros);

        // Act
        var resultado = await _controller.ObtenerLogrosObtenidosPorSocio(socioId, gimnasioId);

        // Assert
        var okResult = resultado.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaLogros);
    }

    //ObtenerLogrosDisponiblesPorSocio_RetornaOkResult_ConListaCompleta
    [Fact]
    public async Task ObtenerLogrosDisponiblesPorSocio_RetornaOkResult_ConListaCompleta()
    {
        // Arrange
        int socioId = 1;
        int gimnasioId = 10;
        var listaLogros = new List<LogroDisponibleDTO>
        {
            new LogroDisponibleDTO { LogroId = 3, Nombre = "Logro 3", NombreClave = "logro_3", Descripcion = "Desc 3", Imagen = "img3.png" },
            new LogroDisponibleDTO { LogroId = 4, Nombre = "Logro 4", NombreClave = "logro_4", Descripcion = "Desc 4", Imagen = "img4.png" }
        };

        _mockObtenerLogrosDisponibles
            .Setup(caso => caso.Ejecutar(socioId, gimnasioId))
            .ReturnsAsync(listaLogros);

        // Act
        var resultado = await _controller.ObtenerLogrosDisponiblesPorSocio(socioId, gimnasioId);

        // Assert
        var okResult = resultado.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaLogros);
    }

    //ObtenerLogrosDisponiblesPorSocio_RetornaOkResult_ConListaVacia
    [Fact]
    public async Task ObtenerLogrosDisponiblesPorSocio_RetornaOkResult_ConListaVacia()
    {
        // Arrange
        int socioId = 1;
        int gimnasioId = 10;
        var listaLogros = new List<LogroDisponibleDTO>();

        _mockObtenerLogrosDisponibles
            .Setup(caso => caso.Ejecutar(socioId, gimnasioId))
            .ReturnsAsync(listaLogros);

        // Act
        var resultado = await _controller.ObtenerLogrosDisponiblesPorSocio(socioId, gimnasioId);

        // Assert
        var okResult = resultado.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaLogros);
    }
}
