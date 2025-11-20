using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.MappingProfiles;
using FitRank_API.Application.DTOs.DiaDeLaSemanaDTOs;
using FitRank_API.Application.CasosDeUso.DiaDeLaSemanaCasoDeUso;
using FitRank_API.Application.Mappings;

namespace CasosDeUsoTests.DiaDeLaSemanaCasoDeUsoTests;

public class EliminarDiaDeLaSemanaCasoDeUsoTests
{
    private readonly Mock<IDiaDeLaSemanaRepositorio> _diaDeLaSemanaRepositorioMock;

    public EliminarDiaDeLaSemanaCasoDeUsoTests()
    {
        _diaDeLaSemanaRepositorioMock = new Mock<IDiaDeLaSemanaRepositorio>();
    }

    [Fact]
    public async Task EliminarDiaDeLaSemana_CuandoElDiaExiste_RetornaTrue()
    {
        // Arrange
        var diaId = 1L;

        _diaDeLaSemanaRepositorioMock
            .Setup(repo => repo.EliminarDiaDeLaSemanaAsync(diaId))
            .ReturnsAsync(true);

        var eliminarDiaDeLaSemanaCasoDeUso = new EliminarDiaDeLaSemanaCasoDeUso(_diaDeLaSemanaRepositorioMock.Object);

        // Act
        var resultado = await eliminarDiaDeLaSemanaCasoDeUso.Ejecutar(diaId);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task EliminarDiaDeLaSemana_CuandoElDiaNoExiste_RetornaFalse()
    {
        // Arrange
        var diaId = 99L; // Suponiendo que este ID no existe

        _diaDeLaSemanaRepositorioMock
            .Setup(repo => repo.EliminarDiaDeLaSemanaAsync(diaId))
            .ReturnsAsync(false);

        var eliminarDiaDeLaSemanaCasoDeUso = new EliminarDiaDeLaSemanaCasoDeUso(_diaDeLaSemanaRepositorioMock.Object);

        // Act
        var resultado = await eliminarDiaDeLaSemanaCasoDeUso.Ejecutar(diaId);

        // Assert
        resultado.Should().BeFalse();
    }
}