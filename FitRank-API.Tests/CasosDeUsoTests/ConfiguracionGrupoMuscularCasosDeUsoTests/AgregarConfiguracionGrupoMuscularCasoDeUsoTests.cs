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

public class AgregarConfiguracionGrupoMuscularCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IConfiguracionGrupoMuscularRepositorio> _configuracionRepositorioMock;

    public AgregarConfiguracionGrupoMuscularCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new ConfiguracionGrupoMuscularProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _configuracionRepositorioMock = new Mock<IConfiguracionGrupoMuscularRepositorio>();
    }

    [Fact]
    public async Task Ejecutar_DeberiaAgregarConfiguracion_CuandoLosDatosSonValidos()
    {
        // Arrange
        var nuevaConfiguracionDTO = new AgregarConfiguracionGrupoMuscularDTO
        {
            GrupoMuscularId = 1,
            MultiplicadorPeso = 0.5,
            MultiplicadorRepeticiones = 0.3,
            FactorProgresion = 1.1
        };

        var configuracionAgregada = new ConfiguracionGrupoMuscular
        {
            Id = 1,
            GrupoMuscularId = nuevaConfiguracionDTO.GrupoMuscularId,
            MultiplicadorPeso = nuevaConfiguracionDTO.MultiplicadorPeso,
            MultiplicadorRepeticiones = nuevaConfiguracionDTO.MultiplicadorRepeticiones,
            FactorProgresion = nuevaConfiguracionDTO.FactorProgresion
        };

        _configuracionRepositorioMock.Setup(repo => repo.AgregarAsync(It.IsAny<ConfiguracionGrupoMuscular>()))
            .ReturnsAsync(configuracionAgregada);

        var agregarConfiguracionCasoDeUso = new AgregarConfiguracionGrupoMuscularCasoDeUso(_configuracionRepositorioMock.Object, _mapper);

        // Act
        var resultado = await agregarConfiguracionCasoDeUso.Ejecutar(nuevaConfiguracionDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(1);
        resultado.GrupoMuscularId.Should().Be(nuevaConfiguracionDTO.GrupoMuscularId);
        resultado.MultiplicadorPeso.Should().Be(nuevaConfiguracionDTO.MultiplicadorPeso);
        resultado.MultiplicadorRepeticiones.Should().Be(nuevaConfiguracionDTO.MultiplicadorRepeticiones);
        resultado.FactorProgresion.Should().Be(nuevaConfiguracionDTO.FactorProgresion);
    }

    [Fact]
    public async Task DebeMapearCorrectamenteTodosLosCampos()
    {
        // Arrange
        var dto = new AgregarConfiguracionGrupoMuscularDTO
        {
            GrupoMuscularId = 10,
            MultiplicadorPeso = 0.65,
            MultiplicadorRepeticiones = 0.35,
            FactorProgresion = 1.25
        };

        _configuracionRepositorioMock.Setup(repo => repo.AgregarAsync(It.IsAny<ConfiguracionGrupoMuscular>()))
            .ReturnsAsync((ConfiguracionGrupoMuscular c) => { c.Id = 5; return c; });

        var casoDeUso = new AgregarConfiguracionGrupoMuscularCasoDeUso(_configuracionRepositorioMock.Object, _mapper);

        // Act
        var resultado = await casoDeUso.Ejecutar(dto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.GrupoMuscularId.Should().Be(10);
        resultado.MultiplicadorPeso.Should().Be(0.65);
        resultado.MultiplicadorRepeticiones.Should().Be(0.35);
        resultado.FactorProgresion.Should().Be(1.25);
    }

    [Fact]
    public async Task DebeLlamarRepositorioConDatosCorrectos()
    {
        // Arrange
        var dto = new AgregarConfiguracionGrupoMuscularDTO
        {
            GrupoMuscularId = 8,
            MultiplicadorPeso = 0.7,
            MultiplicadorRepeticiones = 0.3,
            FactorProgresion = 1.15
        };

        _configuracionRepositorioMock.Setup(repo => repo.AgregarAsync(It.IsAny<ConfiguracionGrupoMuscular>()))
            .ReturnsAsync((ConfiguracionGrupoMuscular c) => { c.Id = 1; return c; });

        var casoDeUso = new AgregarConfiguracionGrupoMuscularCasoDeUso(_configuracionRepositorioMock.Object, _mapper);

        // Act
        await casoDeUso.Ejecutar(dto);

        // Assert
        _configuracionRepositorioMock.Verify(repo => repo.AgregarAsync(
            It.Is<ConfiguracionGrupoMuscular>(c => c.GrupoMuscularId == dto.GrupoMuscularId && 
                                                   c.MultiplicadorPeso == dto.MultiplicadorPeso &&
                                                   c.MultiplicadorRepeticiones == dto.MultiplicadorRepeticiones &&
                                                   c.FactorProgresion == dto.FactorProgresion)), 
            Times.Once);
    }

    [Fact]
    public async Task DeberiaAgregarConfiguracionesConDiferentesMultiplicadores()
    {
        // Arrange
        var dto1 = new AgregarConfiguracionGrupoMuscularDTO { GrupoMuscularId = 1, MultiplicadorPeso = 0.2, MultiplicadorRepeticiones = 0.8, FactorProgresion = 1.0 };
        var dto2 = new AgregarConfiguracionGrupoMuscularDTO { GrupoMuscularId = 2, MultiplicadorPeso = 0.95, MultiplicadorRepeticiones = 0.05, FactorProgresion = 3.0 };

        _configuracionRepositorioMock.Setup(repo => repo.AgregarAsync(It.IsAny<ConfiguracionGrupoMuscular>()))
            .ReturnsAsync((ConfiguracionGrupoMuscular c) => { c.Id = 1; return c; });

        var casoDeUso = new AgregarConfiguracionGrupoMuscularCasoDeUso(_configuracionRepositorioMock.Object, _mapper);

        // Act
        var resultado1 = await casoDeUso.Ejecutar(dto1);
        var resultado2 = await casoDeUso.Ejecutar(dto2);

        // Assert
        resultado1.MultiplicadorPeso.Should().Be(0.2);
        resultado2.MultiplicadorPeso.Should().Be(0.95);
    }

    [Fact]
    public async Task DeberiaAgregarConfiguracionConValoresExtremos()
    {
        // Arrange
        var dto = new AgregarConfiguracionGrupoMuscularDTO
        {
            GrupoMuscularId = 999,
            MultiplicadorPeso = 1.0,
            MultiplicadorRepeticiones = 0.0,
            FactorProgresion = 5.0
        };

        _configuracionRepositorioMock.Setup(repo => repo.AgregarAsync(It.IsAny<ConfiguracionGrupoMuscular>()))
            .ReturnsAsync((ConfiguracionGrupoMuscular c) => { c.Id = 100; return c; });

        var casoDeUso = new AgregarConfiguracionGrupoMuscularCasoDeUso(_configuracionRepositorioMock.Object, _mapper);

        // Act
        var resultado = await casoDeUso.Ejecutar(dto);

        // Assert
        resultado.MultiplicadorPeso.Should().Be(1.0);
        resultado.MultiplicadorRepeticiones.Should().Be(0.0);
        resultado.FactorProgresion.Should().Be(5.0);
    }

    [Fact]
    public async Task DeberiaRetornarTipoConfiguracionGrupoMuscularDTO()
    {
        // Arrange
        var dto = new AgregarConfiguracionGrupoMuscularDTO
        {
            GrupoMuscularId = 1,
            MultiplicadorPeso = 0.5,
            MultiplicadorRepeticiones = 0.5,
            FactorProgresion = 1.0
        };

        _configuracionRepositorioMock.Setup(repo => repo.AgregarAsync(It.IsAny<ConfiguracionGrupoMuscular>()))
            .ReturnsAsync((ConfiguracionGrupoMuscular c) => { c.Id = 1; return c; });

        var casoDeUso = new AgregarConfiguracionGrupoMuscularCasoDeUso(_configuracionRepositorioMock.Object, _mapper);

        // Act
        var resultado = await casoDeUso.Ejecutar(dto);

        // Assert
        resultado.Should().BeOfType<ConfiguracionGrupoMuscularDTO>();
    }
}
