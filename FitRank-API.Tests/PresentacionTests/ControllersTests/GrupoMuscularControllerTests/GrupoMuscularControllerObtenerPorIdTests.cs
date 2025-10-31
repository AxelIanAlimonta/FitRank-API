using AutoMapper;
using FitRank_API.Application.CasosDeUso.GrupoMuscularCasosDeUso;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Presentacion.Controllers;
using Moq;
using Xunit;
using FitRank_API.Application.DTOs.GrupoMuscularDTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.tests.PresentacionTests.ControllersTests.GrupoMuscularControllerTests;

public class GrupoMuscularControllerObtenerPorIdTests
{
    private readonly GrupoMuscularController _controller;
    private readonly Mock<ObtenerGrupoMuscularPorIdCasoDeUso> _mockObtenerPorId;

    public GrupoMuscularControllerObtenerPorIdTests()
    {
        var mockRepositorio = new Mock<IGrupoMuscularRepositorio>();
        var mockMapper = new Mock<IMapper>();
        _mockObtenerPorId = new Mock<ObtenerGrupoMuscularPorIdCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _controller = new GrupoMuscularController(
            null!,
            _mockObtenerPorId.Object,
            null!,
            null!,
            null!
        );
    }

    [Fact]
    public async Task ObtenerPorId_GrupoMuscularNoExiste_RetornaNotFound()
    {
        // Arrange
        long grupoMuscularId = 1;
        _mockObtenerPorId.Setup(x => x.Ejecutar(grupoMuscularId)).ReturnsAsync((ObtenerGrupoMuscularDTO?)null);
        // Act
        var result = await _controller.ObtenerPorId(grupoMuscularId);

        // Assert
        var notFoundResult = result as NotFoundResult;

        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task ObtenerPorId_GrupoMuscularExiste_RetornaOkConGrupoMuscular()
    {
        // Arrange
        long grupoMuscularId = 1;
        var grupoMuscularDTO = new ObtenerGrupoMuscularDTO { Id = grupoMuscularId, Nombre = "Pecho" };
        _mockObtenerPorId.Setup(x => x.Ejecutar(grupoMuscularId)).ReturnsAsync(grupoMuscularDTO);
        // Act
        var result = await _controller.ObtenerPorId(grupoMuscularId);
        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var returnedGrupoMuscular = okResult.Value as ObtenerGrupoMuscularDTO;
        returnedGrupoMuscular.Should().NotBeNull();
        returnedGrupoMuscular!.Id.Should().Be(grupoMuscularId);
        returnedGrupoMuscular.Nombre.Should().Be("Pecho");
    }
}
