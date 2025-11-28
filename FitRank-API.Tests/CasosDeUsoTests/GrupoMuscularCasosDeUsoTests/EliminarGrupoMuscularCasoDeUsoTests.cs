using FitRank_API.Application.CasosDeUso.GrupoMuscularCasosDeUso;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Application.DTOs;
using FitRank_API.Domain.Entities;
using FluentAssertions;

namespace CasosDeUsoTests.GrupoMuscularCasosDeUsoTests;

public class EliminarGrupoMuscularCasoDeUsoTests
{
    private readonly Mock<IGrupoMuscularRepositorio> _grupoMuscularRepositorioMock;
    private readonly EliminarGrupoMuscularCasoDeUso _eliminarGrupoMuscularCasoDeUso;
    public EliminarGrupoMuscularCasoDeUsoTests()
    {
        _grupoMuscularRepositorioMock = new Mock<IGrupoMuscularRepositorio>();
        _eliminarGrupoMuscularCasoDeUso = new EliminarGrupoMuscularCasoDeUso(_grupoMuscularRepositorioMock.Object);
    }

    [Fact]
    public void EliminarGrupoMuscularCasoDeUso_EliminacionExitosa_RetornaTrue()
    {
        // Arrange
        int grupoMuscularId = 1;
        _grupoMuscularRepositorioMock
            .Setup(repo => repo.EliminarAsync(grupoMuscularId))
            .ReturnsAsync(true);
        // Act
        var resultado = _eliminarGrupoMuscularCasoDeUso.Ejecutar(grupoMuscularId).Result;
        // Assert con FluentAssertions
        resultado.Should().BeTrue();
    }

    [Fact]
    public void EliminarGrupoMuscularCasoDeUso_EliminacionFalla_RetornaFalse()
    {
        // Arrange
        int grupoMuscularId = 1;
        _grupoMuscularRepositorioMock
            .Setup(repo => repo.EliminarAsync(grupoMuscularId))
            .ReturnsAsync(false);
        // Act
        var resultado = _eliminarGrupoMuscularCasoDeUso.Ejecutar(grupoMuscularId).Result;
        // Assert con FluentAssertions
        resultado.Should().BeFalse();
    }

    
}
