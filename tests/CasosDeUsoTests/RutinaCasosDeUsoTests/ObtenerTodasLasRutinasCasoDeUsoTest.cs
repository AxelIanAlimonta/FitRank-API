using AutoMapper;
using FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace CasosDeUsoTests.RutinaCasosDeUsoTests;

public class ObtenerTodasLasRutinasCasoDeUsoTest
{
    private readonly Mock<IRutinaRepositorio> _rutinaRepositorioMock;
    private readonly IMapper _mapper;
    private readonly ObtenerTodasLasRutinasCasoDeUso _obtenerTodasLasRutinasCasoDeUso;

    public ObtenerTodasLasRutinasCasoDeUsoTest()
    {
        _rutinaRepositorioMock = new Mock<IRutinaRepositorio>();
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<RutinaProfile>();
        }));
        _obtenerTodasLasRutinasCasoDeUso = new ObtenerTodasLasRutinasCasoDeUso(_rutinaRepositorioMock.Object, _mapper);
    }

    [Fact]
    public async Task ObtenerTodasLasRutinasCasoDeUso_RetornaListaDeObtenerRutinaDTO()
    {
        // Arrange
        var rutinasEntidad = new List<Rutina>
        {
            new Rutina { Id = 1, Nombre = "Rutina 1", Descripcion = "Descripción 1" },
            new Rutina { Id = 2, Nombre = "Rutina 2", Descripcion = "Descripción 2" }
        };
        _rutinaRepositorioMock
            .Setup(repo => repo.ObtenerTodasAsync())
            .ReturnsAsync(rutinasEntidad);
        // Act
        var resultado = await _obtenerTodasLasRutinasCasoDeUso.Ejecutar();
        // FluentAssertions
        resultado.Should().NotBeNull();
        resultado.Count().Should().Be(rutinasEntidad.Count);
    }
}