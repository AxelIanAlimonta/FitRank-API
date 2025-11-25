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

    [Fact]
    public async Task DebeLlamarRepositorioConIdCorrecto()
    {
        // Arrange
        var fotoId = 123L;

        _fotoRepositorioMock
            .Setup(repo => repo.EliminarAsync(fotoId))
            .ReturnsAsync(true);

        var casoDeUso = new EliminarFotoCasoDeUso(_fotoRepositorioMock.Object);

        // Act
        await casoDeUso.Ejecutar(fotoId);

        // Assert
        _fotoRepositorioMock.Verify(repo => repo.EliminarAsync(fotoId), Times.Once);
    }

    [Fact]
    public async Task DebeRetornarResultadoDelRepositorio()
    {
        // Arrange
        var fotoId = 456L;
        var resultadoEsperado = true;

        _fotoRepositorioMock
            .Setup(repo => repo.EliminarAsync(fotoId))
            .ReturnsAsync(resultadoEsperado);

        var casoDeUso = new EliminarFotoCasoDeUso(_fotoRepositorioMock.Object);

        // Act
        var resultado = await casoDeUso.Ejecutar(fotoId);

        // Assert
        resultado.Should().Be(resultadoEsperado);
    }

    [Fact]
    public async Task DeberiaEliminarFotosConDiferentesIds()
    {
        // Arrange
        var fotoId1 = 1L;
        var fotoId2 = 9999L;

        _fotoRepositorioMock
            .Setup(repo => repo.EliminarAsync(fotoId1))
            .ReturnsAsync(true);

        _fotoRepositorioMock
            .Setup(repo => repo.EliminarAsync(fotoId2))
            .ReturnsAsync(false);

        var casoDeUso = new EliminarFotoCasoDeUso(_fotoRepositorioMock.Object);

        // Act
        var resultado1 = await casoDeUso.Ejecutar(fotoId1);
        var resultado2 = await casoDeUso.Ejecutar(fotoId2);

        // Assert
        resultado1.Should().BeTrue();
        resultado2.Should().BeFalse();
        _fotoRepositorioMock.Verify(repo => repo.EliminarAsync(It.IsAny<long>()), Times.Exactly(2));
    }
}