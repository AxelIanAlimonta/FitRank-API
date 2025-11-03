using FitRank_API.Application.Mappings;
using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.FotoCasosDeUso;
using FitRank_API.Application.DTOs.FotoDTOs;
using FitRank_API.Domain.Entities;

namespace CasosDeUsoTests.FotoCasosDeUsoTests;

public class EliminarFotoCasoDeUsoTests
{
    private readonly Mock<IFotoRepositorio> _fotoRepositorioMock;

    public EliminarFotoCasoDeUsoTests()
    {
        _fotoRepositorioMock = new Mock<IFotoRepositorio>();
    }

    [Fact]
    public async Task EliminarFoto_CuandoElIdExiste_DevuelveTrue()
    {
        // Arrange
        var fotoId = 1;

        _fotoRepositorioMock
            .Setup(repo => repo.EliminarAsync(fotoId))
            .ReturnsAsync(true);

        var eliminarFotoCasoDeUso = new EliminarFotoCasoDeUso(_fotoRepositorioMock.Object);

        // Act
        var resultado = await eliminarFotoCasoDeUso.Ejecutar(fotoId);

        // Assert
        resultado.Should().BeTrue();

    }

    [Fact]
    public async Task EliminarFoto_CuandoElIdNoExiste_DevuelveFalse()
    {
        // Arrange
        var fotoId = 99; // Suponiendo que este ID no existe

        _fotoRepositorioMock
            .Setup(repo => repo.EliminarAsync(fotoId))
            .ReturnsAsync(false);

        var eliminarFotoCasoDeUso = new EliminarFotoCasoDeUso(_fotoRepositorioMock.Object);

        // Act
        var resultado = await eliminarFotoCasoDeUso.Ejecutar(fotoId);

        // Assert
        resultado.Should().BeFalse();
    }
}