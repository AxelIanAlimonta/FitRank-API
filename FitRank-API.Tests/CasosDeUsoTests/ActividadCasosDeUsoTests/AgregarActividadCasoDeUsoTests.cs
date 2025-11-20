using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.MappingProfiles;
using FitRank_API.Application.DTOs.ActividadDTOs;
using FitRank_API.Application.UseCases.Actividad;

namespace CasosDeUsoTests.ActividadCasosDeUsoTests;

public class AgregarActividadCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IActividadRepositorio> _actividadRepositorioMock;

    public AgregarActividadCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new ActividadProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _actividadRepositorioMock = new Mock<IActividadRepositorio>();
    }

    [Fact]
    public async Task AgregarActividad_CuandoLosDatosSonValidos_RetornaActividadDTO()
    {
        // Arrange
        var nuevaActividadDTO = new AgregarActividadDTO
        {
            Repeticiones = 10,
            Peso = 50.5,
            Punto = 20.0,
            EjercicioAsignadoId = 2,
            EntrenamientoId = 3,
            SerieId = 4
        };

        var actividadCreada = new Actividad
        {
            Id = 1,
            Repeticiones = nuevaActividadDTO.Repeticiones,
            Peso = nuevaActividadDTO.Peso,
            Punto = nuevaActividadDTO.Punto,
            EjercicioAsignadoId = nuevaActividadDTO.EjercicioAsignadoId,
            EntrenamientoId = nuevaActividadDTO.EntrenamientoId,
            SerieId = nuevaActividadDTO.SerieId
        };

        _actividadRepositorioMock
            .Setup(repo => repo.AgregarAsync(It.IsAny<Actividad>()))
            .ReturnsAsync(actividadCreada);

        var agregarActividadCasoDeUso = new AgregarActividadCasoDeUso(_actividadRepositorioMock.Object, _mapper);

        // Act
        var resultado = await agregarActividadCasoDeUso.Ejecutar(nuevaActividadDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(actividadCreada.Id);
        resultado.Repeticiones.Should().Be(nuevaActividadDTO.Repeticiones);
        resultado.Peso.Should().Be(nuevaActividadDTO.Peso);
        resultado.Punto.Should().Be(nuevaActividadDTO.Punto);
        resultado.EjercicioAsignadoId.Should().Be(nuevaActividadDTO.EjercicioAsignadoId);
        resultado.EntrenamientoId.Should().Be(nuevaActividadDTO.EntrenamientoId);
        resultado.SerieId.Should().Be(nuevaActividadDTO.SerieId);
    }
}