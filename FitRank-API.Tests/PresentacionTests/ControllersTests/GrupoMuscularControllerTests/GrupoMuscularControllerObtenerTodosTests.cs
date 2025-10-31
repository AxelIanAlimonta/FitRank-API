using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using FitRank_API.Application.CasosDeUso.GrupoMuscularCasosDeUso;
using FitRank_API.Presentacion.Controllers;
using FitRank_API.Application.DTOs.GrupoMuscularDTOs;
using AutoMapper;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.tests.PresentacionTests.ControllersTests.GrupoMuscularControllerTests;

public class GrupoMuscularControllerObtenerTodosTests
{
    private readonly GrupoMuscularController _controller;
    private readonly Mock<ObtenerTodosLosGruposMuscularesCasoDeUso> _mockObtenerTodos;

    public GrupoMuscularControllerObtenerTodosTests()
    {
        var mockRepositorio = new Mock<IGrupoMuscularRepositorio>();
        var mockMapper = new Mock<IMapper>();
        _mockObtenerTodos = new Mock<ObtenerTodosLosGruposMuscularesCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _controller = new GrupoMuscularController(
            _mockObtenerTodos.Object,
            null!,
            null!,
            null!,
            null!
        );
    }

    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaDeGruposMusculares()
    {
        // Arrange
        var gruposMusculares = new List<ObtenerGrupoMuscularDTO>
        {
            new ObtenerGrupoMuscularDTO { Id = 1, Nombre = "Pecho" },
            new ObtenerGrupoMuscularDTO { Id = 2, Nombre = "Espalda" }
        };
        _mockObtenerTodos.Setup(x => x.Ejecutar()).ReturnsAsync(gruposMusculares);
        // Act
        var result = await _controller.ObtenerTodos();
        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var returnedGruposMusculares = okResult.Value as List<ObtenerGrupoMuscularDTO>;
        returnedGruposMusculares.Should().NotBeNull();
        returnedGruposMusculares!.Count.Should().Be(2);
        returnedGruposMusculares[0].Nombre.Should().Be("Pecho");
        returnedGruposMusculares[1].Nombre.Should().Be("Espalda");
    }

    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaVacia()
    {
        // Arrange
        var gruposMusculares = new List<ObtenerGrupoMuscularDTO>();
        _mockObtenerTodos.Setup(x => x.Ejecutar()).ReturnsAsync(gruposMusculares);
        // Act
        var result = await _controller.ObtenerTodos();
        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var returnedGruposMusculares = okResult.Value as List<ObtenerGrupoMuscularDTO>;
        returnedGruposMusculares.Should().NotBeNull();
        returnedGruposMusculares!.Count.Should().Be(0);
    }

    [Fact]
    public async Task ObtenerTodos_LanzaExcepcion_RetornaStatus500()
    {
        // Arrange
        _mockObtenerTodos.Setup(x => x.Ejecutar()).ThrowsAsync(new Exception("Error inesperado"));
        // Act
        var result = await _controller.ObtenerTodos();
        // Assert
        var objectResult = result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Error inesperado");
    }
}
