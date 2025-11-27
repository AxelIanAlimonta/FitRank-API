using FitRank_API.Domain.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.EjercicioAsignadoCasoDeUso;
using FitRank_API.Application.DTOs.EjercicioAsignadoDTOs;
using FitRank_API.Application.Mappings;

namespace CasosDeUsoTests.EjercicioAsignadoCasoDeUsoTests;

public class EliminarEjercicioAsignadoCasoDeUsoTests
{
    private readonly Mock<IEjercicioAsignadoRepositorio> _ejercicioAsignadoRepositorioMock;

    public EliminarEjercicioAsignadoCasoDeUsoTests()
    {
        _ejercicioAsignadoRepositorioMock = new Mock<IEjercicioAsignadoRepositorio>();
    }

    //eliminar ejercicio asignado tiene exito
    [Fact]
    public async Task Ejecutar_DeberiaEliminarEjercicioAsignado_CuandoElEjercicioAsignadoExiste()
    {
        // Arrange
        var ejercicioAsignadoId = 1;

        _ejercicioAsignadoRepositorioMock.Setup(repo => repo.EliminarAsync(ejercicioAsignadoId))
            .ReturnsAsync(true);

        var eliminarEjercicioAsignadoCasoDeUso = new EliminarEjercicioAsignadoCasoDeUso(_ejercicioAsignadoRepositorioMock.Object);

        // Act
        var resultado = await eliminarEjercicioAsignadoCasoDeUso.Ejecutar(ejercicioAsignadoId);

        // Assert
        resultado.Should().BeTrue();
    }

    //eliminar ejercicio asignado falla porque no existe
    [Fact]
    public async Task Ejecutar_DeberiaFallarAlEliminarEjercicioAsignado_CuandoElEjercicioAsignadoNoExiste()
    {
        // Arrange
        var ejercicioAsignadoId = 999; // ID que no existe

        _ejercicioAsignadoRepositorioMock.Setup(repo => repo.EliminarAsync(ejercicioAsignadoId))
            .ReturnsAsync(false);

        var eliminarEjercicioAsignadoCasoDeUso = new EliminarEjercicioAsignadoCasoDeUso(_ejercicioAsignadoRepositorioMock.Object);

        // Act
        var resultado = await eliminarEjercicioAsignadoCasoDeUso.Ejecutar(ejercicioAsignadoId);

        // Assert
        resultado.Should().BeFalse();
    }
}