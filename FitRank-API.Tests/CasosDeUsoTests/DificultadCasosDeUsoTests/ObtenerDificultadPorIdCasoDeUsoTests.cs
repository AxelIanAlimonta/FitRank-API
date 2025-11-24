using FitRank_API.Application.Mappings;
using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.DificultadCasosDeUso;
using FitRank_API.Application.DTOs.DificultadDTOs;
using FitRank_API.Domain.Entities;

namespace CasosDeUsoTests.DificultadCasosDeUsoTests;

public class ObtenerDificultadPorIdCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IDificultadRepositorio> _dificultadRepositorioMock;

    public ObtenerDificultadPorIdCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new DificultadProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _dificultadRepositorioMock = new Mock<IDificultadRepositorio>();
    }

    [Fact]
    public async Task Ejecutar_DeberiaObtenerDificultad_CuandoLaDificultadExiste()
    {
        // Arrange
        var dificultadId = 1;
        var dificultadExistente = new Dificultad
        {
            Id = dificultadId,
            Descripcion = "Intermedio"
        };

        _dificultadRepositorioMock.Setup(repo => repo.ObtenerPorIdAsync(dificultadId))
            .ReturnsAsync(dificultadExistente);

        var obtenerDificultadPorIdCasoDeUso = new ObtenerDificultadPorIdCasoDeUso(_dificultadRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerDificultadPorIdCasoDeUso.Ejecutar(dificultadId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be((int)dificultadExistente.Id);
        resultado.Descripcion.Should().Be(dificultadExistente.Descripcion);
    }

    [Fact]
    public async Task Ejecutar_DeberiaRetornarNull_CuandoLaDificultadNoExiste()
    {
        // Arrange
        var dificultadId = 999;

        _dificultadRepositorioMock.Setup(repo => repo.ObtenerPorIdAsync(dificultadId))
            .ReturnsAsync((Dificultad?)null);

        var obtenerDificultadPorIdCasoDeUso = new ObtenerDificultadPorIdCasoDeUso(_dificultadRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerDificultadPorIdCasoDeUso.Ejecutar(dificultadId);

        // Assert
        resultado.Should().BeNull();
    }
}
