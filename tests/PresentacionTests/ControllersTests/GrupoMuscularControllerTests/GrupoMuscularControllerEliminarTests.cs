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

public class EliminarGrupoMuscularCasoDeUsoTests
{
    private readonly Mock<EliminarGrupoMuscularCasoDeUso> _eliminarGrupoMuscularCasoDeUsoMock;
    private readonly GrupoMuscularController _grupoMuscularController;

    public EliminarGrupoMuscularCasoDeUsoTests()
    {
        var mockRepositorio = new Mock<IGrupoMuscularRepositorio>();
        _eliminarGrupoMuscularCasoDeUsoMock = new Mock<EliminarGrupoMuscularCasoDeUso>(mockRepositorio.Object);
        _grupoMuscularController = new GrupoMuscularController(
            null,
            null,
            _eliminarGrupoMuscularCasoDeUsoMock.Object,
            null,
            null
        );

    }

    [Fact]
    public async Task Eliminar_GrupoMuscular_Existente_Deberia_Retornar_NoContent()
    {
        // Arrange
        long grupoMuscularId = 1;
        _eliminarGrupoMuscularCasoDeUsoMock
            .Setup(x => x.Ejecutar(grupoMuscularId))
            .ReturnsAsync(true);

        // Act
        var result = await _grupoMuscularController.Eliminar(grupoMuscularId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Eliminar_GrupoMuscular_NoExistente_Deberia_Retornar_NotFound()
    {
        // Arrange
        long grupoMuscularId = 1;
        _eliminarGrupoMuscularCasoDeUsoMock
            .Setup(x => x.Ejecutar(grupoMuscularId))
            .ReturnsAsync(false);

        // Act
        var result = await _grupoMuscularController.Eliminar(grupoMuscularId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Eliminar_Cuando_Ocurre_Error_Deberia_Retornar_StatusCode500()
    {
        // Arrange
        long grupoMuscularId = 1;
        _eliminarGrupoMuscularCasoDeUsoMock
            .Setup(x => x.Ejecutar(grupoMuscularId))
            .ThrowsAsync(new Exception("Error inesperado"));

        // Act
        var result = await _grupoMuscularController.Eliminar(grupoMuscularId);

        // Assert
        var objectResult = result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Error inesperado");
    }
}
