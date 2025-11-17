using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.DTOs.SerieDTOs;
using FitRank_API.Application.CasosDeUso.SerieCasosDeUso;
using FitRank_API.Application.MappingProfiles;

namespace CasosDeUsoTests.SerieCasosDeUsoTests;

public class EliminarSerieCasoDeUsoTest
{
    private readonly Mock<ISerieRepositorio> _serieRepositoryMock;
    private readonly EliminarSerieCasoDeUso _eliminarSerieCasoDeUso;

    public EliminarSerieCasoDeUsoTest()
    {
        _serieRepositoryMock = new Mock<ISerieRepositorio>();
        _eliminarSerieCasoDeUso = new EliminarSerieCasoDeUso(_serieRepositoryMock.Object);
    }

    //eliminar serie exitoso
    [Fact]
    public async Task EliminarSerie_CuandoElIdExiste_DevuelveTrue()
    {
        // Arrange
        var serieId = 1;

        _serieRepositoryMock
            .Setup(repo => repo.EliminarAsync(serieId))
            .ReturnsAsync(true);

        // Act
        var resultado = await _eliminarSerieCasoDeUso.Ejecutar(serieId);

        // Assert
        resultado.Should().BeTrue();

    }

    [Fact]
    public async Task EliminarSerie_CuandoElIdNoExiste_DevuelveFalse()
    {
        // Arrange
        var serieId = 99; // Suponiendo que este ID no existe

        _serieRepositoryMock
            .Setup(repo => repo.EliminarAsync(serieId))
            .ReturnsAsync(false);

        // Act
        var resultado = await _eliminarSerieCasoDeUso.Ejecutar(serieId);

        // Assert
        resultado.Should().BeFalse();
    }

}