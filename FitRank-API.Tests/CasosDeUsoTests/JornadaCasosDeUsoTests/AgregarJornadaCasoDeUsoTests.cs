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

public class AgregarJornadaCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IJornadaRepositorio> _jornadaRepositorioMock;

    public AgregarJornadaCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new JornadaProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _jornadaRepositorioMock = new Mock<IJornadaRepositorio>();
    }

    [Fact]
    public async Task AgregarJornada_CuandoLosDatosSonValidos_RetornaJornadaDTO()
    {
        // Arrange
        var nuevoJornadaDTO = new AgregarJornadaDTO
        {
            HoraInicio = new TimeSpan(9, 0, 0),
            HoraFin = new TimeSpan(17, 0, 0),
            ProfesorId = 1,
            DiaDeLaSemanaId = 1
        };

        var jornadaAGregar = new Jornada
        {
            HoraInicio = nuevoJornadaDTO.HoraInicio,
            HoraFin = nuevoJornadaDTO.HoraFin,
            ProfesorId = nuevoJornadaDTO.ProfesorId,
            DiaDeLaSemanaId = nuevoJornadaDTO.DiaDeLaSemanaId
        };

        var jornadaAgregada = new Jornada
        {
            Id = 1,
            HoraInicio = nuevoJornadaDTO.HoraInicio,
            HoraFin = nuevoJornadaDTO.HoraFin,
            ProfesorId = nuevoJornadaDTO.ProfesorId,
            DiaDeLaSemanaId = nuevoJornadaDTO.DiaDeLaSemanaId
        };

        _jornadaRepositorioMock
            .Setup(repo => repo.AgregarJornadaAsync(It.IsAny<Jornada>()))
            .ReturnsAsync(jornadaAgregada);

        var agregarJornadaCasoDeUso = new AgregarJornadaCasoDeUso(_jornadaRepositorioMock.Object, _mapper);

        // Act
        var resultado = await agregarJornadaCasoDeUso.Ejecutar(nuevoJornadaDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(jornadaAgregada.Id);
        resultado.HoraInicio.Should().Be(nuevoJornadaDTO.HoraInicio);
        resultado.HoraFin.Should().Be(nuevoJornadaDTO.HoraFin);
        resultado.ProfesorId.Should().Be(nuevoJornadaDTO.ProfesorId);
        resultado.DiaDeLaSemanaId.Should().Be(nuevoJornadaDTO.DiaDeLaSemanaId);
    }
}