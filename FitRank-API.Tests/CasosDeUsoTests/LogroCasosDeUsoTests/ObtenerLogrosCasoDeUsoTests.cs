using FitRank_API.Domain.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.LogroCasosDeUso;
using FitRank_API.Application.DTOs.LogroDTOs;
using FitRank_API.Application.Mappings;

namespace CasosDeUsoTests.LogroCasoDeUsoTests;

public class ObtenerLogrosCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<ILogroRepositorio> _logroRepositorioMock;

    public ObtenerLogrosCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(
            mc =>
            {
                mc.AddProfile(new LogroProfile());
            }
        );
        _mapper = mappingConfig.CreateMapper();
        _logroRepositorioMock = new Mock<ILogroRepositorio>();
    }

    [Fact]
    public async Task ObtenerLogros_RetornaListaDeLogroDTOs()
    {
        // Arrange
        var logrosExistentes = new List<Logro>
        {
            new Logro
            {
                Id = 1,
                Nombre = "Logro 1",
                Descripcion = "Descripcion del logro 1",
                Imagen = "http://imagen.1"
            },
            new Logro
            {
                Id = 2,
                Nombre = "Logro 2",
                Descripcion = "Descripcion del logro 2",
                Imagen = "http://imagen.2"
            }
        };

        _logroRepositorioMock
            .Setup(repo => repo.ObtenerTodosLosLogros())
            .ReturnsAsync(logrosExistentes);

        var obtenerLogrosCasoDeUso = new ObtenerLogrosCasoDeUso(
            _logroRepositorioMock.Object,
            _mapper
        );

        // Act
        var resultado = await obtenerLogrosCasoDeUso.Ejecutar();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(logrosExistentes.Count);
        for (int i = 0; i < logrosExistentes.Count; i++)
        {
            resultado[i].Id.Should().Be(logrosExistentes[i].Id);
            resultado[i].Nombre.Should().Be(logrosExistentes[i].Nombre);
            resultado[i].Imagen.Should().Be(logrosExistentes[i].Imagen);
        }
    }

    [Fact]
    public async Task ObtenerLogros_CuandoNoHayLogros_RetornaListaVacia()
    {
        // Arrange
        var logrosExistentes = new List<Logro>();

        _logroRepositorioMock
            .Setup(repo => repo.ObtenerTodosLosLogros())
            .ReturnsAsync(logrosExistentes);

        var obtenerLogrosCasoDeUso = new ObtenerLogrosCasoDeUso(
            _logroRepositorioMock.Object,
            _mapper
        );

        // Act
        var resultado = await obtenerLogrosCasoDeUso.Ejecutar();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }
}