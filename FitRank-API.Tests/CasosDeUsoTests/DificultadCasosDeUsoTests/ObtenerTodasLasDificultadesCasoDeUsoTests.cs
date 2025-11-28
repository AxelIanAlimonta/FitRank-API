using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.DificultadCasosDeUso;
using FitRank_API.Domain.Entities;

namespace CasosDeUsoTests.DificultadCasosDeUsoTests;

public class ObtenerTodasLasDificultadesCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IDificultadRepositorio> _dificultadRepositorioMock;

    public ObtenerTodasLasDificultadesCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new DificultadProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _dificultadRepositorioMock = new Mock<IDificultadRepositorio>();
    }

    [Fact]
    public async Task Ejecutar_DeberiaRetornarTodasLasDificultades_CuandoExistenDificultades()
    {
        // Arrange
        var dificultadesEnLaBaseDeDatos = new List<Dificultad>
        {
            new Dificultad { Id = 1, Descripcion = "Principiante" },
            new Dificultad { Id = 2, Descripcion = "Intermedio" },
            new Dificultad { Id = 3, Descripcion = "Avanzado" }
        };

        _dificultadRepositorioMock.Setup(repo => repo.ObtenerTodosAsync())
            .ReturnsAsync(dificultadesEnLaBaseDeDatos);

        var obtenerTodasLasDificultadesCasoDeUso = new ObtenerTodasLasDificultadesCasoDeUso(_dificultadRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerTodasLasDificultadesCasoDeUso.Ejecutar();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Count.Should().Be(3);
        resultado[0].Id.Should().Be(1);
        resultado[0].Descripcion.Should().Be("Principiante");
        resultado[1].Id.Should().Be(2);
        resultado[1].Descripcion.Should().Be("Intermedio");
        resultado[2].Id.Should().Be(3);
        resultado[2].Descripcion.Should().Be("Avanzado");
    }

    [Fact]
    public async Task Ejecutar_DeberiaRetornarListaVacia_CuandoNoExistenDificultades()
    {
        // Arrange
        var dificultadesEnLaBaseDeDatos = new List<Dificultad>();

        _dificultadRepositorioMock.Setup(repo => repo.ObtenerTodosAsync())
            .ReturnsAsync(dificultadesEnLaBaseDeDatos);

        var obtenerTodasLasDificultadesCasoDeUso = new ObtenerTodasLasDificultadesCasoDeUso(_dificultadRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerTodasLasDificultadesCasoDeUso.Ejecutar();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Count.Should().Be(0);
    }
}
