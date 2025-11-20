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

public class AgregarRutinaCasoDeUsoTest
{
    private readonly Mock<IRutinaRepositorio> _rutinaRepositorioMock;
    private readonly IMapper _mapper;
    private readonly AgregarRutinaCasoDeUso _agregarRutinaCasoDeUso;

    public AgregarRutinaCasoDeUsoTest()
    {
        _rutinaRepositorioMock = new Mock<IRutinaRepositorio>();
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<RutinaProfile>();
        }));
        _agregarRutinaCasoDeUso = new AgregarRutinaCasoDeUso(_rutinaRepositorioMock.Object, _mapper);
    }

    [Fact]
    public async Task AgregarRutinaCasoDeUso_CreacionExitosa_RetornaObtenerRutinaDTO()
    {
        // Arrange
        var agregarRutinaDTO = new AgregarRutinaDTO
        {
            Nombre = "Rutina de Prueba",
            Descripcion = "Descripción de la rutina de prueba"
        };
        var rutinaEntidad = new Rutina
        {
            Id = 1,
            Nombre = agregarRutinaDTO.Nombre,
            Descripcion = agregarRutinaDTO.Descripcion
        };
        _rutinaRepositorioMock
            .Setup(repo => repo.AgregarAsync(It.IsAny<Rutina>()))
            .ReturnsAsync(rutinaEntidad);
        // Act
        var resultado = await _agregarRutinaCasoDeUso.Ejecutar(agregarRutinaDTO);
        // FluentAssertions
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(rutinaEntidad.Id);
        resultado.Nombre.Should().Be(rutinaEntidad.Nombre);
        resultado.Descripcion.Should().Be(rutinaEntidad.Descripcion);
    }

}