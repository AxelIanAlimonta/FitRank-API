using FitRank_API.Domain.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.DTOs.JornadaDTOs;
using FitRank_API.Application.Mappings;
using FitRank_API.Application.CasosDeUso.JornadaCasosDeUso;

namespace CasosDeUsoTests.JornadaCasosDeUsoTests;

public class ObtenerJornadaPorIdCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IJornadaRepositorio> _jornadaRepositorioMock;

    public ObtenerJornadaPorIdCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new JornadaProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _jornadaRepositorioMock = new Mock<IJornadaRepositorio>();
    }

    [Fact]
    public async Task ObtenerJornadaPorId_CuandoLaJornadaExiste_RetornaJornadaDTO()
    {
        // Arrange
        var jornadaId = 1;
        var jornadaExistente = new Jornada
        {
            Id = jornadaId,
            HoraInicio = new TimeSpan(9, 0, 0),
            HoraFin = new TimeSpan(17, 0, 0),
            ProfesorId = 1,
            DiaDeLaSemanaId = 1
        };

        _jornadaRepositorioMock
            .Setup(repo => repo.ObtenerJornadaPorIdAsync(jornadaId))
            .ReturnsAsync(jornadaExistente);

        var obtenerJornadaPorIdCasoDeUso = new ObtenerJornadaPorIdCasoDeUso(_jornadaRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerJornadaPorIdCasoDeUso.Ejecutar(jornadaId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(jornadaExistente.Id);
        resultado.HoraInicio.Should().Be(jornadaExistente.HoraInicio);
        resultado.HoraFin.Should().Be(jornadaExistente.HoraFin);
        resultado.ProfesorId.Should().Be(jornadaExistente.ProfesorId);
        resultado.DiaDeLaSemanaId.Should().Be(jornadaExistente.DiaDeLaSemanaId);
    }

    [Fact]
    public async Task ObtenerJornadaPorId_CuandoLaJornadaNoExiste_RetornaNull()
    {
        // Arrange
        var jornadaId = 999; // ID que no existe

        _jornadaRepositorioMock
            .Setup(repo => repo.ObtenerJornadaPorIdAsync(jornadaId))
            .ReturnsAsync((Jornada?)null);

        var obtenerJornadaPorIdCasoDeUso = new ObtenerJornadaPorIdCasoDeUso(_jornadaRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerJornadaPorIdCasoDeUso.Ejecutar(jornadaId);

        // Assert
        resultado.Should().BeNull();
    }
}