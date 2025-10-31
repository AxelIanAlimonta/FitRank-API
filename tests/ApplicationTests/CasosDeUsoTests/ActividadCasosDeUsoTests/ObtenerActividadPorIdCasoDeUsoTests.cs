using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.MappingProfiles;
using FitRank_API.Application.DTOs.ActividadDTOs;
using FitRank_API.Application.UseCases.Actividad;

namespace FitRank_API.ApplicationTests.CasosDeUsoTests.ActividadCasosDeUsoTests;

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
}