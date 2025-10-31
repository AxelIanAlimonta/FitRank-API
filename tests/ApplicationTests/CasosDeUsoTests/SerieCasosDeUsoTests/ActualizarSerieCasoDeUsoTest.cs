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

public class ActualizarSerieCasoDeUsoTest
{
    private readonly Mock<ISerieRepositorio> _serieRepositoryMock;
    private readonly IMapper _mapper;
    private readonly ActualizarSerieCasoDeUso _actualizarSerieCasoDeUso;

    public ActualizarSerieCasoDeUsoTest()
    {
        _serieRepositoryMock = new Mock<ISerieRepositorio>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SerieProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _actualizarSerieCasoDeUso = new ActualizarSerieCasoDeUso(_serieRepositoryMock.Object, _mapper);
    }

    //actualizar serie exitoso
    [Fact]
    public async Task ActualizarSerie_CuandoLosDatosSonValidos_RetornaSerieDTO()
    {
        // Arrange
        var actualizarSerieDTO = new ActualizarSerieDTO
        {
            Id = 1,
            NumeroDeSerie = 4,
            EjercicioAsignadoId = 1
        };

        var serieExistente = new Serie
        {
            Id = actualizarSerieDTO.Id,
            NumeroDeSerie = 3,
            EjercicioAsignadoId = 1
        };

        var serieActualizada = new Serie
        {
            Id = actualizarSerieDTO.Id,
            NumeroDeSerie = actualizarSerieDTO.NumeroDeSerie,
            EjercicioAsignadoId = actualizarSerieDTO.EjercicioAsignadoId
        };

        _serieRepositoryMock
            .Setup(repo => repo.ObtenerPorIdAsync(actualizarSerieDTO.Id))
            .ReturnsAsync(serieExistente);

        _serieRepositoryMock
            .Setup(repo => repo.ActualizarAsync(It.IsAny<Serie>()))
            .ReturnsAsync(serieActualizada);

        // Act
        var resultado = await _actualizarSerieCasoDeUso.Ejecutar(actualizarSerieDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(serieActualizada.Id);
        resultado.NumeroDeSerie.Should().Be(actualizarSerieDTO.NumeroDeSerie);
        resultado.EjercicioAsignadoId.Should().Be(actualizarSerieDTO.EjercicioAsignadoId);
    }

    //actualizar serie no existente
    [Fact]
    public async Task ActualizarSerie_CuandoLaSerieNoExiste_RetornaNull()
    {
        // Arrange
        var actualizarSerieDTO = new ActualizarSerieDTO
        {
            Id = 99,
            NumeroDeSerie = 4,
            EjercicioAsignadoId = 1
        };

        _serieRepositoryMock
            .Setup(repo => repo.ObtenerPorIdAsync(actualizarSerieDTO.Id))
            .ReturnsAsync((Serie?)null);

        // Act
        var resultado = await _actualizarSerieCasoDeUso.Ejecutar(actualizarSerieDTO);

        // Assert
        resultado.Should().BeNull();
    }
}