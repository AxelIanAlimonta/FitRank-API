using FitRank_API.Application.Mappings;
using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Application.DTOs;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.DTOs.EjercicioDTOs.ActualizarEjercicioDTO;
using FitRank_API.Application.CasosDeUso.EjercicioCasosDeUso;

namespace CasosDeUsoTests.EjercicioCasosDeUsoTests;

public class EliminarEjercicioCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IEjercicioRepositorio> _ejercicioRepositorioMock;

    public EliminarEjercicioCasoDeUsoTests()
    {
        _ejercicioRepositorioMock = new Mock<IEjercicioRepositorio>();
    }

    //eliminar ejercicio tiene exito
    [Fact]
    public async Task Ejecutar_DeberiaEliminarEjercicio_CuandoElEjercicioExiste()
    {
        // Arrange
        var ejercicioId = 1;

        _ejercicioRepositorioMock.Setup(repo => repo.EliminarEjercicioAsync(ejercicioId))
            .ReturnsAsync(true);

        var eliminarEjercicioCasoDeUso = new EliminarEjercicioCasoDeUso(_ejercicioRepositorioMock.Object);

        // Act
        var resultado = await eliminarEjercicioCasoDeUso.Ejecutar(ejercicioId);

        // Assert
        resultado.Should().BeTrue();
    }

    //eliminar ejercicio falla porque no existe
    [Fact]
    public async Task Ejecutar_DeberiaFallarAlEliminarEjercicio_CuandoElEjercicioNoExiste()
    {
        // Arrange
        var ejercicioId = 999; // ID que no existe

        _ejercicioRepositorioMock.Setup(repo => repo.EliminarEjercicioAsync(ejercicioId))
            .ReturnsAsync(false);

        var eliminarEjercicioCasoDeUso = new EliminarEjercicioCasoDeUso(_ejercicioRepositorioMock.Object);

        // Act
        var resultado = await eliminarEjercicioCasoDeUso.Ejecutar(ejercicioId);

        // Assert
        resultado.Should().BeFalse();
    }
}