using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using FitRank_API.Application.CasosDeUso.GrupoMuscularCasosDeUso;
using FitRank_API.Presentacion.Controllers;
using FitRank_API.Application.DTOs.GrupoMuscularDTOs;
using AutoMapper;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Application.DTOs;


namespace FitRank_API.tests.PresentacionTests.ControllersTests.GrupoMuscularControllerTests;

public class GrupoMuscularControllerActualizarTests
{
    private readonly GrupoMuscularController _controller;
    private readonly Mock<ActualizarGrupoMuscularCasoDeUso> _mockActualizar;

    public GrupoMuscularControllerActualizarTests()
    {
        var mockRepositorio = new Mock<IGrupoMuscularRepositorio>();
        var mockMapper = new Mock<IMapper>();
        _mockActualizar = new Mock<ActualizarGrupoMuscularCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _controller = new GrupoMuscularController(
            null!,
            null!,
            null!,
            _mockActualizar.Object,
            null!
        );
    }

    [Fact]
    public async Task Actualizar_RetornaOkObjectResult_ConGrupoMuscularActualizado()
    {
        // Arrange
        var actualizarDTO = new ActualizarGrupoMuscularDTO { Id = 1, Nombre = "Espalda" };
        var obtenerDTO = new ObtenerGrupoMuscularDTO { Id = 1, Nombre = "Espalda" };

        _mockActualizar.Setup(x => x.Ejecutar(actualizarDTO)).ReturnsAsync(obtenerDTO);

        // Act
        var result = await _controller.Actualizar(1, actualizarDTO);

        // Assert
        var okObjectResult = result as OkObjectResult;
        okObjectResult.Should().NotBeNull();
        okObjectResult!.StatusCode.Should().Be(200);
        var returnedGrupoMuscular = okObjectResult.Value as ObtenerGrupoMuscularDTO;
        returnedGrupoMuscular.Should().NotBeNull();
        returnedGrupoMuscular!.Id.Should().Be(1);
        returnedGrupoMuscular.Nombre.Should().Be("Espalda");
    }

    [Fact]
    public async Task Actualizar_GrupoMuscularNoEncontrado_RetornaNotFoundResult()
    {
        // Arrange
        var actualizarDTO = new ActualizarGrupoMuscularDTO { Id = 99, Nombre = "NoExiste" };
        _mockActualizar.Setup(x => x.Ejecutar(actualizarDTO)).ReturnsAsync((ObtenerGrupoMuscularDTO?)null);
        // Act
        var result = await _controller.Actualizar(99, actualizarDTO);
        // Assert
        var notFoundResult = result as NotFoundResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Actualizar_LanzaExcepcion_RetornaStatusCode500()
    {
        // Arrange
        var actualizarDTO = new ActualizarGrupoMuscularDTO { Id = 1, Nombre = "Espalda" };
        _mockActualizar.Setup(x => x.Ejecutar(actualizarDTO)).ThrowsAsync(new Exception("Hubo un error en el servidor."));
        // Act
        var result = await _controller.Actualizar(1, actualizarDTO);
        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task Actualizar_IdNoCoincide_RetornaBadRequestResult()
    {
        // Arrange
        var actualizarDTO = new ActualizarGrupoMuscularDTO { Id = 1, Nombre = "Espalda" };
        // Act
        var result = await _controller.Actualizar(2, actualizarDTO);
        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_DTONulo_RetornaBadRequestResult()
    {
        // Arrange
        ActualizarGrupoMuscularDTO actualizarDTO = null!;
        // Act
        var result = await _controller.Actualizar(1, actualizarDTO);
        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

}
