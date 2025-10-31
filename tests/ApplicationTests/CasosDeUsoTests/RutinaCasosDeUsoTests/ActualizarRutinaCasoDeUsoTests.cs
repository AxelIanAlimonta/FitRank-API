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

public class ActualizarRutinaCasoDeUsoTests
{
    private readonly Mock<IRutinaRepositorio> _rutinaRepositorioMock;
    private readonly IMapper _mapper;
    private readonly ActualizarRutinaCasoDeUso _actualizarRutinaCasoDeUso;

    public ActualizarRutinaCasoDeUsoTests()
    {
        _rutinaRepositorioMock = new Mock<IRutinaRepositorio>();
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<RutinaProfile>();
        }));
        _actualizarRutinaCasoDeUso = new ActualizarRutinaCasoDeUso(_rutinaRepositorioMock.Object, _mapper);
    }

    //actualizar rutina actualiza exitosamente
    [Fact]
    public async Task ActualizarRutinaCasoDeUso_ActualizacionExitosa_RetornaObtenerRutinaDTO()
    {
        // Arrange
        var actualizarRutinaDTO = new ActualizarRutinaDTO
        {
            Id = 1,
            Nombre = "Rutina Actualizada",
            Descripcion = "Descripción actualizada de la rutina"
        };
        var rutinaEntidad = new Rutina
        {
            Id = actualizarRutinaDTO.Id,
            Nombre = actualizarRutinaDTO.Nombre,
            Descripcion = actualizarRutinaDTO.Descripcion
        };
        _rutinaRepositorioMock
            .Setup(repo => repo.ActualizarAsync(It.IsAny<Rutina>()))
            .ReturnsAsync(rutinaEntidad);
        // Act
        var resultado = await _actualizarRutinaCasoDeUso.Ejecutar(actualizarRutinaDTO);
        // FluentAssertions
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(rutinaEntidad.Id);
        resultado.Nombre.Should().Be(rutinaEntidad.Nombre);
        resultado.Descripcion.Should().Be(rutinaEntidad.Descripcion);
    }

    //actualizar rutina con id no existente devuelve null
    [Fact]
    public async Task ActualizarRutinaCasoDeUso_IdNoExistente_RetornaNull()
    {
        // Arrange
        var actualizarRutinaDTO = new ActualizarRutinaDTO
        {
            Id = 99,
            Nombre = "Rutina No Existente",
            Descripcion = "Descripción de la rutina no existente"
        };
        _rutinaRepositorioMock
            .Setup(repo => repo.ActualizarAsync(It.IsAny<Rutina>()))
            .ReturnsAsync((Rutina?)null);
        // Act
        var resultado = await _actualizarRutinaCasoDeUso.Ejecutar(actualizarRutinaDTO);
        // FluentAssertions
        resultado.Should().BeNull();
    }
}