using FitRank_API.Application.Mappings;
using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.ConfiguracionGrupoMuscular;
using FitRank_API.Domain.Entities;

namespace CasosDeUsoTests.ConfiguracionGrupoMuscularCasosDeUsoTests;

public class ObtenerTodasLasConfiguracionGrupoMuscularCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IConfiguracionGrupoMuscularRepositorio> _configuracionRepositorioMock;

    public ObtenerTodasLasConfiguracionGrupoMuscularCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new ConfiguracionGrupoMuscularProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _configuracionRepositorioMock = new Mock<IConfiguracionGrupoMuscularRepositorio>();
    }

    [Fact]
    public async Task Ejecutar_DeberiaRetornarTodasLasConfiguraciones_CuandoExistenConfiguraciones()
    {
        // Arrange
        var configuracionesEnLaBaseDeDatos = new List<ConfiguracionGrupoMuscular>
        {
            new ConfiguracionGrupoMuscular 
            { 
                Id = 1, 
                GrupoMuscularId = 1, 
                MultiplicadorPeso = 0.5, 
                MultiplicadorRepeticiones = 0.3, 
                FactorProgresion = 1.1 
            },
            new ConfiguracionGrupoMuscular 
            { 
                Id = 2, 
                GrupoMuscularId = 2, 
                MultiplicadorPeso = 0.6, 
                MultiplicadorRepeticiones = 0.4, 
                FactorProgresion = 1.2 
            },
            new ConfiguracionGrupoMuscular 
            { 
                Id = 3, 
                GrupoMuscularId = 3, 
                MultiplicadorPeso = 0.7, 
                MultiplicadorRepeticiones = 0.5, 
                FactorProgresion = 1.3 
            }
        };

        _configuracionRepositorioMock.Setup(repo => repo.ObtenerTodosAsync())
            .ReturnsAsync(configuracionesEnLaBaseDeDatos);

        var obtenerTodasLasConfiguracionesCasoDeUso = new ObtenerTodasLasConfiguracionGrupoMuscularCasoDeUso(_configuracionRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerTodasLasConfiguracionesCasoDeUso.Ejecutar();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Count.Should().Be(3);
        resultado[0].Id.Should().Be(1);
        resultado[0].GrupoMuscularId.Should().Be(1);
        resultado[1].Id.Should().Be(2);
        resultado[1].GrupoMuscularId.Should().Be(2);
        resultado[2].Id.Should().Be(3);
        resultado[2].GrupoMuscularId.Should().Be(3);
    }

    [Fact]
    public async Task Ejecutar_DeberiaRetornarListaVacia_CuandoNoExistenConfiguraciones()
    {
        // Arrange
        var configuracionesEnLaBaseDeDatos = new List<ConfiguracionGrupoMuscular>();

        _configuracionRepositorioMock.Setup(repo => repo.ObtenerTodosAsync())
            .ReturnsAsync(configuracionesEnLaBaseDeDatos);

        var obtenerTodasLasConfiguracionesCasoDeUso = new ObtenerTodasLasConfiguracionGrupoMuscularCasoDeUso(_configuracionRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerTodasLasConfiguracionesCasoDeUso.Ejecutar();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Count.Should().Be(0);
    }
}
