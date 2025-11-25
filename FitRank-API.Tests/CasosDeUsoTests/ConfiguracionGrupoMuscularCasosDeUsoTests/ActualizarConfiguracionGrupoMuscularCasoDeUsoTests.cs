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

    [Fact]
    public async Task DebeMapearCorrectamenteTodosLosCampos()
    {
        // Arrange
        var dto = new ConfiguracionGrupoMuscularDTO
        {
            Id = 10,
            GrupoMuscularId = 5,
            MultiplicadorPeso = 0.75,
            MultiplicadorRepeticiones = 0.25,
            FactorProgresion = 1.5
        };

        var configuracionActualizada = new ConfiguracionGrupoMuscular
        {
            Id = 10,
            GrupoMuscularId = 5,
            MultiplicadorPeso = 0.75,
            MultiplicadorRepeticiones = 0.25,
            FactorProgresion = 1.5
        };

        _configuracionRepositorioMock.Setup(repo => repo.ActualizarAsync(It.IsAny<ConfiguracionGrupoMuscular>()))
            .ReturnsAsync(configuracionActualizada);

        var casoDeUso = new ActualizarConfiguracionGrupoMuscularCasoDeUso(_configuracionRepositorioMock.Object, _mapper);

        // Act
        var resultado = await casoDeUso.Ejecutar(dto);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(10);
        resultado.GrupoMuscularId.Should().Be(5);
        resultado.MultiplicadorPeso.Should().Be(0.75);
        resultado.MultiplicadorRepeticiones.Should().Be(0.25);
        resultado.FactorProgresion.Should().Be(1.5);
    }

    [Fact]
    public async Task DebeLlamarRepositorioConDatosCorrectos()
    {
        // Arrange
        var dto = new ConfiguracionGrupoMuscularDTO
        {
            Id = 3,
            GrupoMuscularId = 7,
            MultiplicadorPeso = 0.8,
            MultiplicadorRepeticiones = 0.2,
            FactorProgresion = 1.4
        };

        _configuracionRepositorioMock.Setup(repo => repo.ActualizarAsync(It.IsAny<ConfiguracionGrupoMuscular>()))
            .ReturnsAsync((ConfiguracionGrupoMuscular c) => c);

        var casoDeUso = new ActualizarConfiguracionGrupoMuscularCasoDeUso(_configuracionRepositorioMock.Object, _mapper);

        // Act
        await casoDeUso.Ejecutar(dto);

        // Assert
        _configuracionRepositorioMock.Verify(repo => repo.ActualizarAsync(
            It.Is<ConfiguracionGrupoMuscular>(c => c.Id == dto.Id && 
                                                   c.GrupoMuscularId == dto.GrupoMuscularId && 
                                                   c.MultiplicadorPeso == dto.MultiplicadorPeso &&
                                                   c.MultiplicadorRepeticiones == dto.MultiplicadorRepeticiones &&
                                                   c.FactorProgresion == dto.FactorProgresion)), 
            Times.Once);
    }

    [Fact]
    public async Task DeberiaActualizarConfiguracionesConDiferentesMultiplicadores()
    {
        // Arrange
        var dto1 = new ConfiguracionGrupoMuscularDTO { Id = 1, GrupoMuscularId = 1, MultiplicadorPeso = 0.1, MultiplicadorRepeticiones = 0.9, FactorProgresion = 1.0 };
        var dto2 = new ConfiguracionGrupoMuscularDTO { Id = 2, GrupoMuscularId = 2, MultiplicadorPeso = 0.9, MultiplicadorRepeticiones = 0.1, FactorProgresion = 2.0 };

        _configuracionRepositorioMock.Setup(repo => repo.ActualizarAsync(It.Is<ConfiguracionGrupoMuscular>(c => c.Id == 1)))
            .ReturnsAsync((ConfiguracionGrupoMuscular c) => c);

        _configuracionRepositorioMock.Setup(repo => repo.ActualizarAsync(It.Is<ConfiguracionGrupoMuscular>(c => c.Id == 2)))
            .ReturnsAsync((ConfiguracionGrupoMuscular c) => c);

        var casoDeUso = new ActualizarConfiguracionGrupoMuscularCasoDeUso(_configuracionRepositorioMock.Object, _mapper);

        // Act
        var resultado1 = await casoDeUso.Ejecutar(dto1);
        var resultado2 = await casoDeUso.Ejecutar(dto2);

        // Assert
        resultado1!.MultiplicadorPeso.Should().Be(0.1);
        resultado2!.MultiplicadorPeso.Should().Be(0.9);
    }

    [Fact]
    public async Task DeberiaRetornarTipoConfiguracionGrupoMuscularDTO()
    {
        // Arrange
        var dto = new ConfiguracionGrupoMuscularDTO
        {
            Id = 1,
            GrupoMuscularId = 1,
            MultiplicadorPeso = 0.5,
            MultiplicadorRepeticiones = 0.5,
            FactorProgresion = 1.1
        };

        _configuracionRepositorioMock.Setup(repo => repo.ActualizarAsync(It.IsAny<ConfiguracionGrupoMuscular>()))
            .ReturnsAsync((ConfiguracionGrupoMuscular c) => c);

        var casoDeUso = new ActualizarConfiguracionGrupoMuscularCasoDeUso(_configuracionRepositorioMock.Object, _mapper);

        // Act
        var resultado = await casoDeUso.Ejecutar(dto);

        // Assert
        resultado.Should().BeOfType<ConfiguracionGrupoMuscularDTO>();
    }
}
