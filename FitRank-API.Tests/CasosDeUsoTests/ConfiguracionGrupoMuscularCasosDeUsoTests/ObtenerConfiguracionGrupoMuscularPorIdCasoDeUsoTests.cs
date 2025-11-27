using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.ConfiguracionGrupoMuscular;
using FitRank_API.Application.DTOs.ConfiguracionGrupoMuscularDTOs;
using FitRank_API.Domain.Entities;

namespace CasosDeUsoTests.ConfiguracionGrupoMuscularCasosDeUsoTests;

public class ObtenerConfiguracionGrupoMuscularPorIdCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IConfiguracionGrupoMuscularRepositorio> _configuracionRepositorioMock;

    public ObtenerConfiguracionGrupoMuscularPorIdCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new ConfiguracionGrupoMuscularProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _configuracionRepositorioMock = new Mock<IConfiguracionGrupoMuscularRepositorio>();
    }

    [Fact]
    public async Task Ejecutar_DeberiaObtenerConfiguracion_CuandoLaConfiguracionExiste()
    {
        // Arrange
        var configuracionId = 1L;
        var configuracionExistente = new ConfiguracionGrupoMuscular
        {
            Id = configuracionId,
            GrupoMuscularId = 1,
            MultiplicadorPeso = 0.5,
            MultiplicadorRepeticiones = 0.3,
            FactorProgresion = 1.1
        };

        _configuracionRepositorioMock.Setup(repo => repo.ObtenerPorIdAsync(configuracionId))
            .ReturnsAsync(configuracionExistente);

        var obtenerConfiguracionPorIdCasoDeUso = new ObtenerConfiguracionGrupoMuscularPorIdCasoDeUso(_configuracionRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerConfiguracionPorIdCasoDeUso.Ejecutar(configuracionId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(configuracionExistente.Id);
        resultado.GrupoMuscularId.Should().Be((int)configuracionExistente.GrupoMuscularId);
        resultado.MultiplicadorPeso.Should().Be(configuracionExistente.MultiplicadorPeso);
        resultado.MultiplicadorRepeticiones.Should().Be(configuracionExistente.MultiplicadorRepeticiones);
        resultado.FactorProgresion.Should().Be(configuracionExistente.FactorProgresion);
    }

    [Fact]
    public async Task Ejecutar_DeberiaRetornarNull_CuandoLaConfiguracionNoExiste()
    {
        // Arrange
        var configuracionId = 999L;

        _configuracionRepositorioMock.Setup(repo => repo.ObtenerPorIdAsync(configuracionId))
            .ReturnsAsync((ConfiguracionGrupoMuscular?)null);

        var obtenerConfiguracionPorIdCasoDeUso = new ObtenerConfiguracionGrupoMuscularPorIdCasoDeUso(_configuracionRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerConfiguracionPorIdCasoDeUso.Ejecutar(configuracionId);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task DebeLlamarRepositorioConIdCorrecto()
    {
        // Arrange
        var configuracionId = 555L;

        _configuracionRepositorioMock.Setup(repo => repo.ObtenerPorIdAsync(configuracionId))
            .ReturnsAsync((ConfiguracionGrupoMuscular?)null);

        var casoDeUso = new ObtenerConfiguracionGrupoMuscularPorIdCasoDeUso(_configuracionRepositorioMock.Object, _mapper);

        // Act
        await casoDeUso.Ejecutar(configuracionId);

        // Assert
        _configuracionRepositorioMock.Verify(repo => repo.ObtenerPorIdAsync(configuracionId), Times.Once);
    }

    [Fact]
    public async Task DeberiaRetornarConfiguracionesConDiferentesValores()
    {
        // Arrange
        var config1 = new ConfiguracionGrupoMuscular { Id = 1, GrupoMuscularId = 1, MultiplicadorPeso = 0.2, MultiplicadorRepeticiones = 0.8, FactorProgresion = 1.0 };
        var config2 = new ConfiguracionGrupoMuscular { Id = 2, GrupoMuscularId = 2, MultiplicadorPeso = 0.9, MultiplicadorRepeticiones = 0.1, FactorProgresion = 3.0 };

        _configuracionRepositorioMock.Setup(repo => repo.ObtenerPorIdAsync(1))
            .ReturnsAsync(config1);

        _configuracionRepositorioMock.Setup(repo => repo.ObtenerPorIdAsync(2))
            .ReturnsAsync(config2);

        var casoDeUso = new ObtenerConfiguracionGrupoMuscularPorIdCasoDeUso(_configuracionRepositorioMock.Object, _mapper);

        // Act
        var resultado1 = await casoDeUso.Ejecutar(1);
        var resultado2 = await casoDeUso.Ejecutar(2);

        // Assert
        resultado1!.MultiplicadorPeso.Should().Be(0.2);
        resultado2!.MultiplicadorPeso.Should().Be(0.9);
    }

    [Fact]
    public async Task DeberiaRetornarTipoConfiguracionGrupoMuscularDTO()
    {
        // Arrange
        var configuracionId = 1L;
        var configuracion = new ConfiguracionGrupoMuscular
        {
            Id = 1,
            GrupoMuscularId = 1,
            MultiplicadorPeso = 0.5,
            MultiplicadorRepeticiones = 0.5,
            FactorProgresion = 1.0
        };

        _configuracionRepositorioMock.Setup(repo => repo.ObtenerPorIdAsync(configuracionId))
            .ReturnsAsync(configuracion);

        var casoDeUso = new ObtenerConfiguracionGrupoMuscularPorIdCasoDeUso(_configuracionRepositorioMock.Object, _mapper);

        // Act
        var resultado = await casoDeUso.Ejecutar(configuracionId);

        // Assert
        resultado.Should().BeOfType<ConfiguracionGrupoMuscularDTO>();
    }
}
