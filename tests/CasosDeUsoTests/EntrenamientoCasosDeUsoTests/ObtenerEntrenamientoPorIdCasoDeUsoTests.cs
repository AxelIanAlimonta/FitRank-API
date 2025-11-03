using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.MappingProfiles;
using FitRank_API.Application.DTOs.EntrenamientoDTOs;
using FitRank_API.Application.UseCases.Entrenamiento;

namespace CasosDeUsoTests.EntrenamientoCasosDeUsoTests;

public class ObtenerEntrenamientoPorIdCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IEntrenamientoRepositorio> _entrenamientoRepositorioMock;

    public ObtenerEntrenamientoPorIdCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new EntrenamientoProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _entrenamientoRepositorioMock = new Mock<IEntrenamientoRepositorio>();
    }

    // [Fact]
    // public async Task ObtenerEntrenamientoPorId_CuandoElEntrenamientoExiste_RetornaEntrenamientoDTO()
    // {
    //     // Arrange
    //     var entrenamientoId = 1;

    //     var entrenamientoExistente = new Entrenamiento
    //     {
    //         Id = entrenamientoId,
    //         SocioId = 1,
    //         Duracion = new DateTime(2023, 1, 22),
    //         Fecha = new DateTime(2023, 1, 22)
    //     };

    //     _entrenamientoRepositorioMock
    //         .Setup(repo => repo.ObtenerPorIdAsync(entrenamientoId))
    //         .ReturnsAsync(entrenamientoExistente);

    //     var casoDeUso = new ObtenerEntrenamientoPorIdCasoDeUso(_entrenamientoRepositorioMock.Object, _mapper);

    //     // Act
    //     var resultado = await casoDeUso.Ejecutar(entrenamientoId);

    //     // Assert
    //     resultado.Should().NotBeNull();
    //     resultado.Id.Should().Be(entrenamientoExistente.Id);
    //     resultado.Fecha.Should().Be(entrenamientoExistente.Fecha);
    //     resultado.Duracion.Should().Be(entrenamientoExistente.Duracion);
    //     resultado.SocioId.Should().Be(entrenamientoExistente.SocioId);
    // }

    [Fact]
    public async Task ObtenerEntrenamientoPorId_CuandoElEntrenamientoNoExiste_RetornaNull()
    {
        // Arrange
        var entrenamientoId = 999; // ID que no existe

        _entrenamientoRepositorioMock
            .Setup(repo => repo.ObtenerPorIdAsync(entrenamientoId))
            .ReturnsAsync((Entrenamiento?)null);

        var casoDeUso = new ObtenerEntrenamientoPorIdCasoDeUso(_entrenamientoRepositorioMock.Object, _mapper);

        // Act
        var resultado = await casoDeUso.Ejecutar(entrenamientoId);

        // Assert
        resultado.Should().BeNull();
    }
}