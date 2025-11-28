using FitRank_API.Domain.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.Mappings;
using FitRank_API.Application.DTOs.EntrenamientoDTOs;
using FitRank_API.Application.UseCases.Entrenamiento;

namespace CasosDeUsoTests.EntrenamientoCasosDeUsoTests;

public class EliminarEntrenamientoCasoDeUsoTests
{
    private readonly Mock<IEntrenamientoRepositorio> _entrenamientoRepositorioMock;

    public EliminarEntrenamientoCasoDeUsoTests()
    {

        _entrenamientoRepositorioMock = new Mock<IEntrenamientoRepositorio>();
    }

    [Fact]
    public async Task EliminarEntrenamiento_CuandoElEntrenamientoExiste_RetornaTrue()
    {
        // Arrange
        long entrenamientoId = 1;

        _entrenamientoRepositorioMock
            .Setup(repo => repo.EliminarAsync(entrenamientoId))
            .ReturnsAsync(true);

        var casoDeUso = new EliminarEntrenamientoCasoDeUso(_entrenamientoRepositorioMock.Object);

        // Act
        var resultado = await casoDeUso.Ejecutar(entrenamientoId);

        // Assert
        resultado.Should().BeTrue();

    }

    [Fact]
    public async Task EliminarEntrenamiento_CuandoElEntrenamientoNoExiste_RetornaFalse()
    {
        // Arrange
        long entrenamientoId = 99; // Suponiendo que este ID no existe

        _entrenamientoRepositorioMock
            .Setup(repo => repo.EliminarAsync(entrenamientoId))
            .ReturnsAsync(false);

        var casoDeUso = new EliminarEntrenamientoCasoDeUso(_entrenamientoRepositorioMock.Object);

        // Act
        var resultado = await casoDeUso.Ejecutar(entrenamientoId);

        // Assert
        resultado.Should().BeFalse();
    }
}