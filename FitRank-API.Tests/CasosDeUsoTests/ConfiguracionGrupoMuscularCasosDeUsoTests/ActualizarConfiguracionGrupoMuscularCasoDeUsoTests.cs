using FitRank_API.Application.Mappings;
using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.ConfiguracionGrupoMuscular;
using FitRank_API.Application.DTOs.ConfiguracionGrupoMuscularDTOs;
using FitRank_API.Domain.Entities;

namespace CasosDeUsoTests.ConfiguracionGrupoMuscularCasosDeUsoTests;

public class ActualizarConfiguracionGrupoMuscularCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IConfiguracionGrupoMuscularRepositorio> _configuracionRepositorioMock;

    public ActualizarConfiguracionGrupoMuscularCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new ConfiguracionGrupoMuscularProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _configuracionRepositorioMock = new Mock<IConfiguracionGrupoMuscularRepositorio>();
    }

    [Fact]
    public async Task Ejecutar_DeberiaActualizarConfiguracion_CuandoLosDatosSonValidos()
    {
        // Arrange
        var configuracionActualizadaDTO = new ConfiguracionGrupoMuscularDTO
        {
            Id = 1,
            GrupoMuscularId = 2,
            MultiplicadorPeso = 0.6,
            MultiplicadorRepeticiones = 0.4,
            FactorProgresion = 1.2
        };

        var configuracionActualizada = new ConfiguracionGrupoMuscular
        {
            Id = 1,
            GrupoMuscularId = 2,
            MultiplicadorPeso = 0.6,
            MultiplicadorRepeticiones = 0.4,
            FactorProgresion = 1.2
        };

        _configuracionRepositorioMock.Setup(repo => repo.ActualizarAsync(It.IsAny<ConfiguracionGrupoMuscular>()))
            .ReturnsAsync(configuracionActualizada);

        var actualizarConfiguracionCasoDeUso = new ActualizarConfiguracionGrupoMuscularCasoDeUso(_configuracionRepositorioMock.Object, _mapper);

        // Act
        var resultado = await actualizarConfiguracionCasoDeUso.Ejecutar(configuracionActualizadaDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(1);
        resultado.GrupoMuscularId.Should().Be(2);
        resultado.MultiplicadorPeso.Should().Be(0.6);
        resultado.MultiplicadorRepeticiones.Should().Be(0.4);
        resultado.FactorProgresion.Should().Be(1.2);
    }

    [Fact]
    public async Task Ejecutar_DeberiaRetornarNull_CuandoLaConfiguracionNoExiste()
    {
        // Arrange
        var configuracionActualizadaDTO = new ConfiguracionGrupoMuscularDTO
        {
            Id = 99,
            GrupoMuscularId = 2,
            MultiplicadorPeso = 0.6,
            MultiplicadorRepeticiones = 0.4,
            FactorProgresion = 1.2
        };

        _configuracionRepositorioMock.Setup(repo => repo.ActualizarAsync(It.IsAny<ConfiguracionGrupoMuscular>()))
            .ReturnsAsync((ConfiguracionGrupoMuscular?)null);

        var actualizarConfiguracionCasoDeUso = new ActualizarConfiguracionGrupoMuscularCasoDeUso(_configuracionRepositorioMock.Object, _mapper);

        // Act
        var resultado = await actualizarConfiguracionCasoDeUso.Ejecutar(configuracionActualizadaDTO);

        // Assert
        resultado.Should().BeNull();
    }
}
