using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.MappingProfiles;
using FitRank_API.Application.DTOs.JornadaDTOs;
using FitRank_API.Application.Mappings;
using FitRank_API.Application.CasosDeUso.JornadaCasosDeUso;

namespace CasosDeUsoTests.JornadaCasosDeUsoTests;

public class ObtenerTodasLasJornadasCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IJornadaRepositorio> _jornadaRepositorioMock;

    public ObtenerTodasLasJornadasCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new JornadaProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _jornadaRepositorioMock = new Mock<IJornadaRepositorio>();
    }

    [Fact]
    public async Task ObtenerTodasLasJornadas_CuandoExistenJornadas_RetornaListaDeJornadaDTO()
    {
        // Arrange
        var jornadasExistentes = new List<Jornada>
        {
            new Jornada
            {
                Id = 1,
                HoraInicio = new TimeSpan(9, 0, 0),
                HoraFin = new TimeSpan(17, 0, 0),
                ProfesorId = 1,
                DiaDeLaSemanaId = 1
            },
            new Jornada
            {
                Id = 2,
                HoraInicio = new TimeSpan(10, 0, 0),
                HoraFin = new TimeSpan(18, 0, 0),
                ProfesorId = 2,
                DiaDeLaSemanaId = 2
            }
        };

        _jornadaRepositorioMock
            .Setup(repo => repo.ObtenerTodasLasJornadasAsync())
            .ReturnsAsync(jornadasExistentes);

        var obtenerTodasLasJornadasCasoDeUso = new ObtenerTodasLasJornadasCasoDeUso(_jornadaRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerTodasLasJornadasCasoDeUso.Ejecutar();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Count.Should().Be(jornadasExistentes.Count);
    }

    [Fact]
    public async Task ObtenerTodasLasJornadas_CuandoNoExistenJornadas_RetornaListaVacia()
    {
        // Arrange
        var jornadasExistentes = new List<Jornada>();

        _jornadaRepositorioMock
            .Setup(repo => repo.ObtenerTodasLasJornadasAsync())
            .ReturnsAsync(jornadasExistentes);

        var obtenerTodasLasJornadasCasoDeUso = new ObtenerTodasLasJornadasCasoDeUso(_jornadaRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerTodasLasJornadasCasoDeUso.Ejecutar();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Count.Should().Be(0);
    }

}