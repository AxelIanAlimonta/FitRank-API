using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.DTOs.SerieDTOs;
using FitRank_API.Application.CasosDeUso.SerieCasosDeUso;
using FitRank_API.Application.Mappings;

namespace CasosDeUsoTests.SerieCasosDeUsoTests;

public class AgregarSerieCasoDeUsoTest
{
    private readonly Mock<ISerieRepositorio> _serieRepositoryMock;
    private readonly IMapper _mapper;
    private readonly AgregarSerieCasoDeUso _agregarSerieCasoDeUso;

    public AgregarSerieCasoDeUsoTest()
    {
        _serieRepositoryMock = new Mock<ISerieRepositorio>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SerieProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _agregarSerieCasoDeUso = new AgregarSerieCasoDeUso(_serieRepositoryMock.Object, _mapper);
    }

    [Fact]
    public async Task AgregarSerie_CuandoLosDatosSonValidos_RetornaSerieDTO()
    {
        // Arrange
        var nuevaSerieDTO = new AgregarSerieDTO
        {
            EjercicioAsignadoId = 1,
            NumeroDeSerie = 3,
        };

        var serieCreada = new Serie
        {
            Id = 1,
            NumeroDeSerie = nuevaSerieDTO.NumeroDeSerie,
            EjercicioAsignadoId = nuevaSerieDTO.EjercicioAsignadoId
        };

        _serieRepositoryMock
            .Setup(repo => repo.AgregarAsync(It.IsAny<Serie>()))
            .ReturnsAsync(serieCreada);

        // Act
        var resultado = await _agregarSerieCasoDeUso.Ejecutar(nuevaSerieDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(serieCreada.Id);
        resultado.NumeroDeSerie.Should().Be(nuevaSerieDTO.NumeroDeSerie);
        resultado.EjercicioAsignadoId.Should().Be(nuevaSerieDTO.EjercicioAsignadoId);
    }
}



