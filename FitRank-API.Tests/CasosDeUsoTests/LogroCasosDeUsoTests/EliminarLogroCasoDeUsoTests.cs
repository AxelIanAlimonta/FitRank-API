using FitRank_API.Domain.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.LogroCasosDeUso;
using FitRank_API.Application.DTOs.LogroDTOs;
using FitRank_API.Application.Mappings;

namespace CasosDeUsoTests.LogroCasoDeUsoTests;

public class EliminarLogroCasoDeUsoTests
{
    private readonly Mock<ILogroRepositorio> _logroRepositorioMock;

    public EliminarLogroCasoDeUsoTests()
    {
        _logroRepositorioMock = new Mock<ILogroRepositorio>();
    }

    [Fact]
    public async Task EliminarLogro_CuandoElLogroExiste_RetornaVerdadero()
    {
        // Arrange
        int logroIdAEliminar = 1;

        _logroRepositorioMock
            .Setup(repo => repo.EliminarLogro(logroIdAEliminar))
            .ReturnsAsync(true);

        var eliminarLogroCasoDeUso = new EliminarLogroCasoDeUso(
            _logroRepositorioMock.Object
        );

        // Act
        var resultado = await eliminarLogroCasoDeUso.Ejecutar(logroIdAEliminar);

        // Assert
        resultado.Should().BeTrue();
        _logroRepositorioMock.Verify(
            repo => repo.EliminarLogro(logroIdAEliminar),
            Times.Once
        );
    }

    [Fact]
    public async Task EliminarLogro_CuandoElLogroNoExiste_RetornaFalso()
    {
        // Arrange
        int logroIdAEliminar = 99; // Suponiendo que este ID no existe

        _logroRepositorioMock
            .Setup(repo => repo.EliminarLogro(logroIdAEliminar))
            .ReturnsAsync(false);

        var eliminarLogroCasoDeUso = new EliminarLogroCasoDeUso(
            _logroRepositorioMock.Object
        );

        // Act
        var resultado = await eliminarLogroCasoDeUso.Ejecutar(logroIdAEliminar);

        // Assert
        resultado.Should().BeFalse();
        _logroRepositorioMock.Verify(
            repo => repo.EliminarLogro(logroIdAEliminar),
            Times.Once
        );
    }
}