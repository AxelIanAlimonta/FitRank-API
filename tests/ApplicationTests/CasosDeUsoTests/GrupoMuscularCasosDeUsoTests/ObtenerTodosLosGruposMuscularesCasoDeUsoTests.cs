using FitRank_API.Application.CasosDeUso.GrupoMuscularCasosDeUso;
using FitRank_API.Application.Mappings;
using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Application.DTOs;
using FitRank_API.Domain.Entities;
using FluentAssertions;

namespace FitRank_API.tests.ApplicationTests.CasosDeUsoTests.GrupoMuscularCasosDeUsoTests;

public class ObtenerTodosLosGruposMuscularesCasoDeUsoTests
{
    private readonly Mock<IGrupoMuscularRepositorio> _grupoMuscularRepositorioMock;
    private readonly IMapper _mapper;
    private readonly ObtenerTodosLosGruposMuscularesCasoDeUso _obtenerTodosLosGruposMuscularesCasoDeUso;
    public ObtenerTodosLosGruposMuscularesCasoDeUsoTests()
    {
        _grupoMuscularRepositorioMock = new Mock<IGrupoMuscularRepositorio>();
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GrupoMuscularProfile>();
        });
        _mapper = mapperConfig.CreateMapper();
        _obtenerTodosLosGruposMuscularesCasoDeUso = new ObtenerTodosLosGruposMuscularesCasoDeUso(_grupoMuscularRepositorioMock.Object, _mapper);
    }

    [Fact]
    public void ObtenerTodosLosGruposMuscularesCasoDeUso_GruposMuscularesExisten_RetornaListaDeGrupoMuscularDTO()
    {
        // Arrange
        var gruposMuscularesEntidad = new List<GrupoMuscular>
        {
            new GrupoMuscular { Id = 1, Nombre = "Brazos", Imagen = "imagen_brazos_url" },
            new GrupoMuscular { Id = 2, Nombre = "Piernas", Imagen = "imagen_piernas_url" }
        };
        _grupoMuscularRepositorioMock
            .Setup(repo => repo.ObtenerTodosAsync())
            .ReturnsAsync(gruposMuscularesEntidad);
        // Act
        var resultado = _obtenerTodosLosGruposMuscularesCasoDeUso.Ejecutar().Result;
        // Assert con FluentAssertions
        resultado.Should().NotBeNull();
        resultado.Count.Should().Be(gruposMuscularesEntidad.Count);
        for (int i = 0; i < resultado.Count; i++)
        {
            resultado[i].Id.Should().Be(gruposMuscularesEntidad[i].Id);
            resultado[i].Nombre.Should().Be(gruposMuscularesEntidad[i].Nombre);
            resultado[i].Imagen.Should().Be(gruposMuscularesEntidad[i].Imagen);
        }
    }

    [Fact]
    public void ObtenerTodosLosGruposMuscularesCasoDeUso_NoHayGruposMusculares_RetornaListaVacia()
    {
        // Arrange
        var gruposMuscularesEntidad = new List<GrupoMuscular>();
        _grupoMuscularRepositorioMock
            .Setup(repo => repo.ObtenerTodosAsync())
            .ReturnsAsync(gruposMuscularesEntidad);
        // Act
        var resultado = _obtenerTodosLosGruposMuscularesCasoDeUso.Ejecutar().Result;
        // Assert con FluentAssertions
        resultado.Should().NotBeNull();
        resultado.Count.Should().Be(0);
    }

}
