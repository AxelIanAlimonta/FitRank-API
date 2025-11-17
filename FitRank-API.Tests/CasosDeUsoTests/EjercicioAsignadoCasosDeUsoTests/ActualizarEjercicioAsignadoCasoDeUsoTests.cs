using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.EjercicioAsignadoCasoDeUso;
using FitRank_API.Application.DTOs.EjercicioAsignadoDTOs;
using FitRank_API.Application.Mappings;

namespace CasosDeUsoTests.EjercicioAsignadoCasoDeUsoTests;

public class ActualizarEjercicioAsignadoCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IEjercicioAsignadoRepositorio> _ejercicioAsignadoRepositorioMock;

    public ActualizarEjercicioAsignadoCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(
            mc =>
            {
                mc.AddProfile(new EjercicioAsignadoProfile());
            }
        );
        _mapper = mappingConfig.CreateMapper();
        _ejercicioAsignadoRepositorioMock = new Mock<IEjercicioAsignadoRepositorio>();
    }

    //actualizar ejercicio asignado tiene exito
    [Fact]
    public async Task Ejecutar_DeberiaActualizarEjercicioAsignado_CuandoElEjercicioAsignadoExiste()
    {
        // Arrange
        var actualizarEjercicioAsignadoDTO = new ActualizarEjercicioAsignadoDTO
        {
            Id = 1,
            NumeroEjercicio = 2,
            EjercicioId = 2,
            SesionId = 1,
        };

        var ejercicioAsignadoActualizado = new EjercicioAsignado
        {
            Id = actualizarEjercicioAsignadoDTO.Id,
            NumeroEjercicio = actualizarEjercicioAsignadoDTO.NumeroEjercicio,
            EjercicioId = actualizarEjercicioAsignadoDTO.EjercicioId,
            SesionId = actualizarEjercicioAsignadoDTO.SesionId,
        };

        _ejercicioAsignadoRepositorioMock.Setup(repo => repo.ActualizarAsync(It.IsAny<EjercicioAsignado>()))
            .ReturnsAsync(ejercicioAsignadoActualizado);

        var actualizarEjercicioAsignadoCasoDeUso = new ActualizarEjercicioAsignadoCasoDeUso(_ejercicioAsignadoRepositorioMock.Object, _mapper);

        // Act
        var resultado = await actualizarEjercicioAsignadoCasoDeUso.Ejecutar(actualizarEjercicioAsignadoDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(ejercicioAsignadoActualizado.Id);
        resultado.NumeroEjercicio.Should().Be(ejercicioAsignadoActualizado.NumeroEjercicio);
        resultado.EjercicioId.Should().Be(ejercicioAsignadoActualizado.EjercicioId);
        resultado.SesionId.Should().Be(ejercicioAsignadoActualizado.SesionId);
    }

    //actualizar ejercicio asignado falla porque no existe
    [Fact]
    public async Task Ejecutar_DeberiaFallarAlActualizarEjercicioAsignado_CuandoElEjercicioAsignadoNoExiste()
    {
        // Arrange
        var actualizarEjercicioAsignadoDTO = new ActualizarEjercicioAsignadoDTO
        {
            Id = 999, // ID que no existe
            NumeroEjercicio = 2,
            EjercicioId = 2,
            SesionId = 1,
        };

        _ejercicioAsignadoRepositorioMock.Setup(repo => repo.ActualizarAsync(It.IsAny<EjercicioAsignado>()))
            .ReturnsAsync((EjercicioAsignado?)null);

        var actualizarEjercicioAsignadoCasoDeUso = new ActualizarEjercicioAsignadoCasoDeUso(_ejercicioAsignadoRepositorioMock.Object, _mapper);

        // Act
        var resultado = await actualizarEjercicioAsignadoCasoDeUso.Ejecutar(actualizarEjercicioAsignadoDTO);

        // Assert
        resultado.Should().BeNull();
    }
}