using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.MappingProfiles;
using FitRank_API.Application.DTOs.DiaDeLaSemanaDTOs;
using FitRank_API.Application.CasosDeUso.DiaDeLaSemanaCasoDeUso;
using FitRank_API.Application.Mappings;

namespace CasosDeUsoTests.DiaDeLaSemanaCasoDeUsoTests;

public class ObtenerTodosLosDiasDeLaSemanaCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IDiaDeLaSemanaRepositorio> _diaDeLaSemanaRepositorioMock;

    public ObtenerTodosLosDiasDeLaSemanaCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new DiaDeLaSemanaProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _diaDeLaSemanaRepositorioMock = new Mock<IDiaDeLaSemanaRepositorio>();
    }

    [Fact]
    public async Task ObtenerTodosLosDiasDeLaSemana_RetornaListaDeDiaDeLaSemanaDTO()
    {
        // Arrange
        var diasExistentes = new List<DiaDeLaSemana>
        {
            new DiaDeLaSemana { Id = 1, Nombre = "Lunes" },
            new DiaDeLaSemana { Id = 2, Nombre = "Martes" }
        };

        _diaDeLaSemanaRepositorioMock
            .Setup(repo => repo.ObtenerTodosLosDiasDeLaSemanaAsync())
            .ReturnsAsync(diasExistentes);

        var obtenerTodosLosDiasDeLaSemanaCasoDeUso = new ObtenerTodosLosDiasDeLaSemanaCasoDeUso(_diaDeLaSemanaRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerTodosLosDiasDeLaSemanaCasoDeUso.Ejecutar();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Count().Should().Be(diasExistentes.Count);
    }

    //obtener lista
    [Fact]
    public async Task ObtenerTodosLosDiasDeLaSemana_CuandoNoExistenDias_RetornaListaVacia()
    {
        // Arrange
        var diasExistentes = new List<DiaDeLaSemana>();

        _diaDeLaSemanaRepositorioMock
            .Setup(repo => repo.ObtenerTodosLosDiasDeLaSemanaAsync())
            .ReturnsAsync(diasExistentes);

        var obtenerTodosLosDiasDeLaSemanaCasoDeUso = new ObtenerTodosLosDiasDeLaSemanaCasoDeUso(_diaDeLaSemanaRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerTodosLosDiasDeLaSemanaCasoDeUso.Ejecutar();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Count().Should().Be(0);
    }
}