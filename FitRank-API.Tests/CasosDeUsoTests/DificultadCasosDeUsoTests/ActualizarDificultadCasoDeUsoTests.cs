using FitRank_API.Application.Mappings;
using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.DificultadCasosDeUso;
using FitRank_API.Application.DTOs.DificultadDTOs;
using FitRank_API.Domain.Entities;

namespace CasosDeUsoTests.DificultadCasosDeUsoTests;

public class ActualizarDificultadCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IDificultadRepositorio> _dificultadRepositorioMock;

    public ActualizarDificultadCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new DificultadProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _dificultadRepositorioMock = new Mock<IDificultadRepositorio>();
    }

    [Fact]
    public async Task Ejecutar_DeberiaActualizarDificultad_CuandoLosDatosSonValidos()
    {
        // Arrange
        var dificultadActualizadaDTO = new DificultadDTO
        {
            Id = 1,
            Descripcion = "Avanzado"
        };

        var dificultadActualizada = new Dificultad
        {
            Id = 1,
            Descripcion = "Avanzado"
        };

        _dificultadRepositorioMock.Setup(repo => repo.ActualizarAsync(It.IsAny<Dificultad>()))
            .ReturnsAsync(dificultadActualizada);

        var actualizarDificultadCasoDeUso = new ActualizarDificultadCasoDeUso(_dificultadRepositorioMock.Object, _mapper);

        // Act
        var resultado = await actualizarDificultadCasoDeUso.Ejecutar(dificultadActualizadaDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(1);
        resultado.Descripcion.Should().Be("Avanzado");
    }

    [Fact]
    public async Task Ejecutar_DeberiaRetornarNull_CuandoLaDificultadNoExiste()
    {
        // Arrange
        var dificultadActualizadaDTO = new DificultadDTO
        {
            Id = 99,
            Descripcion = "No Existe"
        };

        _dificultadRepositorioMock.Setup(repo => repo.ActualizarAsync(It.IsAny<Dificultad>()))
            .ReturnsAsync((Dificultad?)null);

        var actualizarDificultadCasoDeUso = new ActualizarDificultadCasoDeUso(_dificultadRepositorioMock.Object, _mapper);

        // Act
        var resultado = await actualizarDificultadCasoDeUso.Ejecutar(dificultadActualizadaDTO);

        // Assert
        resultado.Should().BeNull();
    }
}
