using FitRank_API.Domain.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.DTOs.SerieDTOs;
using FitRank_API.Application.CasosDeUso.SerieCasosDeUso;
using FitRank_API.Application.Mappings;

namespace CasosDeUsoTests.SerieCasosDeUsoTests;

public class ObtenerSeriesCasoDeUsoTest
{
    private readonly Mock<ISerieRepositorio> _serieRepositoryMock;
    private readonly IMapper _mapper;
    private readonly ObtenerSeriesCasoDeUso _obtenerSeriesCasoDeUso;

    public ObtenerSeriesCasoDeUsoTest()
    {
        _serieRepositoryMock = new Mock<ISerieRepositorio>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SerieProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _obtenerSeriesCasoDeUso = new ObtenerSeriesCasoDeUso(_serieRepositoryMock.Object, _mapper);
    }

    //obtener lista de series exitoso
    [Fact]
    public async Task ObtenerSeries_CuandoExistenSeries_RetornaListaDeSerieDTO()
    {
        // Arrange
        var seriesExistentes = new List<Serie>
        {
            new Serie { Id = 1, NumeroDeSerie = 3, EjercicioAsignadoId = 1 },
            new Serie { Id = 2, NumeroDeSerie = 4, EjercicioAsignadoId = 1 }
        };

        _serieRepositoryMock
            .Setup(repo => repo.ObtenerTodasAsync())
            .ReturnsAsync(seriesExistentes);

        // Act
        var resultado = await _obtenerSeriesCasoDeUso.Ejecutar();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Count().Should().Be(seriesExistentes.Count);

        for (int i = 0; i < seriesExistentes.Count; i++)
        {
            resultado.ElementAt(i).Id.Should().Be(seriesExistentes[i].Id);
            resultado.ElementAt(i).NumeroDeSerie.Should().Be(seriesExistentes[i].NumeroDeSerie);
            resultado.ElementAt(i).EjercicioAsignadoId.Should().Be(seriesExistentes[i].EjercicioAsignadoId);
        }
    }

    //obtener lista de series vacia
    [Fact]
    public async Task ObtenerSeries_CuandoNoExistenSeries_RetornaListaVacia()
    {
        // Arrange
        var seriesExistentes = new List<Serie>();

        _serieRepositoryMock
            .Setup(repo => repo.ObtenerTodasAsync())
            .ReturnsAsync(seriesExistentes);

        // Act
        var resultado = await _obtenerSeriesCasoDeUso.Ejecutar();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Count().Should().Be(0);
    }
    
}