using AutoMapper;
using FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace CasosDeUsoTests.RutinaCasosDeUsoTests;

public class EliminarRutinaCasoDeUsoTests
{
    private readonly Mock<IRutinaRepositorio> _rutinaRepositorioMock;
    private readonly EliminarRutinaCasoDeUso _eliminarRutinaCasoDeUso;

    public EliminarRutinaCasoDeUsoTests()
    {
        _rutinaRepositorioMock = new Mock<IRutinaRepositorio>();
        _eliminarRutinaCasoDeUso = new EliminarRutinaCasoDeUso(_rutinaRepositorioMock.Object);
    }

    //eliminar rutina elimina exitosamente
    [Fact]
    public async Task EliminarRutinaCasoDeUso_EliminacionExitosa_RetornaTrue()
    {
        // Arrange
        var rutinaId = 1;
        _rutinaRepositorioMock
            .Setup(repo => repo.EliminarAsync(rutinaId))
            .ReturnsAsync(true);
        // Act
        var resultado = await _eliminarRutinaCasoDeUso.Ejecutar(rutinaId);
        // FluentAssertions
        resultado.Should().BeTrue();
    }

    //eliminar rutina con id no existente devuelve false
    [Fact]
    public async Task EliminarRutinaCasoDeUso_IdNoExistente_RetornaFalse()
    {
        // Arrange
        var rutinaId = 999; // ID no existente
        _rutinaRepositorioMock
            .Setup(repo => repo.EliminarAsync(rutinaId))
            .ReturnsAsync(false);
        // Act
        var resultado = await _eliminarRutinaCasoDeUso.Ejecutar(rutinaId);
        // FluentAssertions
        resultado.Should().BeFalse();
    }
}