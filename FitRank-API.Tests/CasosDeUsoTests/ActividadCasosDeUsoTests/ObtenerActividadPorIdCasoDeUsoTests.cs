using FitRank_API.Domain.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.Mappings;
using FitRank_API.Application.DTOs.ActividadDTOs;
using FitRank_API.Application.UseCases.Actividad;

namespace CasosDeUsoTests.ActividadCasosDeUsoTests;

public class ObtenerActividadPorIdCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IActividadRepositorio> _actividadRepositorioMock;

    public ObtenerActividadPorIdCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new ActividadProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _actividadRepositorioMock = new Mock<IActividadRepositorio>();
    }

    [Fact]
    public async Task ObtenerActividadPorId_CuandoLaActividadExiste_RetornaActividadDTO()
    {
        // Arrange
        var actividadId = 1;
        var actividadEnLaBaseDeDatos = new Actividad
        {
            Id = actividadId,
            Repeticiones = 10,
            Peso = 50.5,
            Punto = 20.0,
            EjercicioAsignadoId = 2,
            EntrenamientoId = 3,
            SerieId = 4
        };

        _actividadRepositorioMock
            .Setup(repo => repo.ObtenerPorIdAsync(actividadId))
            .ReturnsAsync(actividadEnLaBaseDeDatos);

        var obtenerActividadPorIdCasoDeUso = new ObtenerActividadPorIdCasoDeUso(_actividadRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerActividadPorIdCasoDeUso.Ejecutar(actividadId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(actividadEnLaBaseDeDatos.Id);
        resultado.Repeticiones.Should().Be(actividadEnLaBaseDeDatos.Repeticiones);
        resultado.Peso.Should().Be(actividadEnLaBaseDeDatos.Peso);
        resultado.Punto.Should().Be(actividadEnLaBaseDeDatos.Punto);
        resultado.EjercicioAsignadoId.Should().Be(actividadEnLaBaseDeDatos.EjercicioAsignadoId);
        resultado.EntrenamientoId.Should().Be(actividadEnLaBaseDeDatos.EntrenamientoId);
        resultado.SerieId.Should().Be(actividadEnLaBaseDeDatos.SerieId);
    }

    [Fact]
    public async Task ObtenerActividadPorId_CuandoLaActividadNoExiste_RetornaNull()
    {
        // Arrange
        var actividadId = 99;

        _actividadRepositorioMock
            .Setup(repo => repo.ObtenerPorIdAsync(actividadId))
            .ReturnsAsync((Actividad?)null);

        var obtenerActividadPorIdCasoDeUso = new ObtenerActividadPorIdCasoDeUso(_actividadRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerActividadPorIdCasoDeUso.Ejecutar(actividadId);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ObtenerActividadPorId_DebeMapearCorrectamenteTodosLosCampos()
    {
        // Arrange
        var actividadId = 5L;
        var actividadExistente = new Actividad
        {
            Id = actividadId,
            Repeticiones = 15,
            Peso = 75.25,
            Punto = 30.5,
            EjercicioAsignadoId = 10,
            EntrenamientoId = 20,
            SerieId = 30
        };

        _actividadRepositorioMock
            .Setup(repo => repo.ObtenerPorIdAsync(actividadId))
            .ReturnsAsync(actividadExistente);

        var casoDeUso = new ObtenerActividadPorIdCasoDeUso(_actividadRepositorioMock.Object, _mapper);

        // Act
        var resultado = await casoDeUso.Ejecutar(actividadId);

        // Assert
        resultado.Should().BeOfType<ObtenerActividadDTO>();
        resultado!.Id.Should().Be(5);
        resultado.Repeticiones.Should().Be(15);
        resultado.Peso.Should().Be(75.25);
        resultado.Punto.Should().Be(30.5);
        resultado.EjercicioAsignadoId.Should().Be(10);
        resultado.EntrenamientoId.Should().Be(20);
        resultado.SerieId.Should().Be(30);
    }

    [Fact]
    public async Task ObtenerActividadPorId_DebeLlamarRepositorioConIdCorrecto()
    {
        // Arrange
        var actividadId = 123L;
        var actividadExistente = new Actividad { Id = actividadId };

        _actividadRepositorioMock
            .Setup(repo => repo.ObtenerPorIdAsync(actividadId))
            .ReturnsAsync(actividadExistente);

        var casoDeUso = new ObtenerActividadPorIdCasoDeUso(_actividadRepositorioMock.Object, _mapper);

        // Act
        await casoDeUso.Ejecutar(actividadId);

        // Assert
        _actividadRepositorioMock.Verify(repo => repo.ObtenerPorIdAsync(actividadId), Times.Once);
    }
}