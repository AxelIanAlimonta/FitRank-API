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

public class ActualizarJornadaCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IJornadaRepositorio> _jornadaRepositorioMock;

    public ActualizarJornadaCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new JornadaProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _jornadaRepositorioMock = new Mock<IJornadaRepositorio>();
    }

    //actualizar jornada exitoso
    [Fact]
    public async Task ActualizarJornada_CuandoLosDatosSonValidos_RetornaJornadaDTO()
    {
        // Arrange
        var jornadaExistente = new Jornada
        {
            Id = 1,
            HoraInicio = new TimeSpan(9, 0, 0),
            HoraFin = new TimeSpan(17, 0, 0),
            ProfesorId = 1,
            DiaDeLaSemanaId = 1
        };

        var actualizarJornadaDTO = new ActualizarJornadaDTO
        {
            Id = 1,
            HoraInicio = new TimeSpan(9, 0, 0),
            HoraFin = new TimeSpan(17, 0, 0),
            ProfesorId = 1,
            DiaDeLaSemanaId = 1
        };

        var jornadaActualizada = new Jornada
        {
            Id = 1,
            HoraInicio = new TimeSpan(10, 0, 0),
            HoraFin = new TimeSpan(18, 0, 0),
            ProfesorId = 1,
            DiaDeLaSemanaId = 1
        };

        _jornadaRepositorioMock
            .Setup(repo => repo.ActualizarJornadaAsync(It.IsAny<Jornada>()))
            .ReturnsAsync(jornadaActualizada);

        var actualizarJornadaCasoDeUso = new ActualizarJornadaCasoDeUso(_jornadaRepositorioMock.Object, _mapper);

        // Act
        var resultado = await actualizarJornadaCasoDeUso.Ejecutar(actualizarJornadaDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(jornadaActualizada.Id);
        resultado.HoraInicio.Should().Be(jornadaActualizada.HoraInicio);
        resultado.HoraFin.Should().Be(jornadaActualizada.HoraFin);
        resultado.ProfesorId.Should().Be(jornadaActualizada.ProfesorId);
        resultado.DiaDeLaSemanaId.Should().Be(jornadaActualizada.DiaDeLaSemanaId);
    }

    //actualizar jornada no existente devuelve null
    [Fact]
    public async Task ActualizarJornada_CuandoLaJornadaNoExiste_RetornaNull()
    {
        // Arrange
        var actualizarJornadaDTO = new ActualizarJornadaDTO
        {
            Id = 99,
            HoraInicio = new TimeSpan(9, 0, 0),
            HoraFin = new TimeSpan(17, 0, 0),
            ProfesorId = 1,
            DiaDeLaSemanaId = 1
        };

        _jornadaRepositorioMock
            .Setup(repo => repo.ActualizarJornadaAsync(It.IsAny<Jornada>()))
            .ReturnsAsync((Jornada?)null);

        var actualizarJornadaCasoDeUso = new ActualizarJornadaCasoDeUso(_jornadaRepositorioMock.Object, _mapper);

        // Act
        var resultado = await actualizarJornadaCasoDeUso.Ejecutar(actualizarJornadaDTO);

        // Assert
        resultado.Should().BeNull();
    }

}