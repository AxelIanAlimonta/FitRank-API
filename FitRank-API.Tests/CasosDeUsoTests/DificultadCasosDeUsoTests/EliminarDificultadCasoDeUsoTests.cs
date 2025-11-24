using FitRank_API.Application.Mappings;
using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.DificultadCasosDeUso;

namespace CasosDeUsoTests.DificultadCasosDeUsoTests;

public class EliminarDificultadCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IDificultadRepositorio> _dificultadRepositorioMock;

    public EliminarDificultadCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new DificultadProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _dificultadRepositorioMock = new Mock<IDificultadRepositorio>();
    }

    [Fact]
    public async Task Ejecutar_DeberiaEliminarDificultad_CuandoLaDificultadExiste()
    {
        // Arrange
        var dificultadId = 1;

        _dificultadRepositorioMock.Setup(repo => repo.EliminarAsync(dificultadId))
            .Returns(Task.CompletedTask);

        var eliminarDificultadCasoDeUso = new EliminarDificultadCasoDeUso(_dificultadRepositorioMock.Object, _mapper);

        // Act
        await eliminarDificultadCasoDeUso.Ejecutar(dificultadId);

        // Assert
        _dificultadRepositorioMock.Verify(repo => repo.EliminarAsync(dificultadId), Times.Once);
    }
}
