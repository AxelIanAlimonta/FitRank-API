using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.Mappings;
using FitRank_API.Application.DTOs.ActividadDTOs;
using FitRank_API.Application.UseCases.Actividad;

namespace CasosDeUsoTests.ActividadCasosDeUsoTests;

public class EliminarActividadCasoDeUsoTests
{
    private readonly Mock<IActividadRepositorio> _actividadRepositorioMock;

    public EliminarActividadCasoDeUsoTests()
    {

        _actividadRepositorioMock = new Mock<IActividadRepositorio>();
    }

    [Fact]
    public async Task EliminarActividad_CuandoLaActividadExiste_EliminaLaActividad()
    {
        // Arrange
        var actividadId = 1;

        _actividadRepositorioMock
            .Setup(repo => repo.EliminarAsync(actividadId))
            .ReturnsAsync(true);

        var eliminarActividadCasoDeUso = new EliminarActividadCasoDeUso(_actividadRepositorioMock.Object);

        // Act
        var resultado = await eliminarActividadCasoDeUso.Ejecutar(actividadId);

        // Assert
        resultado.Should().BeTrue();
        _actividadRepositorioMock.Verify(repo => repo.EliminarAsync(actividadId), Times.Once);
    }

    [Fact]
    public async Task EliminarActividad_CuandoLaActividadNoExiste_NoHaceNada()
    {
        // Arrange
        var actividadId = 99; // Suponiendo que este ID no existe

        _actividadRepositorioMock
            .Setup(repo => repo.EliminarAsync(actividadId))
            .ReturnsAsync(false);

        var eliminarActividadCasoDeUso = new EliminarActividadCasoDeUso(_actividadRepositorioMock.Object);

        // Act
        var resultado = await eliminarActividadCasoDeUso.Ejecutar(actividadId);

        // Assert
        resultado.Should().BeFalse();
        _actividadRepositorioMock.Verify(repo => repo.EliminarAsync(actividadId), Times.Once);
    }

    [Fact]
    public async Task EliminarActividad_DebeLlamarRepositorioConIdCorrecto()
    {
        // Arrange
        var actividadId = 42L;

        _actividadRepositorioMock
            .Setup(repo => repo.EliminarAsync(actividadId))
            .ReturnsAsync(true);

        var casoDeUso = new EliminarActividadCasoDeUso(_actividadRepositorioMock.Object);

        // Act
        await casoDeUso.Ejecutar(actividadId);

        // Assert
        _actividadRepositorioMock.Verify(repo => repo.EliminarAsync(actividadId), Times.Once);
    }

    [Fact]
    public async Task EliminarActividad_DebeRetornarResultadoDelRepositorio()
    {
        // Arrange
        var actividadId = 10L;
        var resultadoEsperado = true;

        _actividadRepositorioMock
            .Setup(repo => repo.EliminarAsync(actividadId))
            .ReturnsAsync(resultadoEsperado);

        var casoDeUso = new EliminarActividadCasoDeUso(_actividadRepositorioMock.Object);

        // Act
        var resultado = await casoDeUso.Ejecutar(actividadId);

        // Assert
        resultado.Should().Be(resultadoEsperado);
    }
}