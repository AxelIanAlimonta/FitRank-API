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

public class ObtenerGrupoMuscularPorIdCasoDeUsoTests
{
    private readonly Mock<IGrupoMuscularRepositorio> _grupoMuscularRepositorioMock;
    private readonly IMapper _mapper;
    private readonly ObtenerGrupoMuscularPorIdCasoDeUso _obtenerGrupoMuscularPorIdCasoDeUso;
    public ObtenerGrupoMuscularPorIdCasoDeUsoTests()
    {
        _grupoMuscularRepositorioMock = new Mock<IGrupoMuscularRepositorio>();
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GrupoMuscularProfile>();
        });
        _mapper = mapperConfig.CreateMapper();
        _obtenerGrupoMuscularPorIdCasoDeUso = new ObtenerGrupoMuscularPorIdCasoDeUso(_grupoMuscularRepositorioMock.Object, _mapper);
    }

    [Fact]
    public void ObtenerGrupoMuscularPorIdCasoDeUso_GrupoMuscularExiste_RetornaGrupoMuscularDTO()
    {
        // Arrange
        int grupoMuscularId = 1;
        var grupoMuscularEntidad = new GrupoMuscular
        {
            Id = grupoMuscularId,
            Nombre = "Piernas",
            Imagen = "imagen_url"
        };
        _grupoMuscularRepositorioMock
            .Setup(repo => repo.ObtenerPorIdAsync(grupoMuscularId))
            .ReturnsAsync(grupoMuscularEntidad);
        // Act
        var resultado = _obtenerGrupoMuscularPorIdCasoDeUso.Ejecutar(grupoMuscularId).Result;
        // Assert con FluentAssertions
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(grupoMuscularEntidad.Id);
        resultado.Nombre.Should().Be(grupoMuscularEntidad.Nombre);
        resultado.Imagen.Should().Be(grupoMuscularEntidad.Imagen);
    }

    [Fact]
    public void ObtenerGrupoMuscularPorIdCasoDeUso_GrupoMuscularNoExiste_RetornaNull()
    {
        // Arrange
        int grupoMuscularId = 1;
        _grupoMuscularRepositorioMock
            .Setup(repo => repo.ObtenerPorIdAsync(grupoMuscularId))
            .ReturnsAsync((GrupoMuscular?)null);
        // Act
        var resultado = _obtenerGrupoMuscularPorIdCasoDeUso.Ejecutar(grupoMuscularId).Result;
        // Assert con FluentAssertions
        resultado.Should().BeNull();
    }

    [Fact]
    public void ObtenerGrupoMuscularPorIdCasoDeUso_RepositorioLanzaExcepcion_LanzaExcepcion()
    {
        // Arrange
        int grupoMuscularId = 1;
        _grupoMuscularRepositorioMock
            .Setup(repo => repo.ObtenerPorIdAsync(grupoMuscularId))
            .ThrowsAsync(new Exception("Error de base de datos"));
        // Act & FluetnAssert
        Action act = () => _obtenerGrupoMuscularPorIdCasoDeUso.Ejecutar(grupoMuscularId).Wait();
        act.Should().Throw<AggregateException>()
            .WithInnerException<Exception>()
            .WithMessage("Error de base de datos");
    }
}
