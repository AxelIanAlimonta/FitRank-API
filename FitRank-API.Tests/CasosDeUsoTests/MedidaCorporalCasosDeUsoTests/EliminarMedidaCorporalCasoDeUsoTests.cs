using FitRank_API.Application.Mappings;
using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Application.DTOs;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.DTOs.MedidaCorporalDTOs;
using FitRank_API.Application.CasosDeUso.MedidaCorporalCasosDeUso;

namespace CasosDeUsoTests.MedidaCorporalCasosDeUsoTests;

public class EliminarMedidaCorporalCasoDeUsoTests
{
    private readonly Mock<IMedidaCorporalRepositorio> _medidaCorporalRepositorioMock;
    public EliminarMedidaCorporalCasoDeUsoTests()
    {
        _medidaCorporalRepositorioMock = new Mock<IMedidaCorporalRepositorio>();
    }

    [Fact]
    public async Task Ejecutar_DeberiaEliminarMedidaCorporal_PorIdExistente()
    {
        // Arrange
        long medidaId = 1;

        _medidaCorporalRepositorioMock
            .Setup(repo => repo.EliminarAsync(medidaId))
            .ReturnsAsync(true);

        var eliminarMedidaCorporalCasoDeUso = new EliminarMedidaCorporalCasoDeUso(_medidaCorporalRepositorioMock.Object);

        // Act
        var resultado = await eliminarMedidaCorporalCasoDeUso.Ejecutar(medidaId);

        // Assert
        resultado.Should().BeTrue();
        _medidaCorporalRepositorioMock.Verify(repo => repo.EliminarAsync(medidaId), Times.Once);
    }

    [Fact]
    public async Task Ejecutar_DeberiaRetornarFalse_PorIdNoExistente()
    {
        // Arrange
        long medidaId = 99; // Suponiendo que este ID no existe

        _medidaCorporalRepositorioMock
            .Setup(repo => repo.EliminarAsync(medidaId))
            .ReturnsAsync(false);

        var eliminarMedidaCorporalCasoDeUso = new EliminarMedidaCorporalCasoDeUso(_medidaCorporalRepositorioMock.Object);

        // Act
        var resultado = await eliminarMedidaCorporalCasoDeUso.Ejecutar(medidaId);

        // Assert
        resultado.Should().BeFalse();
        _medidaCorporalRepositorioMock.Verify(repo => repo.EliminarAsync(medidaId), Times.Once);
    }

}