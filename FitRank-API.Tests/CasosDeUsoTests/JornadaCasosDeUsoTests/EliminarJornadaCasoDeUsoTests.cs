using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.DTOs.JornadaDTOs;
using FitRank_API.Application.Mappings;
using FitRank_API.Application.CasosDeUso.JornadaCasosDeUso;

namespace CasosDeUsoTests.JornadaCasosDeUsoTests;

public class EliminarJornadaCasoDeUsoTests
{
    private readonly Mock<IJornadaRepositorio> _jornadaRepositorioMock;

    public EliminarJornadaCasoDeUsoTests()
    {
        _jornadaRepositorioMock = new Mock<IJornadaRepositorio>();
    }

    [Fact]
    public async Task EliminarJornada_CuandoLaJornadaExiste_RetornaTrue()
    {
        // Arrange
        var jornadaId = 1;

        _jornadaRepositorioMock
            .Setup(repo => repo.EliminarJornadaAsync(jornadaId))
            .ReturnsAsync(true);

        var eliminarJornadaCasoDeUso = new EliminarJornadaCasoDeUso(_jornadaRepositorioMock.Object);

        // Act
        var resultado = await eliminarJornadaCasoDeUso.Ejecutar(jornadaId);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task EliminarJornada_CuandoLaJornadaNoExiste_RetornaFalse()
    {
        // Arrange
        var jornadaId = 99; // Suponiendo que este ID no existe

        _jornadaRepositorioMock
            .Setup(repo => repo.EliminarJornadaAsync(jornadaId))
            .ReturnsAsync(false);

        var eliminarJornadaCasoDeUso = new EliminarJornadaCasoDeUso(_jornadaRepositorioMock.Object);

        // Act
        var resultado = await eliminarJornadaCasoDeUso.Ejecutar(jornadaId);

        // Assert
        resultado.Should().BeFalse();
    }
}