using FitRank_API.Domain.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.Mappings;
using FitRank_API.Application.DTOs.DiaDeLaSemanaDTOs;
using FitRank_API.Application.Mappings;
using FitRank_API.Application.CasosDeUso.DiaDeLaSemanaCasoDeUso;

namespace CasosDeUsoTests.DiaDeLaSemanaCasoDeUsoTests;

public class ObtenerDiaDeLaSemanaPorIdCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IDiaDeLaSemanaRepositorio> _diaDeLaSemanaRepositorioMock;

    public ObtenerDiaDeLaSemanaPorIdCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new DiaDeLaSemanaProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _diaDeLaSemanaRepositorioMock = new Mock<IDiaDeLaSemanaRepositorio>();
    }

    [Fact]
    public async Task ObtenerDiaDeLaSemanaPorId_CuandoElIdExiste_RetornaDiaDeLaSemanaDTO()
    {
        // Arrange
        var diaId = 1;

        var diaExistente = new DiaDeLaSemana
        {
            Id = diaId,
            Nombre = "Lunes"
        };

        _diaDeLaSemanaRepositorioMock
            .Setup(repo => repo.ObtenerDiaDeLaSemanaPorIdAsync(diaId))
            .ReturnsAsync(diaExistente);

        var obtenerDiaDeLaSemanaPorIdCasoDeUso = new ObtenerDiaDeLaSemanaPorIdCasoDeUso(_diaDeLaSemanaRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerDiaDeLaSemanaPorIdCasoDeUso.Ejecutar(diaId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(diaExistente.Id);
        resultado.Nombre.Should().Be(diaExistente.Nombre);
    }

    [Fact]
    public async Task ObtenerDiaDeLaSemanaPorId_CuandoElIdNoExiste_RetornaNulo()
    {
        // Arrange
        var diaId = 99; // Suponiendo que este ID no existe

        _diaDeLaSemanaRepositorioMock
            .Setup(repo => repo.ObtenerDiaDeLaSemanaPorIdAsync(diaId))
            .ReturnsAsync((DiaDeLaSemana?)null);

        var obtenerDiaDeLaSemanaPorIdCasoDeUso = new ObtenerDiaDeLaSemanaPorIdCasoDeUso(_diaDeLaSemanaRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerDiaDeLaSemanaPorIdCasoDeUso.Ejecutar(diaId);

        // Assert
        resultado.Should().BeNull();
    }
}