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
}
