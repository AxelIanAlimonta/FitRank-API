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

public class ObtenerLogroPorIdCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<ILogroRepositorio> _logroRepositorioMock;

    public ObtenerLogroPorIdCasoDeUsoTests()
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
    public async Task ObtenerLogroPorId_CuandoElLogroExiste_RetornaLogroDTO()
    {
        // Arrange
        var logroId = 1;

        var logroExistente = new Logro
        {
            Id = logroId,
            Nombre = "Logro Existente",
            Descripcion = "Descripcion del logro existente",
            Imagen = "http://imagen.existente"
        };

        _logroRepositorioMock
            .Setup(repo => repo.ObtenerLogroPorId(logroId))
            .ReturnsAsync(logroExistente);

        var obtenerLogroPorIdCasoDeUso = new ObtenerLogroPorIdCasoDeUso(
            _logroRepositorioMock.Object,
            _mapper
        );

        // Act
        var resultado = await obtenerLogroPorIdCasoDeUso.Ejecutar(logroId);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(logroExistente.Id);
        resultado.Nombre.Should().Be(logroExistente.Nombre);
        resultado.Imagen.Should().Be(logroExistente.Imagen);
    }

    [Fact]
    public async Task ObtenerLogroPorId_CuandoElLogroNoExiste_RetornaNull()
    {
        // Arrange
        var logroId = 999; // ID que no existe

        _logroRepositorioMock
            .Setup(repo => repo.ObtenerLogroPorId(logroId))
            .ReturnsAsync((Logro?)null);

        var obtenerLogroPorIdCasoDeUso = new ObtenerLogroPorIdCasoDeUso(
            _logroRepositorioMock.Object,
            _mapper
        );

        // Act
        var resultado = await obtenerLogroPorIdCasoDeUso.Ejecutar(logroId);

        // Assert
        resultado.Should().BeNull();
    }
}