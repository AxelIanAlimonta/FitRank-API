using FitRank_API.Infrastructure.Interfaces;
using Moq;
using Xunit;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.ProfesorCasosDeUso;

namespace CasosDeUsoTests.ProfesorCasosDeUsoTests;

public class EliminarProfesorCasoDeUsoTests
{
    private readonly Mock<IProfesorRepositorio> _profesorRepositorioMock;

    public EliminarProfesorCasoDeUsoTests()
    {
        _profesorRepositorioMock = new Mock<IProfesorRepositorio>();
    }

    [Fact]
    public async Task Ejecutar_DeberiaEliminarProfesor_CuandoElProfesorExiste()
    {
        // Arrange
        var profesorId = 1L;

        _profesorRepositorioMock.Setup(repo => repo.EliminarAsync(profesorId))
            .ReturnsAsync(true);

        var eliminarProfesorCasoDeUso = new EliminarProfesorCasoDeUso(_profesorRepositorioMock.Object);

        // Act
        var resultado = await eliminarProfesorCasoDeUso.Ejecutar(profesorId);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task Ejecutar_DeberiaFallarAlEliminarProfesor_CuandoElProfesorNoExiste()
    {
        // Arrange
        var profesorId = 999L;

        _profesorRepositorioMock.Setup(repo => repo.EliminarAsync(profesorId))
            .ReturnsAsync(false);

        var eliminarProfesorCasoDeUso = new EliminarProfesorCasoDeUso(_profesorRepositorioMock.Object);

        // Act
        var resultado = await eliminarProfesorCasoDeUso.Ejecutar(profesorId);

        // Assert
        resultado.Should().BeFalse();
    }
}
