using AutoMapper;
using FitRank_API.Application.CasosDeUso.SesionCasosDeUso;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace CasosDeUsoTests.SesionCasosDeUsoTests;

public class ObtenerSesionPorIdCasoDeUsoTests
{
    private readonly Mock<ISesionRepositorio> _sesionRepositorioMock;
    private readonly IMapper _mapper;
    private readonly ObtenerSesionPorIdCasoDeUso _obtenerSesionPorIdCasoDeUso;

    public ObtenerSesionPorIdCasoDeUsoTests()
    {
        _sesionRepositorioMock = new Mock<ISesionRepositorio>();
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SesionProfile>();
        }));
        _obtenerSesionPorIdCasoDeUso = new ObtenerSesionPorIdCasoDeUso(_sesionRepositorioMock.Object, _mapper);
    }

    // Obtener sesión por ID exitosa
    [Fact]
    public async Task ObtenerSesionPorIdCasoDeUso_ObtencionExitosa_RetornaObtenerSesionDTO()
    {
        // Arrange
        var sesionId = 1L;
        var sesionEntidad = new Sesion
        {
            Id = sesionId,
            NumeroDeSesion = 1,
            Nombre = "Sesión de Prueba",
            RutinaId = 1
        };
        _sesionRepositorioMock
            .Setup(repo => repo.ObtenerPorIdAsync(sesionId))
            .ReturnsAsync(sesionEntidad);
        // Act
        var resultado = await _obtenerSesionPorIdCasoDeUso.Ejecutar(sesionId);
        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(sesionEntidad.Id);
        resultado.NumeroDeSesion.Should().Be(sesionEntidad.NumeroDeSesion);
        resultado.Nombre.Should().Be(sesionEntidad.Nombre);
        resultado.RutinaId.Should().Be(sesionEntidad.RutinaId);
    }

    // Obtener sesión por ID no existente devuelve null
    [Fact]
    public async Task ObtenerSesionPorIdCasoDeUso_IdNoExistente_RetornaNull()
    {
        // Arrange
        var sesionId = 999L;
        _sesionRepositorioMock
            .Setup(repo => repo.ObtenerPorIdAsync(sesionId))
            .ReturnsAsync((Sesion?)null);
        // Act
        var resultado = await _obtenerSesionPorIdCasoDeUso.Ejecutar(sesionId);
        // Assert
        resultado.Should().BeNull();
    }
}