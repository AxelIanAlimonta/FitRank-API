using AutoMapper;
using FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace FitRank_API.tests.ApplicationTests.CasosDeUsoTests.RutinaCasosDeUsoTests;

public class ObtenerRutinaPorIdCasoDeUsoTests
{
    private readonly Mock<IRutinaRepositorio> _rutinaRepositorioMock;
    private readonly IMapper _mapper;
    private readonly ObtenerRutinaPorIdCasoDeUso _obtenerRutinaPorIdCasoDeUso;

    public ObtenerRutinaPorIdCasoDeUsoTests()
    {
        _rutinaRepositorioMock = new Mock<IRutinaRepositorio>();
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<RutinaProfile>();
        }));
        _obtenerRutinaPorIdCasoDeUso = new ObtenerRutinaPorIdCasoDeUso(_rutinaRepositorioMock.Object, _mapper);
    }

    //obtener rutina por id existente devuelve rutina
    [Fact]
    public async Task ObtenerRutinaPorIdCasoDeUso_IdExistente_RetornaObtenerRutinaDTO()
    {
        // Arrange
        var rutinaId = 1;
        var rutinaEntidad = new Rutina
        {
            Id = rutinaId,
            Nombre = "Rutina de Prueba",
            Descripcion = "Descripción de la rutina de prueba"
        };
        _rutinaRepositorioMock
            .Setup(repo => repo.ObtenerPorIdAsync(rutinaId))
            .ReturnsAsync(rutinaEntidad);
        // Act
        var resultado = await _obtenerRutinaPorIdCasoDeUso.Ejecutar(rutinaId);
        // FluentAssertions
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(rutinaEntidad.Id);
        resultado.Nombre.Should().Be(rutinaEntidad.Nombre);
        resultado.Descripcion.Should().Be(rutinaEntidad.Descripcion);
    }

    //obtener rutina por id no existente devuelve null
    [Fact]
    public async Task ObtenerRutinaPorIdCasoDeUso_IdNoExistente_RetornaNull()
    {
        // Arrange
        var rutinaId = 999; // ID no existente
        _rutinaRepositorioMock
            .Setup(repo => repo.ObtenerPorIdAsync(rutinaId))
            .ReturnsAsync((Rutina?)null);
        // Act
        var resultado = await _obtenerRutinaPorIdCasoDeUso.Ejecutar(rutinaId);
        // FluentAssertions
        resultado.Should().BeNull();
    }
}