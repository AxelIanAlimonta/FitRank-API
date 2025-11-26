using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using FitRank_API.Controllers;
using FitRank_API.Application.DTOs.LogroGimnasioDTOs;
using AutoMapper;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Application.CasosDeUso.LogroGimnasioCasosDeUso;

namespace FitRank_API.tests.ControllersTests;

public class LogroGimnasioControllerTests
{
    private readonly LogrosGimnasioController _controller;
    private readonly Mock<ObtenerLogrosGimnasioCasoDeUso> _mockObtenerLogrosGimnasio;
    private readonly Mock<ActualizarLogroGimnasioCasoDeUso> _mockActualizarLogroGimnasio;

    public LogroGimnasioControllerTests()
    {
        var mockLogroGimnasioRepositorio = new Mock<ILogroGimnasioRepositorio>();
        var mockLogroRepositorio = new Mock<ILogroRepositorio>();
        var mockMapper = new Mock<IMapper>();

        _mockObtenerLogrosGimnasio = new Mock<ObtenerLogrosGimnasioCasoDeUso>(mockLogroGimnasioRepositorio.Object, mockMapper.Object);
        _mockActualizarLogroGimnasio = new Mock<ActualizarLogroGimnasioCasoDeUso>(mockLogroGimnasioRepositorio.Object, mockLogroRepositorio.Object, mockMapper.Object);

        _controller = new LogrosGimnasioController(
            _mockObtenerLogrosGimnasio.Object,
            _mockActualizarLogroGimnasio.Object
        );
    }

    //ObtenerLogrosGimnasio_RetornaOkResult_ConListaCompleta
    [Fact]
    public async Task ObtenerLogrosGimnasio_RetornaOkResult_ConListaCompleta()
    {
        // Arrange
        int gimnasioId = 10;
        var listaLogros = new List<LogroGimnasioDTO>
        {
            new LogroGimnasioDTO { LogroId = 1, GimnasioId = gimnasioId, EstaHabilitado = true, Nombre = "Logro 1", NombreClave = "logro_1", Descripcion = "Desc 1", Imagen = "img1.png" },
            new LogroGimnasioDTO { LogroId = 2, GimnasioId = gimnasioId, EstaHabilitado = false, Nombre = "Logro 2", NombreClave = "logro_2", Descripcion = "Desc 2", Imagen = "img2.png" }
        };

        _mockObtenerLogrosGimnasio
            .Setup(caso => caso.Ejecutar(gimnasioId))
            .ReturnsAsync(listaLogros);

        // Act
        var resultado = await _controller.ObtenerLogrosGimnasio(gimnasioId);

        // Assert
        var okResult = resultado.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaLogros);
    }

    //ObtenerLogrosGimnasio_RetornaOkResult_ConListaVacia
    [Fact]
    public async Task ObtenerLogrosGimnasio_RetornaOkResult_ConListaVacia()
    {
        // Arrange
        int gimnasioId = 10;
        var listaLogros = new List<LogroGimnasioDTO>();

        _mockObtenerLogrosGimnasio
            .Setup(caso => caso.Ejecutar(gimnasioId))
            .ReturnsAsync(listaLogros);

        // Act
        var resultado = await _controller.ObtenerLogrosGimnasio(gimnasioId);

        // Assert
        var okResult = resultado.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaLogros);
    }

    //ActualizarLogrosGimnasio_RetornaOkResult_ConObjetoActualizado
    [Fact]
    public async Task ActualizarLogrosGimnasio_RetornaOkResult_ConObjetoActualizado()
    {
        // Arrange
        int gimnasioId = 10;
        int logroId = 5;
        var actualizarDTO = new ActualizarLogroGimnasioDTO { GimnasioId = gimnasioId, LogroId = logroId, EstaActivo = true };
        var logroActualizado = new LogroGimnasioDTO { GimnasioId = gimnasioId, LogroId = logroId, EstaHabilitado = true, Nombre = "Logro 5", NombreClave = "logro_5", Descripcion = "Desc 5", Imagen = "img5.png" };

        _mockActualizarLogroGimnasio
            .Setup(caso => caso.Ejecutar(It.Is<ActualizarLogroGimnasioDTO>(dto => 
                dto.GimnasioId == gimnasioId && dto.LogroId == logroId)))
            .ReturnsAsync(logroActualizado);

        // Act
        var resultado = await _controller.ActualizarLogrosGimnasio(gimnasioId, logroId, actualizarDTO);

        // Assert
        var okResult = resultado.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(logroActualizado);
    }

    //ActualizarLogrosGimnasio_NoEncontrado_RetornaNotFound
    [Fact]
    public async Task ActualizarLogrosGimnasio_NoEncontrado_RetornaNotFound()
    {
        // Arrange
        int gimnasioId = 999;
        int logroId = 999;
        var actualizarDTO = new ActualizarLogroGimnasioDTO { GimnasioId = gimnasioId, LogroId = logroId, EstaActivo = true };

        _mockActualizarLogroGimnasio
            .Setup(caso => caso.Ejecutar(It.IsAny<ActualizarLogroGimnasioDTO>()))
            .ReturnsAsync((LogroGimnasioDTO?)null);

        // Act
        var resultado = await _controller.ActualizarLogrosGimnasio(gimnasioId, logroId, actualizarDTO);

        // Assert
        var notFoundResult = resultado.Result as NotFoundResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }
}
