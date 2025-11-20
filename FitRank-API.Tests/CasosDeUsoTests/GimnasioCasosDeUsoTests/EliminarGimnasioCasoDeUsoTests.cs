using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.GimnasioCasosDeUso;
using FitRank_API.Application.DTOs.GimnasioDTOs;
using FitRank_API.Application.Mappings;

namespace CasosDeUsoTests.GimnasioCasoDeUsoTests;

public class EliminarGimnasioCasoDeUsoTests
{
    private readonly Mock<IGimnasioRepositorio> _gimnasioRepositorioMock;

    public EliminarGimnasioCasoDeUsoTests()
    {
        _gimnasioRepositorioMock = new Mock<IGimnasioRepositorio>();
    }

    //eliminar gimnasio tiene exito
    [Fact]
    public async Task Ejecutar_DeberiaEliminarGimnasio_CuandoElGimnasioExiste()
    {
        // Arrange
        var gimnasioIdAEliminar = 1;
        _gimnasioRepositorioMock.Setup(repo => repo.EliminarGimnasio(gimnasioIdAEliminar))
            .ReturnsAsync(true);

        // Act
        var eliminarGimnasioCasoDeUso = new EliminarGimnasioCasoDeUso(_gimnasioRepositorioMock.Object);
        var resultado = await eliminarGimnasioCasoDeUso.Ejecutar(gimnasioIdAEliminar);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task Ejecutar_DeberiaDevolverFalse_CuandoElGimnasioNoExiste()
    {
        // Arrange
        var gimnasioIdAEliminar = 99; // Suponiendo que este ID no existe
        _gimnasioRepositorioMock.Setup(repo => repo.EliminarGimnasio(gimnasioIdAEliminar))
            .ReturnsAsync(false);

        // Act
        var eliminarGimnasioCasoDeUso = new EliminarGimnasioCasoDeUso(_gimnasioRepositorioMock.Object);
        var resultado = await eliminarGimnasioCasoDeUso.Ejecutar(gimnasioIdAEliminar);

        // Assert
        resultado.Should().BeFalse();
    }
}