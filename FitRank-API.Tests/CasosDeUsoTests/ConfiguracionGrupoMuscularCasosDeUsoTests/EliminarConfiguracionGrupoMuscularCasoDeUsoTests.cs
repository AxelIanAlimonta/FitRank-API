using FitRank_API.Domain.Interfaces;
using Moq;
using Xunit;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.ConfiguracionGrupoMuscular;

namespace CasosDeUsoTests.ConfiguracionGrupoMuscularCasosDeUsoTests;

public class EliminarConfiguracionGrupoMuscularCasoDeUsoTests
{
    private readonly Mock<IConfiguracionGrupoMuscularRepositorio> _configuracionRepositorioMock;

    public EliminarConfiguracionGrupoMuscularCasoDeUsoTests()
    {
        _configuracionRepositorioMock = new Mock<IConfiguracionGrupoMuscularRepositorio>();
    }

    [Fact]
    public async Task Ejecutar_DeberiaEliminarConfiguracion_CuandoLaConfiguracionExiste()
    {
        // Arrange
        var configuracionId = 1L;

        _configuracionRepositorioMock.Setup(repo => repo.EliminarAsync(configuracionId))
            .Returns(Task.CompletedTask);

        var eliminarConfiguracionCasoDeUso = new EliminarConfiguracionGrupoMuscularCasoDeUso(_configuracionRepositorioMock.Object);

        // Act
        await eliminarConfiguracionCasoDeUso.Ejecutar(configuracionId);

        // Assert
        _configuracionRepositorioMock.Verify(repo => repo.EliminarAsync(configuracionId), Times.Once);
    }

    [Fact]
    public async Task DebeLlamarRepositorioConIdCorrecto()
    {
        // Arrange
        var configuracionId = 789L;

        _configuracionRepositorioMock.Setup(repo => repo.EliminarAsync(configuracionId))
            .Returns(Task.CompletedTask);

        var casoDeUso = new EliminarConfiguracionGrupoMuscularCasoDeUso(_configuracionRepositorioMock.Object);

        // Act
        await casoDeUso.Ejecutar(configuracionId);

        // Assert
        _configuracionRepositorioMock.Verify(repo => repo.EliminarAsync(configuracionId), Times.Once);
    }

    [Fact]
    public async Task DeberiaEliminarConfiguracionesConDiferentesIds()
    {
        // Arrange
        var ids = new[] { 1L, 50L, 999L };

        foreach (var id in ids)
        {
            _configuracionRepositorioMock.Setup(repo => repo.EliminarAsync(id))
                .Returns(Task.CompletedTask);
        }

        var casoDeUso = new EliminarConfiguracionGrupoMuscularCasoDeUso(_configuracionRepositorioMock.Object);

        // Act & Assert
        foreach (var id in ids)
        {
            await casoDeUso.Ejecutar(id);
        }

        _configuracionRepositorioMock.Verify(repo => repo.EliminarAsync(It.IsAny<long>()), Times.Exactly(3));
    }

    [Fact]
    public async Task DeberiaCompletarOperacionDeEliminacion()
    {
        // Arrange
        var configuracionId = 25L;
        var eliminacionCompletada = false;

        _configuracionRepositorioMock.Setup(repo => repo.EliminarAsync(configuracionId))
            .Callback(() => eliminacionCompletada = true)
            .Returns(Task.CompletedTask);

        var casoDeUso = new EliminarConfiguracionGrupoMuscularCasoDeUso(_configuracionRepositorioMock.Object);

        // Act
        await casoDeUso.Ejecutar(configuracionId);

        // Assert
        eliminacionCompletada.Should().BeTrue();
    }

    [Fact]
    public async Task DeberiaLlamarRepositorioUnaVezPorEliminacion()
    {
        // Arrange
        var configuracionId = 100L;

        _configuracionRepositorioMock.Setup(repo => repo.EliminarAsync(configuracionId))
            .Returns(Task.CompletedTask);

        var casoDeUso = new EliminarConfiguracionGrupoMuscularCasoDeUso(_configuracionRepositorioMock.Object);

        // Act
        await casoDeUso.Ejecutar(configuracionId);
        await casoDeUso.Ejecutar(configuracionId);

        // Assert
        _configuracionRepositorioMock.Verify(repo => repo.EliminarAsync(configuracionId), Times.Exactly(2));
    }
}
