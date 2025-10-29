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

public class GrupoMuscularControllerAgregarTests
{
    private readonly GrupoMuscularController _controller;
    private readonly Mock<AgregarGrupoMuscularCasoDeUso> _mockAgregar;
    public GrupoMuscularControllerAgregarTests()
    {
        var mockRepositorio = new Mock<IGrupoMuscularRepositorio>();
        var mockMapper = new Mock<IMapper>();
        _mockAgregar = new Mock<AgregarGrupoMuscularCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _controller = new GrupoMuscularController(
            null!,
            null!,
            null!,
            null!,
            _mockAgregar.Object
        );
    }

    [Fact]
    public async Task Agregar_RetornaCreatedAtActionResult_ConGrupoMuscularCreado()
    {
        // Arrange
        var agregarDTO = new AgregarGrupoMuscularDTO { Nombre = "Piernas" };
        var obtenerDTO = new ObtenerGrupoMuscularDTO { Id = 1, Nombre = "Piernas" };
        _mockAgregar.Setup(x => x.Ejecutar(agregarDTO)).ReturnsAsync(obtenerDTO);
        // Act
        var result = await _controller.Agregar(agregarDTO);
        // Assert
        var createdAtActionResult = result as CreatedAtActionResult;
        createdAtActionResult.Should().NotBeNull();
        createdAtActionResult!.StatusCode.Should().Be(201);
        var returnedGrupoMuscular = createdAtActionResult.Value as ObtenerGrupoMuscularDTO;
        returnedGrupoMuscular.Should().NotBeNull();
        returnedGrupoMuscular!.Id.Should().Be(1);
        returnedGrupoMuscular.Nombre.Should().Be("Piernas");
    }


    [Fact]
    public async Task Agregar_LanzaExcepcion_RetornaStatusCode500()
    {
        // Arrange
        var agregarDTO = new AgregarGrupoMuscularDTO { Nombre = "Piernas" };
        _mockAgregar.Setup(x => x.Ejecutar(agregarDTO)).ThrowsAsync(new Exception("Hubo un error en el servidor."));
        //Act
        var result = await _controller.Agregar(agregarDTO);
        //Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
        statusCodeResult.Value.Should().Be("Hubo un error en el servidor.");
    }

    [Fact]
    public async Task Agregar_RetornaBadRequest_CuandoDTOEsNulo()
    {
        // Arrange
        AgregarGrupoMuscularDTO agregarDTO = null!;
        // Act
        var result = await _controller.Agregar(agregarDTO);
        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("El grupo muscular no puede ser nulo.");
    }

}
