using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using FitRank_API.Application.DTOs.LogroGimnasioDTOs;
using AutoMapper;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Application.CasosDeUso.LogroGimnasioCasosDeUso;
using FitRank_API.Presentacion.Controllers;

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

        _mockObtenerLogrosGimnasio = new Mock<ObtenerLogrosGimnasioCasoDeUso>(
            mockLogroGimnasioRepositorio.Object, mockMapper.Object);
        _mockActualizarLogroGimnasio = new Mock<ActualizarLogroGimnasioCasoDeUso>(
            mockLogroGimnasioRepositorio.Object, mockLogroRepositorio.Object, mockMapper.Object);

        _controller = new LogrosGimnasioController(
            _mockObtenerLogrosGimnasio.Object,
            _mockActualizarLogroGimnasio.Object
        );
    }

    #region ObtenerLogrosGimnasio Tests

    [Fact]
    public async Task ObtenerLogrosGimnasio_RetornaOkResult_ConListaCompleta()
    {
        // Arrange
        int gimnasioId = 10;
        var listaLogros = new List<LogroGimnasioDTO>
        {
            new LogroGimnasioDTO 
            { 
                LogroId = 1, 
                GimnasioId = gimnasioId, 
                EstaHabilitado = true, 
                Nombre = "Logro 1" 
            },
            new LogroGimnasioDTO 
            { 
                LogroId = 2, 
                GimnasioId = gimnasioId, 
                EstaHabilitado = false, 
                Nombre = "Logro 2" 
            }
        };

        _mockObtenerLogrosGimnasio.Setup(caso => caso.Ejecutar(gimnasioId)).ReturnsAsync(listaLogros);

        // Act
        var resultado = await _controller.ObtenerLogrosGimnasio(gimnasioId);

        // Assert
        var okResult = resultado.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaLogros);
    }

    [Fact]
    public async Task ObtenerLogrosGimnasio_RetornaOkResult_ConListaVacia()
    {
        // Arrange
        int gimnasioId = 10;
        _mockObtenerLogrosGimnasio.Setup(caso => caso.Ejecutar(gimnasioId))
            .ReturnsAsync(new List<LogroGimnasioDTO>());

        // Act
        var resultado = await _controller.ObtenerLogrosGimnasio(gimnasioId);

        // Assert
        var okResult = resultado.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerLogrosGimnasio_GimnasioIdCero_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.ObtenerLogrosGimnasio(0);

        // Assert
        var badRequestResult = resultado.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerLogrosGimnasio_GimnasioIdNegativo_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.ObtenerLogrosGimnasio(-5);

        // Assert
        var badRequestResult = resultado.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerLogrosGimnasio_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockObtenerLogrosGimnasio.Setup(caso => caso.Ejecutar(1)).ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.ObtenerLogrosGimnasio(1);

        // Assert
        var statusCodeResult = resultado.Result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ActualizarLogrosGimnasio Tests

    [Fact]
    public async Task ActualizarLogrosGimnasio_RetornaOkResult_ConObjetoActualizado()
    {
        // Arrange
        int gimnasioId = 10;
        int logroId = 5;
        var actualizarDTO = new ActualizarLogroGimnasioDTO 
        { 
            GimnasioId = gimnasioId, 
            LogroId = logroId, 
            EstaActivo = true 
        };
        var logroActualizado = new LogroGimnasioDTO 
        { 
            GimnasioId = gimnasioId, 
            LogroId = logroId, 
            EstaHabilitado = true, 
            Nombre = "Logro 5" 
        };

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

    [Fact]
    public async Task ActualizarLogrosGimnasio_GimnasioIdCero_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarLogroGimnasioDTO();

        // Act
        var resultado = await _controller.ActualizarLogrosGimnasio(0, 1, dto);

        // Assert
        var badRequestResult = resultado.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ActualizarLogrosGimnasio_GimnasioIdNegativo_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarLogroGimnasioDTO();

        // Act
        var resultado = await _controller.ActualizarLogrosGimnasio(-5, 1, dto);

        // Assert
        var badRequestResult = resultado.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ActualizarLogrosGimnasio_LogroIdCero_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarLogroGimnasioDTO();

        // Act
        var resultado = await _controller.ActualizarLogrosGimnasio(1, 0, dto);

        // Assert
        var badRequestResult = resultado.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ActualizarLogrosGimnasio_LogroIdNegativo_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarLogroGimnasioDTO();

        // Act
        var resultado = await _controller.ActualizarLogrosGimnasio(1, -3, dto);

        // Assert
        var badRequestResult = resultado.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ActualizarLogrosGimnasio_DtoNulo_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.ActualizarLogrosGimnasio(1, 1, null!);

        // Assert
        var badRequestResult = resultado.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ActualizarLogrosGimnasio_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("EstaActivo", "Requerido");
        var dto = new ActualizarLogroGimnasioDTO();

        // Act
        var resultado = await _controller.ActualizarLogrosGimnasio(1, 1, dto);

        // Assert
        var badRequestResult = resultado.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ActualizarLogrosGimnasio_NoEncontrado_RetornaNotFound()
    {
        // Arrange
        int gimnasioId = 999;
        int logroId = 999;
        var actualizarDTO = new ActualizarLogroGimnasioDTO();

        _mockActualizarLogroGimnasio
            .Setup(caso => caso.Ejecutar(It.IsAny<ActualizarLogroGimnasioDTO>()))
            .ReturnsAsync((LogroGimnasioDTO?)null);

        // Act
        var resultado = await _controller.ActualizarLogrosGimnasio(gimnasioId, logroId, actualizarDTO);

        // Assert
        var notFoundResult = resultado.Result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task ActualizarLogrosGimnasio_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var dto = new ActualizarLogroGimnasioDTO();
        _mockActualizarLogroGimnasio.Setup(caso => caso.Ejecutar(It.IsAny<ActualizarLogroGimnasioDTO>()))
            .ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.ActualizarLogrosGimnasio(1, 1, dto);

        // Assert
        var statusCodeResult = resultado.Result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion
}
