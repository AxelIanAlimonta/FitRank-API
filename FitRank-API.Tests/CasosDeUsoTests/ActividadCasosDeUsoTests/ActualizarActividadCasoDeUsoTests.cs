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

public class ActualizarActividadCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IActividadRepositorio> _actividadRepositorioMock;

    public ActualizarActividadCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new ActividadProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _actividadRepositorioMock = new Mock<IActividadRepositorio>();
    }

    [Fact]
    public async Task ActualizarActividad_CuandoLosDatosSonValidos_RetornaActividadDTO()
    {
        // Arrange
        var actualizarActividadDTO = new ActualizarActividadDTO
        {
            Id = 1,
            Repeticiones = 10,
            Peso = 50.5,
            Punto = 20.0,
            EjercicioAsignadoId = 2,
            EntrenamientoId = 3,
            SerieId = 4
        };

        var actividadExistente = new Actividad
        {
            Id = actualizarActividadDTO.Id,
            Repeticiones = 8,
            Peso = 45.0,
            Punto = 15.0,
            EjercicioAsignadoId = 1,
            EntrenamientoId = 1,
            SerieId = 1
        };

        var actividadActualizada = new Actividad
        {
            Id = actualizarActividadDTO.Id,
            Repeticiones = actualizarActividadDTO.Repeticiones,
            Peso = actualizarActividadDTO.Peso,
            Punto = actualizarActividadDTO.Punto,
            EjercicioAsignadoId = actualizarActividadDTO.EjercicioAsignadoId,
            EntrenamientoId = actualizarActividadDTO.EntrenamientoId,
            SerieId = actualizarActividadDTO.SerieId
        };

        _actividadRepositorioMock
            .Setup(repo => repo.ObtenerPorIdAsync(actualizarActividadDTO.Id))
            .ReturnsAsync(actividadExistente);

        _actividadRepositorioMock
            .Setup(repo => repo.ActualizarAsync(It.IsAny<Actividad>()))
            .ReturnsAsync(actividadActualizada);

        var actualizarActividadCasoDeUso = new ActualizarActividadCasoDeUso(_actividadRepositorioMock.Object, _mapper);

        // Act
        var resultado = await actualizarActividadCasoDeUso.Ejecutar(actualizarActividadDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(actividadActualizada.Id);
        resultado.Repeticiones.Should().Be(actualizarActividadDTO.Repeticiones);
        resultado.Peso.Should().Be(actualizarActividadDTO.Peso);
        resultado.Punto.Should().Be(actualizarActividadDTO.Punto);
        resultado.EjercicioAsignadoId.Should().Be(actualizarActividadDTO.EjercicioAsignadoId);
        resultado.EntrenamientoId.Should().Be(actualizarActividadDTO.EntrenamientoId);
        resultado.SerieId.Should().Be(actualizarActividadDTO.SerieId);
    }

    //actualizar actividad no existente
    [Fact]
    public async Task ActualizarActividad_CuandoLaActividadNoExiste_RetornaNull()
    {
        // Arrange
        var actualizarActividadDTO = new ActualizarActividadDTO
        {
            Id = 99,
            Repeticiones = 10,
            Peso = 50.5,
            Punto = 20.0,
            EjercicioAsignadoId = 2,
            EntrenamientoId = 3,
            SerieId = 4
        };

        _actividadRepositorioMock
            .Setup(repo => repo.ObtenerPorIdAsync(actualizarActividadDTO.Id))
            .ReturnsAsync((Actividad?)null);

        var actualizarActividadCasoDeUso = new ActualizarActividadCasoDeUso(_actividadRepositorioMock.Object, _mapper);

        // Act
        var resultado = await actualizarActividadCasoDeUso.Ejecutar(actualizarActividadDTO);

        // Assert
        resultado.Should().BeNull();
    }

}
