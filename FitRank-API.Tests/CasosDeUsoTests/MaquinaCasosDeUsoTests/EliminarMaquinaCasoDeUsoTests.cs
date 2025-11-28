using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.DTOs.MaquinaDTOs;
using FitRank_API.Application.CasosDeUso.MaquinaCasosDeUso;

namespace CasosDeUsoTests.MaquinaCasosDeUsoTests;

public class EliminarMaquinaCasoDeUsoTests
{
    private readonly Mock<IMaquinaRepositorio> _maquinaRepositorioMock;

    public EliminarMaquinaCasoDeUsoTests()
    {

        _maquinaRepositorioMock = new Mock<IMaquinaRepositorio>();
    }

    //eliminar maquina exitoso
    [Fact]
    public async Task EliminarMaquina_CuandoLaMaquinaExiste_RetornaExito()
    {
        // Arrange
        var maquinaId = 1;

        _maquinaRepositorioMock
            .Setup(repo => repo.EliminarMaquina(maquinaId))
            .ReturnsAsync(true);

        var eliminarMaquinaCasoDeUso = new EliminarMaquinaCasoDeUso(_maquinaRepositorioMock.Object);

        // Act
        var resultado = await eliminarMaquinaCasoDeUso.Ejecutar(maquinaId);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task EliminarMaquina_CuandoLaMaquinaNoExiste_RetornaFallo()
    {
        // Arrange
        var maquinaId = 99; // Suponiendo que este ID no existe

        _maquinaRepositorioMock
            .Setup(repo => repo.ObtenerMaquinaPorId(maquinaId))
            .ReturnsAsync((Maquina?)null);

        var eliminarMaquinaCasoDeUso = new EliminarMaquinaCasoDeUso(_maquinaRepositorioMock.Object);

        // Act
        var resultado = await eliminarMaquinaCasoDeUso.Ejecutar(maquinaId);

        // Assert
        resultado.Should().BeFalse();
    }
}