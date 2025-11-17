using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.MappingProfiles;
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
}