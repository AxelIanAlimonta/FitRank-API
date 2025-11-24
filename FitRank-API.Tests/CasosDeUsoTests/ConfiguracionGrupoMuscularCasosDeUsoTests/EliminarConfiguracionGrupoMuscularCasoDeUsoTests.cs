using FitRank_API.Infrastructure.Interfaces;
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
}
