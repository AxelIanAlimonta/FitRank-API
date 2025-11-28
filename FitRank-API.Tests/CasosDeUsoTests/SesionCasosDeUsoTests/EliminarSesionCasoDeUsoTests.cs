using AutoMapper;
using FitRank_API.Application.CasosDeUso.SesionCasosDeUso;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace CasosDeUsoTests.SesionCasosDeUsoTests;

public class EliminarSesionCasoDeUsoTests
{
    private readonly Mock<ISesionRepositorio> _sesionRepositorioMock;
    private readonly EliminarSesionCasoDeUso _eliminarSesionCasoDeUso;

    public EliminarSesionCasoDeUsoTests()
    {
        _sesionRepositorioMock = new Mock<ISesionRepositorio>();
        _eliminarSesionCasoDeUso = new EliminarSesionCasoDeUso(_sesionRepositorioMock.Object);
    }

    //eliminar sesión exitosa
    [Fact]
    public async Task EliminarSesionCasoDeUso_EliminacionExitosa_RetornaTrue()
    {
        // Arrange
        var sesionId = 1L;
        _sesionRepositorioMock
            .Setup(repo => repo.EliminarAsync(sesionId))
            .ReturnsAsync(true);
        // Act
        var resultado = await _eliminarSesionCasoDeUso.Ejecutar(sesionId);
        // Assert
        resultado.Should().BeTrue();
    }

    //eliminar sesión no existente
    [Fact]
    public async Task EliminarSesionCasoDeUso_IdNoExistente_RetornaFalse()
    {
        // Arrange
        var sesionId = 999L;
        _sesionRepositorioMock
            .Setup(repo => repo.EliminarAsync(sesionId))
            .ReturnsAsync(false);
        // Act
        var resultado = await _eliminarSesionCasoDeUso.Ejecutar(sesionId);
        // Assert
        resultado.Should().BeFalse();
    }
}