using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.DTOs.SerieDTOs;
using FitRank_API.Application.CasosDeUso.SerieCasosDeUso;
using FitRank_API.Application.MappingProfiles;

namespace FitRank_API.ApplicationTests.CasosDeUsoTests.SerieCasosDeUsoTests;

public class ObtenerSeriePorIdCasoDeUsoTest
{
    private readonly Mock<ISerieRepositorio> _serieRepositoryMock;
    private readonly IMapper _mapper;
    private readonly ObtenerSeriePorIdCasoDeUso _obtenerSeriePorIdCasoDeUso;

    public ObtenerSeriePorIdCasoDeUsoTest()
    {
        _serieRepositoryMock = new Mock<ISerieRepositorio>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SerieProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _obtenerSeriePorIdCasoDeUso = new ObtenerSeriePorIdCasoDeUso(_serieRepositoryMock.Object, _mapper);
    }

    //obtener serie por id exitoso
    [Fact]
    public async Task ObtenerSeriePorId_CuandoElIdExiste_RetornaSerieDTO()
    {
        // Arrange
        var serieId = 1;

        var serieExistente = new Serie
        {
            Id = serieId,
            NumeroDeSerie = 3,
            EjercicioAsignadoId = 1
        };

        _serieRepositoryMock
            .Setup(repo => repo.ObtenerPorIdAsync(serieId))
            .ReturnsAsync(serieExistente);

        // Act
        var resultado = await _obtenerSeriePorIdCasoDeUso.Ejecutar(serieId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(serieExistente.Id);
        resultado.NumeroDeSerie.Should().Be(serieExistente.NumeroDeSerie);
        resultado.EjercicioAsignadoId.Should().Be(serieExistente.EjercicioAsignadoId);
    }

    //obtener serie por id no existente
    [Fact]
    public async Task ObtenerSeriePorId_CuandoElIdNoExiste_RetornaNull()
    {
        // Arrange
        var serieId = 99;

        _serieRepositoryMock
            .Setup(repo => repo.ObtenerPorIdAsync(serieId))
            .ReturnsAsync((Serie?)null);

        // Act
        var resultado = await _obtenerSeriePorIdCasoDeUso.Ejecutar(serieId);

        // Assert
        resultado.Should().BeNull();
    }
    
}