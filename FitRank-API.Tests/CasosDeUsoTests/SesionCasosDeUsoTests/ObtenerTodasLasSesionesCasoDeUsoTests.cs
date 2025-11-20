using AutoMapper;
using FitRank_API.Application.CasosDeUso.SesionCasosDeUso;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace CasosDeUsoTests.SesionCasosDeUsoTests;

public class ObtenerTodasLasSesionesCasoDeUsoTests
{
    private readonly Mock<ISesionRepositorio> _sesionRepositorioMock;
    private readonly IMapper _mapper;
    private readonly ObtenerTodasLasSesionesCasoDeUso _obtenerTodasLasSesionesCasoDeUso;

    public ObtenerTodasLasSesionesCasoDeUsoTests()
    {
        _sesionRepositorioMock = new Mock<ISesionRepositorio>();
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SesionProfile>();
        }));
        _obtenerTodasLasSesionesCasoDeUso = new ObtenerTodasLasSesionesCasoDeUso(_sesionRepositorioMock.Object, _mapper);
    }

    //obtener todas las sesiones exitoso
    [Fact]
    public async Task ObtenerTodasLasSesionesCasoDeUso_ObtencionExitosa_RetornaListaDeObtenerSesionDTO()
    {
        // Arrange
        var sesionesEntidad = new List<Sesion>
        {
            new Sesion { Id = 1, NumeroDeSesion = 1, Nombre = "Sesión 1", RutinaId = 1 },
            new Sesion { Id = 2, NumeroDeSesion = 2, Nombre = "Sesión 2", RutinaId = 1 }
        };
        _sesionRepositorioMock
            .Setup(repo => repo.ObtenerTodasAsync())
            .ReturnsAsync(sesionesEntidad);
        // Act
        var resultado = await _obtenerTodasLasSesionesCasoDeUso.Ejecutar();
        // Assert
        resultado.Should().NotBeNull();
        resultado.Count.Should().Be(sesionesEntidad.Count);
    }
}